using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using ManufacturingTimeTracking.Models;

namespace ManufacturingTimeTracking.Pages.Admin.Users;

[Authorize(Policy = "CanManageUsers")]
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public IList<UserRoleViewModel> Users { get; set; } = new List<UserRoleViewModel>();

    public class UserRoleViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
    }

    public async Task OnGetAsync()
    {
        var allUsers = _userManager.Users.OrderBy(u => u.Email).ToList();
        foreach (var user in allUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);
            Users.Add(new UserRoleViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                Roles = roles.Any() ? string.Join(", ", roles) : "—"
            });
        }
    }
}
