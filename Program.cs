using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using ManufacturingTimeTracking.Data;
using ManufacturingTimeTracking.Models;
using System.Data.Common;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptions =>
        sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => 
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    
    // User settings
    options.User.RequireUniqueEmail = true;
    
    // Sign in settings
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization(options =>
{
    // Inventory menu: only users with Inventory or Admin role
    options.AddPolicy("CanUseInventory", p => p.RequireRole("Inventory", "Admin"));
    // Time management menu: Operator, Supervisor, Admin
    options.AddPolicy("CanUseTimeManagement", p => p.RequireRole("Operator", "Supervisor", "Admin"));
});

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ManufacturingTimeTracking.Services.InventoryService>();
builder.Services.AddScoped<ManufacturingTimeTracking.Services.OpenAIVisionService>();
builder.Services.AddHttpClient();

// Add forwarded headers support for reverse proxies (ngrok, Caddy gateway).
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
    options.KnownProxies.Add(IPAddress.Loopback);
    options.ForwardLimit = 1;
});

// Configure Kestrel to listen on all network interfaces
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5173); // HTTP
    options.ListenAnyIP(7245, listenOptions =>
    {
        listenOptions.UseHttps();
    }); // HTTPS
});

var app = builder.Build();

// When behind LFSInventario gateway, app is served at /manufacturing (path base from config)
var pathBase = builder.Configuration["PathBase"] ?? builder.Configuration["ASPNETCORE_PATHBASE"];
if (!string.IsNullOrWhiteSpace(pathBase))
{
    pathBase = pathBase.Trim().TrimEnd('/');
    if (!string.IsNullOrEmpty(pathBase))
        app.UsePathBase("/" + pathBase.TrimStart('/'));
}

// Use forwarded headers (important for ngrok and reverse proxies)
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Skip HTTPS redirect when behind gateway (PathBase set); proxy already serves HTTPS.
if (string.IsNullOrWhiteSpace(pathBase))
    app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        // Try to apply migrations, or create database if no migrations exist
        var migrationFailedDueToExistingTables = false;
        try
        {
            context.Database.Migrate();
        }
        catch (InvalidOperationException)
        {
            context.Database.EnsureCreated();
        }
        catch (SqlException ex) when (ex.Number == 2714 || ex.Number == 2715) // Object already exists / Duplicate object
        {
            migrationFailedDueToExistingTables = true;
        }

        var connection = context.Database.GetDbConnection();
        await connection.OpenAsync();
        try
        {
            if (migrationFailedDueToExistingTables)
            {
                // Database already had tables (e.g. from EnsureCreated). Mark migration as applied and ensure inventory schema exists.
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                        IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260209161308_AddInventory')
                        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260209161308_AddInventory', N'9.0.0');";
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            // Always ensure inventory tables (Locations, Items, etc.) so the app works after clone/pull on another machine.
            await DatabaseStartup.EnsureInventorySchemaAsync(connection, services.GetRequiredService<ILogger<Program>>());

            // Add ImageUrl to Materials and MaterialId to Items if missing
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Materials]') AND name = 'ImageUrl')
                    BEGIN
                        ALTER TABLE [Materials] ADD [ImageUrl] NVARCHAR(MAX) NULL;
                    END";
                await command.ExecuteNonQueryAsync();
            }
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Items')
                    AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Items]') AND name = 'MaterialId')
                    BEGIN
                        ALTER TABLE [dbo].[Items] ADD [MaterialId] INT NULL;
                        IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Items_Materials_MaterialId')
                            ALTER TABLE [dbo].[Items] ADD CONSTRAINT [FK_Items_Materials_MaterialId]
                                FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Materials] ([Id]) ON DELETE SET NULL;
                        IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Items_MaterialId' AND object_id = OBJECT_ID(N'[dbo].[Items]'))
                            CREATE NONCLUSTERED INDEX [IX_Items_MaterialId] ON [dbo].[Items] ([MaterialId]);
                    END";
                await command.ExecuteNonQueryAsync();
            }
        }
        finally
        {
            await connection.CloseAsync();
        }

        // Seed data
        await SeedData.InitializeAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
