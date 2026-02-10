# How to Change ngrok Domain Name

## Quick Guide

### Step 1: Get Your Custom Domain
1. Go to https://dashboard.ngrok.com
2. Sign in to your account
3. Go to **"Cloud Edge"** → **"Domains"** (or **"Domains"** in the menu)
4. Reserve a domain (e.g., `myapp.ngrok-free.app`)
   - Free accounts: Can reserve 1 static domain
   - Paid accounts: Can reserve multiple domains

### Step 2: Edit the Config File
1. Open `ngrok-config.txt` in your project folder
2. Change this line:
   ```
   DOMAIN=
   ```
   To your domain (without https://):
   ```
   DOMAIN=myapp.ngrok-free.app
   ```
3. Save the file

### Step 3: Restart ngrok
1. Stop ngrok if it's running (Ctrl+C)
2. Run `start-ngrok.ps1` again
3. It will now use your custom domain!

## Examples

### Using Custom Domain
```
DOMAIN=manufacturing-app.ngrok-free.app
```

### Using Random Domain (Free - Changes Each Time)
```
DOMAIN=
```
(Leave it empty)

## Notes

- **Free accounts**: You can reserve 1 static domain that stays the same
- **Paid accounts**: Can have multiple custom domains
- **No domain set**: ngrok will generate a random URL each time
- **Domain format**: Don't include `https://`, just the domain name (e.g., `myapp.ngrok-free.app`)

## Troubleshooting

- **Domain not found**: Make sure you reserved the domain in your ngrok dashboard
- **Still using random domain**: Check that `ngrok-config.txt` is in the same folder as the script
- **Domain already in use**: Make sure you're not running multiple ngrok instances
