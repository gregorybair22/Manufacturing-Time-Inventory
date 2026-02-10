# Setting Up ngrok for Remote Access

Since your laptops are on different networks, you'll need to use ngrok to create a tunnel to your local server.

## Step 1: Install ngrok

### Option A: Automatic Installation (Recommended)

Run the installation script:
```powershell
.\install-ngrok.ps1
```

This script will:
- Download ngrok automatically
- Install it to `%LOCALAPPDATA%\ngrok\`
- Optionally add it to your PATH
- Guide you through the setup

### Option B: Manual Installation

1. Download ngrok from: https://ngrok.com/download
2. Extract `ngrok.exe` to a folder (e.g., `C:\ngrok\`)
3. Add ngrok to your PATH, or use the full path in the scripts

## Step 2: Sign up and Configure ngrok

1. Sign up for a free account at: https://ngrok.com
2. Get your authtoken from the dashboard: https://dashboard.ngrok.com/get-started/your-authtoken
3. Configure the authtoken using one of these methods:

   **Option A: Using the helper script (Recommended)**
   ```powershell
   .\configure-ngrok-token.ps1 YOUR_AUTHTOKEN_HERE
   ```

   **Option B: Using ngrok directly**
   ```powershell
   ngrok config add-authtoken YOUR_AUTHTOKEN_HERE
   ```

   If ngrok is not in your PATH, use the full path:
   ```powershell
   $env:LOCALAPPDATA\ngrok\ngrok.exe config add-authtoken YOUR_AUTHTOKEN_HERE
   ```

## Step 3: Start Your Application

Make sure your ASP.NET Core application is running on:
- HTTP: `http://localhost:5173`
- HTTPS: `https://localhost:7245`

## Step 4: Start ngrok Tunnel

### Option A: Using HTTP (Easier, Recommended for Testing)

Run the PowerShell script:
```powershell
.\start-ngrok.ps1
```

Or manually:
```powershell
ngrok http 5173
```

### Option B: Using HTTPS

Run the PowerShell script:
```powershell
.\start-ngrok-https.ps1
```

Or manually:
```powershell
ngrok http 7245
```

## Step 5: Get the Public URL

After starting ngrok, you'll see output like:
```
Forwarding   https://abc123.ngrok-free.app -> http://localhost:5173
```

The `https://abc123.ngrok-free.app` is your public URL that you can share.

## Step 6: Access from the Other Laptop

1. Copy the ngrok URL (e.g., `https://abc123.ngrok-free.app`)
2. Open it in a browser on the other laptop
3. You may see an ngrok warning page - click "Visit Site" to proceed

## Important Notes

- **Free ngrok accounts**: The URL changes every time you restart ngrok
- **Paid ngrok accounts**: You can get a fixed domain name
- **Security**: ngrok URLs are public, so be careful with sensitive data
- **Performance**: There may be some latency since traffic goes through ngrok's servers

## Alternative: Use ngrok Web Interface

You can also check the ngrok web interface at `http://127.0.0.1:4040` to see:
- Request logs
- The public URL
- Traffic inspection

## Troubleshooting

- If ngrok doesn't start, make sure your application is running first
- If you get connection errors, check that the application is listening on the correct port
- Make sure Windows Firewall allows ngrok to run
