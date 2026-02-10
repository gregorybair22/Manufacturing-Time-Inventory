# Manufacturing Time Tracking & Inventory

A single web application that combines **time tracking** (manufacturing/assembly by phases and steps) and **inventory** (items, locations, stock, movements) in one database. Some users can access only time management, others only inventory, and items can be moved from warehouse to production stations (shared items).

## Single database, role-based access

- **One database** (`ManufacturingTimeTracking`) for both time tracking and inventory.
- **Time management menu** (Models, Templates, Resources, Orders): visible to roles **Admin**, **Supervisor**, **Operator**.
- **Inventory menu** (Items, Locations, Stock, Movements, Operations): visible to **Admin** and **Inventory**.
- **Shared items:** Inventory items can be linked to process Materials (same product in warehouse and production). Use **Putaway** and **Pick** to move items from warehouse to production stations (create locations with type "Workstation").

**Test accounts:**

| Role       | Email               | Password      | Access              |
|-----------|---------------------|---------------|---------------------|
| Admin     | admin@test.com      | Admin123!     | Time + Inventory    |
| Supervisor| supervisor@test.com | Supervisor123!| Time only           |
| Operator  | operator@test.com   | Operator123!  | Time only           |
| Inventory | inventory@test.com   | Inventory123! | Inventory only      |

## 🚀 Quick Start Guide

### First Time Setup (5 Minutes)

1. **Open the project** in Visual Studio
2. **Press F5** or click "Run" to start the application
3. **Wait** for the browser to open automatically
4. **Login** with:
   - Email: `admin@test.com`
   - Password: `Admin123!`
5. **Done!** You're ready to use the system

> 💡 **Note:** The database is created automatically on first run. No manual setup needed!

### Troubleshooting (clone/pull on another machine)

- **"Invalid object name 'Locations'"**  
  The app now ensures inventory tables (Locations, Items, etc.) exist on every startup. If you still see this, run the app once so migrations and schema setup complete, or run `dotnet ef database update` from the project folder.

- **"Address already in use" (e.g. port 5173)**  
  Another process is using the default HTTP/HTTPS ports. Either stop that process or use different ports by adding to `appsettings.Development.json`:
  ```json
  "Kestrel": { "HttpPort": 5174, "HttpsPort": 7246 }
  ```
  Then open `http://localhost:5174` (or your chosen HTTP port).

---

## 📋 Complete User Guide

### 🔐 Step 1: Login

**Where:** Top right corner of the page

**How:**
1. Click the **"Login"** button
2. Enter your email and password
3. Click **"Log in"**

**Test Accounts:** See table above. For inventory-only access use `inventory@test.com` / `Inventory123!`

---

### 🏭 Step 2: Create Machine Models

**Purpose:** Define the types of machines you manufacture

**Location:** Click **"Models"** in the top menu

**Steps:**
1. Click **"Create New Model"** button (blue button at top)
2. Type a name (example: `S600` or `D600`)
3. Click **"Create"** button
4. ✅ Model created! You'll see it in the list

**To Add Variants:**
1. Click **"Details"** next to your model
2. Scroll down to **"Add New Variant"** section
3. Fill in:
   - **Name:** (example: `UHF` or `VHF`)
   - **Code:** (example: `UHF-001`)
4. Click **"Add Variant"**
5. ✅ Variant appears in the list above

**To Delete:**
- **Model:** Click **"Delete"** button → Confirm
- **Variant:** Click **"Delete"** next to the variant → Confirm

---

### 🔧 Step 3: Add Tools and Materials

**Purpose:** Create a catalog of tools and materials used in manufacturing

**Location:** Click **"Resources"** → Choose **"Tools"** or **"Materials"**

#### Adding Tools:
1. Click **"Create New Tool"**
2. Enter tool name (example: `Screwdriver`, `Wrench`)
3. Click **"Create"**
4. ✅ Tool added!

#### Adding Materials:
1. Click **"Create New Material"**
2. Enter:
   - **Name:** (example: `Screw M4x20`)
   - **Unit:** (example: `pcs` or `kg`) - Optional
3. Click **"Create"**
4. ✅ Material added!

**To Edit or Delete:**
- Click **"Edit"** to change name
- Click **"Delete"** to remove

---

### 📝 Step 4: Create a Process Template

**Purpose:** Define the manufacturing process (phases and steps) for a model+variant

**Location:** Click **"Templates"** in the top menu

**Steps:**
1. Click **"Create New Template"**
2. Select:
   - **Machine Model:** Choose from dropdown
   - **Machine Variant:** Will load automatically
3. Click **"Create"**
4. You'll see the template editor page

#### Adding Phases:
1. Find **"Add New Phase"** section
2. Enter:
   - **Name:** (example: `Preparation`, `Assembly`, `Testing`)
   - **Sort Order:** Number (1, 2, 3...)
3. Click **"Add Phase"**
4. ✅ Phase appears in the list

#### Adding Steps to Each Phase:
1. Find the phase you want to add steps to
2. In that phase's **"Add New Step"** section, enter:
   - **Title:** (example: `Check materials`, `Install component`)
   - **Instructions:** What to do (detailed description)
   - **Sort Order:** Number within the phase (1, 2, 3...)
   - **Allow Skip:** ☑ Check if step can be skipped
3. Click **"Add Step"**
4. ✅ Step appears under that phase
5. Repeat for all steps

**To Edit or Delete:**
- Click **"Edit"** next to template/phase/step
- Click **"Delete"** to remove

---

### 📦 Step 5: Create a Build Order

**Purpose:** Create an order for manufacturing a specific unit

**Location:** Click **"Orders"** in the top menu

**Steps:**
1. Click **"Create New Order"** button
2. Fill in the form:
   - **External Reference:** Your reference number (example: `ORD-001`)
   - **Serial Number:** Unique serial (example: `SN-2026-001`)
   - **Machine Model:** Select from dropdown
   - **Machine Variant:** Loads automatically
3. Click **"Create"**
4. ✅ Order created with status "Pending"

---

### ⚙️ Step 6: Execute Manufacturing

**Purpose:** Actually perform the manufacturing and track time

**Location:** Orders list → Click **"Execute"** button

#### Starting Manufacturing:
1. Review order information at top
2. Click **"Start Manufacturing"** button
3. ✅ Template snapshot created, status changes to "InProgress"

#### Working with Steps:

**▶️ To Start a Step:**
1. Find the step you want to work on
2. Click **"Start"** button
3. (Optional) Select a **Workstation** from dropdown
4. Click **"Start"** in the popup
5. ✅ Step status = "In Progress", timer starts

**⏹️ To Stop a Step:**
1. When finished, click **"Stop"** button
2. ✅ Time recorded automatically, status = "Done"

**⏭️ To Skip a Step:**
1. Click **"Skip"** button (only if step allows skipping)
2. Type a **reason** (required)
3. Click **"Skip Step"**
4. ✅ Status = "Skipped"

**🔄 To Rework (Do Step Again):**
1. For completed steps, click **"Rework"** button
2. (Optional) Select workstation
3. Click **"Start"**
4. ✅ New run starts, all runs tracked separately

**📷 To Add Evidence:**
1. Click **"Add Evidence"** button on any step
2. (Optional) Upload an **Image** file
3. (Optional) Type a **Note**
4. Click **"Upload"**
5. ✅ Evidence thumbnail appears on step card

**➕ To Add New Step During Execution:**
1. Click **"Add Step"** button at top of any phase
2. Enter:
   - **Step Title** (required)
   - **Instructions** (optional)
   - ☑ **Allow Skip** (optional)
3. Click **"Add Step"**
4. ✅ New step appears immediately, ready to execute

---

### 📊 Step 7: View Summary and Export

**Purpose:** See complete execution report and export data

**Location:** Orders list → Click **"Summary"** button

**What You'll See:**
- Order information
- Execution timeline
- All phases and steps
- Total times for each step
- Number of runs (rework count)
- Workstations used

**To Export CSV:**
1. Click **"Export CSV"** button
2. File downloads automatically
3. Open in Excel or any spreadsheet program
4. ✅ Contains all execution data

---

### 👁️ Step 8: View Order Details

**Location:** Orders list → Click **"Details"** button

**What You'll See:**
- All executions for that order
- Execution history
- Status of each execution

---

## 🗑️ How to Delete Items

| Item | Where to Delete | How |
|------|----------------|-----|
| **Model** | Models list | Click **"Delete"** → Confirm |
| **Variant** | Model Details page | Click **"Delete"** next to variant → Confirm |
| **Tool** | Tools list | Click **"Delete"** → Confirm |
| **Material** | Materials list | Click **"Delete"** → Confirm |
| **Template** | Templates list | Click **"Delete"** → Confirm |
| **Phase/Step** | Template Edit page | Click **"Delete"** → Confirm |

> ⚠️ **Warning:** Deleting a Model also deletes all its Variants!

---

## 🎯 Typical Workflow

```
1. Login
   ↓
2. Create Model + Add Variants
   ↓
3. (Optional) Add Tools & Materials
   ↓
4. Create Template → Add Phases → Add Steps
   ↓
5. Create Order
   ↓
6. Execute Manufacturing → Start/Stop Steps
   ↓
7. View Summary → Export CSV
```

---

## 🔧 Technical Setup

### Prerequisites
- .NET 9.0 SDK
- SQL Server LocalDB (included with Visual Studio)
- Visual Studio 2022 (recommended)

### Running the Application

**Option 1: Visual Studio**
1. Open `ManufacturingTimeTracking.csproj`
2. Press **F5** or click Run
3. Browser opens automatically

**Option 2: Command Line**
```bash
dotnet restore
dotnet build
dotnet run
```

### Database
- Created automatically on first run
- Uses LocalDB by default
- Seed data (test users, sample model) added automatically

### Configuration Files
- `appsettings.Development.json` - Development settings
- `appsettings.json` - Production settings

---

## 📁 Project Structure

```
ManufacturingTimeTracking/
├── Pages/
│   ├── Account/          # Login/Logout
│   ├── Catalog/          # Models & Variants
│   ├── Templates/        # Process Templates
│   ├── Orders/           # Orders & Execution
│   ├── Tools/            # Tools Management
│   └── Materials/        # Materials Management
├── Models/               # Data Models
├── Data/                 # Database Context
└── wwwroot/             # Static Files & Uploads
```

---

## ❓ Troubleshooting

### Can't Login?
- Check email and password are correct
- Make sure database was created (first run takes longer)

### Can't Add Variant?
- Make sure Name and Code fields are filled
- Check you're on the Model Details page

### Can't Delete?
- Make sure item isn't being used in a Template or Order
- Check you clicked the Delete button and confirmed

### Application Won't Start?
- Stop Visual Studio debugger
- Close any running instances
- Rebuild solution (Build → Rebuild Solution)

### Database Errors?
- Check SQL Server LocalDB is running
- Verify connection string in `appsettings.Development.json`

---

## 📞 Need Help?

If you encounter issues:
1. Check this README first
2. Review error messages carefully
3. Check the Troubleshooting section above
4. Contact the development team

---

## ✅ Features Checklist

- ✅ User authentication (Login/Logout)
- ✅ Machine model and variant management
- ✅ Tools and materials catalog
- ✅ Process template creation
- ✅ Build order management
- ✅ Manufacturing execution with timing
- ✅ Step skipping with reasons
- ✅ Rework tracking (multiple runs)
- ✅ Workstation selection
- ✅ Image evidence upload
- ✅ Add steps during execution
- ✅ Order summary and CSV export
- ✅ Delete functionality for all items

---

**Last Updated:** January 2026
