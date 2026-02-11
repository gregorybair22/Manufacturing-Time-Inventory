# Manufacturing Time Tracking & Inventory System
## Complete User Guide

**Version:** 1.0  
**Last Updated:** February 2026  
**Document Type:** Detailed Step-by-Step Usage Guide

---

## Table of Contents

1. [Introduction](#1-introduction)
2. [System Overview](#2-system-overview)
3. [Getting Started](#3-getting-started)
4. [User Roles and Access](#4-user-roles-and-access)
5. [Time Tracking Module](#5-time-tracking-module)
6. [Inventory Module](#6-inventory-module)
7. [Integration Workflows](#7-integration-workflows)
8. [Reports and Analytics](#8-reports-and-analytics)
9. [Troubleshooting](#9-troubleshooting)
10. [Best Practices](#10-best-practices)
11. [Appendix](#11-appendix)

---

## 1. Introduction

### 1.1 Purpose of This Guide

This comprehensive usage guide provides detailed, step-by-step instructions for using the Manufacturing Time Tracking & Inventory System. Whether you are a new user or an experienced operator, this guide will help you navigate all features of the system effectively.

### 1.2 What is This System?

The Manufacturing Time Tracking & Inventory System is a unified web application that combines two essential manufacturing functions:

- **Time Tracking**: Track manufacturing and assembly processes by phases and steps, recording time spent on each operation
- **Inventory Management**: Manage items, locations, stock levels, and movements between warehouse and production stations

Both modules share a single database, allowing seamless integration between production planning and inventory management.

### 1.3 Key Benefits

- **Unified System**: One application for both time tracking and inventory
- **Role-Based Access**: Different user roles see only relevant features
- **Real-Time Tracking**: Track manufacturing progress and inventory movements in real-time
- **Complete Audit Trail**: All actions are logged with timestamps and user information
- **Export Capabilities**: Export data to CSV for analysis and reporting
- **Mobile-Friendly**: Works on tablets and smartphones for shop floor use

---

## 2. System Overview

### 2.1 Architecture

The system consists of two main modules:

#### Time Tracking Module
- **Models**: Define machine types and variants
- **Templates**: Create manufacturing process templates with phases and steps
- **Orders**: Create build orders for specific units
- **Execution**: Execute manufacturing processes and track time
- **Resources**: Manage tools and materials catalog

#### Inventory Module
- **Items**: Manage inventory items (SKU, name, family, model/type)
- **Locations**: Define warehouse locations and production stations
- **Stock**: View current stock levels by location
- **Movements**: Track all inventory movements
- **Operations**: Perform putaway and pick operations

### 2.2 Database Structure

- **Single Database**: Both modules use the same database (`ManufacturingTimeTracking`)
- **Shared Items**: Inventory items can be linked to process materials
- **Unified User Management**: Single authentication system for all users

### 2.3 Technology Stack

- **Framework**: ASP.NET Core (Razor Pages)
- **Database**: SQL Server (LocalDB for development)
- **Frontend**: HTML, CSS, JavaScript, Bootstrap
- **Authentication**: ASP.NET Core Identity

---

## 3. Getting Started

### 3.1 System Requirements

#### Minimum Requirements
- **Operating System**: Windows 10/11, Windows Server 2016+, or Linux
- **.NET Runtime**: .NET 9.0 SDK
- **Database**: SQL Server 2019+ or SQL Server LocalDB
- **Browser**: Chrome, Edge, Firefox, or Safari (latest versions)
- **RAM**: 4 GB minimum (8 GB recommended)
- **Storage**: 500 MB for application + database space

#### Recommended Setup
- **Development**: Visual Studio 2022
- **Production**: IIS or Kestrel web server
- **Database**: SQL Server Express or Standard Edition

### 3.2 First-Time Setup

#### Step 1: Launch the Application

1. **Option A - Visual Studio**:
   - Open the project in Visual Studio 2022
   - Press **F5** or click the **Run** button
   - Wait for the browser to open automatically

2. **Option B - Command Line**:
   ```bash
   cd D:\Manufacturing-Time-Inventory
   dotnet restore
   dotnet build
   dotnet run
   ```
   - Note the URL displayed (typically `http://localhost:5173` or `https://localhost:7246`)
   - Open your browser and navigate to the displayed URL

#### Step 2: Verify Database Creation

- The database is created automatically on first run
- You may see a brief delay (10-30 seconds) during initial startup
- If you see any database errors, wait a moment and refresh the page

#### Step 3: Login

1. Click the **"Login"** button in the top right corner
2. Use the default admin credentials:
   - **Email**: `admin@test.com`
   - **Password**: `Admin123!`
3. Click **"Log in"**
4. You should now see the main dashboard

#### Step 4: Verify Access

- **Admin users** should see both **Time Tracking** and **Inventory** menus
- **Time-only users** see: Models, Templates, Resources, Orders
- **Inventory-only users** see: Items, Locations, Stock, Movements, Operations

### 3.3 Initial Configuration

#### Configure Workstations (Optional but Recommended)

1. Navigate to **Workstations** (if visible in menu)
2. Click **"Create New Workstation"**
3. Enter:
   - **Name**: e.g., `WS-01`, `Assembly Station 1`
   - **Code**: e.g., `WS-01`
4. Click **"Create"**
5. Repeat for all production workstations

#### Configure Inventory Locations (For Inventory Users)

1. Navigate to **Locations**
2. Create locations for:
   - Warehouse shelves (type: `Shelf`)
   - Production stations (type: `Workstation`)
   - Receiving area (type: `Receiving`)
   - Shipping area (type: `Shipping`)

---

## 4. User Roles and Access

### 4.1 Available Roles

| Role | Email | Password | Access Level |
|------|-------|----------|-------------|
| **Admin** | admin@test.com | Admin123! | Full access (Time + Inventory) |
| **Supervisor** | supervisor@test.com | Supervisor123! | Time tracking only |
| **Operator** | operator@test.com | Operator123! | Time tracking only |
| **Inventory** | inventory@test.com | Inventory123! | Inventory only |

### 4.2 Role Permissions

#### Admin Role
- ✅ Create, edit, delete models and variants
- ✅ Create, edit, delete templates
- ✅ Create, edit, delete orders
- ✅ Execute manufacturing processes
- ✅ Manage inventory items
- ✅ Manage locations
- ✅ Perform putaway and pick operations
- ✅ View all reports
- ✅ Export data

#### Supervisor Role
- ✅ View models and variants
- ✅ View templates
- ✅ Create and edit orders
- ✅ Execute manufacturing processes
- ✅ View execution summaries
- ❌ Cannot delete models/templates
- ❌ No inventory access

#### Operator Role
- ✅ View models and variants
- ✅ View templates
- ✅ View orders
- ✅ Execute manufacturing processes (start/stop steps)
- ✅ Add evidence to steps
- ❌ Cannot create orders
- ❌ Cannot modify templates
- ❌ No inventory access

#### Inventory Role
- ✅ Manage inventory items
- ✅ Manage locations
- ✅ View stock levels
- ✅ Perform putaway operations
- ✅ Perform pick operations
- ✅ View movement history
- ✅ View inventory reports
- ❌ No time tracking access

### 4.3 Changing User Roles

**Note**: Role changes must be performed by a system administrator through the database or application configuration. Contact your IT administrator for role modifications.

---

## 5. Time Tracking Module

### 5.1 Overview

The Time Tracking module allows you to:
- Define machine models and variants
- Create manufacturing process templates
- Create build orders
- Execute manufacturing processes
- Track time spent on each step
- Record evidence and notes
- Export execution data

### 5.2 Step 1: Create Machine Models

**Purpose**: Define the types of machines you manufacture (e.g., S600, D600)

**Access**: Admin, Supervisor, Operator (view only for Operator)

#### Detailed Steps:

1. **Navigate to Models**
   - Click **"Models"** in the top navigation menu
   - You'll see a list of existing models (if any)

2. **Create a New Model**
   - Click the **"Create New Model"** button (blue button at the top)
   - A form will appear

3. **Enter Model Information**
   - **Name**: Enter a descriptive name
     - Example: `S600`, `D600`, `Industrial Robot`
     - Use clear, consistent naming conventions
   - Click **"Create"** button

4. **Verify Creation**
   - The new model appears in the list
   - You can see:
     - Model name
     - Number of variants
     - Actions (Details, Delete)

#### Adding Variants to a Model

**Purpose**: Define different configurations or versions of a model

1. **Open Model Details**
   - Click **"Details"** next to the model name
   - You'll see the model details page

2. **Navigate to Variants Section**
   - Scroll down to the **"Variants"** section
   - You'll see existing variants (if any)

3. **Add a New Variant**
   - Find the **"Add New Variant"** section
   - Fill in the form:
     - **Name**: Variant name (e.g., `UHF`, `VHF`, `Standard`, `Premium`)
     - **Code**: Unique variant code (e.g., `UHF-001`, `VHF-001`)
       - **Important**: Codes must be unique
   - Click **"Add Variant"** button

4. **Verify Variant Creation**
   - The variant appears in the variants list above
   - You can see:
     - Variant name
     - Variant code
     - Delete button

#### Editing Models and Variants

**To Edit a Model**:
- Currently, models can only be deleted and recreated
- To change a model name, delete and recreate it (⚠️ This will delete all variants)

**To Edit a Variant**:
- Variants can be deleted and recreated
- Click **"Delete"** next to the variant → Confirm → Recreate with new information

#### Deleting Models and Variants

**To Delete a Variant**:
1. Go to Model Details page
2. Find the variant in the list
3. Click **"Delete"** next to the variant
4. Confirm deletion in the popup
5. ✅ Variant is removed

**To Delete a Model**:
1. Go to Models list
2. Click **"Delete"** next to the model
3. Confirm deletion
4. ⚠️ **Warning**: This will delete ALL variants associated with this model
5. ✅ Model and all variants are removed

**Best Practice**: Only delete models/variants that are not used in any templates or orders.

---

### 5.3 Step 2: Add Tools and Materials

**Purpose**: Create a catalog of tools and materials used in manufacturing processes

**Access**: Admin, Supervisor (view only for Operator)

#### Adding Tools

**Tools** are equipment or instruments used during manufacturing (e.g., screwdrivers, wrenches, measuring devices).

1. **Navigate to Tools**
   - Click **"Resources"** in the top menu
   - Select **"Tools"** from the dropdown or click the Tools link

2. **Create a New Tool**
   - Click **"Create New Tool"** button
   - Enter the tool name:
     - Examples: `Phillips Screwdriver`, `Torque Wrench`, `Multimeter`, `Calipers`
     - Use descriptive names
   - Click **"Create"** button

3. **Verify Creation**
   - The tool appears in the tools list
   - You can see:
     - Tool name
     - Actions (Edit, Delete)

#### Adding Materials

**Materials** are consumable items or components used in manufacturing (e.g., screws, wires, components).

1. **Navigate to Materials**
   - Click **"Resources"** in the top menu
   - Select **"Materials"** from the dropdown

2. **Create a New Material**
   - Click **"Create New Material"** button
   - Fill in the form:
     - **Name**: Material name (e.g., `Screw M4x20`, `Wire AWG 18`, `PCB Component`)
     - **Unit**: Measurement unit (optional)
       - Examples: `pcs` (pieces), `kg` (kilograms), `m` (meters), `ud` (units)
       - Default: `ud` (units)
   - Click **"Create"** button

3. **Verify Creation**
   - The material appears in the materials list
   - You can see:
     - Material name
     - Unit
     - Actions (Edit, Delete)

#### Editing Tools and Materials

**To Edit a Tool**:
1. Go to Tools list
2. Click **"Edit"** next to the tool
3. Modify the name
4. Click **"Save"**

**To Edit a Material**:
1. Go to Materials list
2. Click **"Edit"** next to the material
3. Modify name and/or unit
4. Click **"Save"**

#### Deleting Tools and Materials

**To Delete**:
1. Go to Tools or Materials list
2. Click **"Delete"** next to the item
3. Confirm deletion
4. ⚠️ **Warning**: Ensure the tool/material is not referenced in any templates

**Best Practice**: 
- Use consistent naming conventions
- Include specifications in material names (e.g., `Screw M4x20 Stainless Steel`)
- Link materials to inventory items if you use the inventory module

---

### 5.4 Step 3: Create Process Templates

**Purpose**: Define the manufacturing process (phases and steps) for a specific model+variant combination

**Access**: Admin, Supervisor (view only for Operator)

#### Understanding Templates

A **Template** defines:
- Which model and variant it applies to
- The phases of manufacturing (e.g., Preparation, Assembly, Testing)
- The steps within each phase
- Instructions for each step
- Whether steps can be skipped
- Required tools and materials

#### Creating a Template

1. **Navigate to Templates**
   - Click **"Templates"** in the top menu
   - You'll see a list of existing templates

2. **Create a New Template**
   - Click **"Create New Template"** button
   - Fill in the form:
     - **Machine Model**: Select from dropdown (models you created)
     - **Machine Variant**: Automatically loads variants for selected model
       - Select the variant this template applies to
   - Click **"Create"** button

3. **Template Editor Opens**
   - You'll be redirected to the template editor page
   - You'll see:
     - Template information at the top
     - Sections for adding phases
     - Existing phases (if any)

#### Adding Phases

**Phases** are major stages of the manufacturing process (e.g., Preparation, Assembly, Quality Control, Testing).

1. **Find "Add New Phase" Section**
   - Scroll to the phase management section

2. **Enter Phase Information**
   - **Name**: Phase name (e.g., `Preparation`, `Assembly`, `Wiring`, `Testing`, `Packaging`)
   - **Sort Order**: Number indicating sequence (1, 2, 3, ...)
     - Lower numbers appear first
     - Use increments of 10 (10, 20, 30) to allow inserting phases later

3. **Click "Add Phase"**
   - The phase appears in the phases list
   - You can see:
     - Phase name
     - Sort order
     - Actions (Edit, Delete)

4. **Repeat for All Phases**
   - Add all phases in the correct order
   - Example sequence:
     - Phase 1: Preparation (Sort Order: 10)
     - Phase 2: Assembly (Sort Order: 20)
     - Phase 3: Wiring (Sort Order: 30)
     - Phase 4: Testing (Sort Order: 40)
     - Phase 5: Packaging (Sort Order: 50)

#### Adding Steps to Phases

**Steps** are individual tasks within a phase.

1. **Find the Phase**
   - Locate the phase you want to add steps to
   - Each phase has its own **"Add New Step"** section

2. **Enter Step Information**
   - **Title**: Step name (e.g., `Check materials`, `Install main board`, `Connect power supply`)
   - **Instructions**: Detailed description of what to do
     - Be specific and clear
     - Include safety warnings if applicable
     - Example: `Install the main PCB board into the chassis. Ensure all mounting screws are tightened to 2.5 Nm torque.`
   - **Sort Order**: Number within the phase (1, 2, 3, ...)
     - Use increments of 10 for flexibility
   - **Allow Skip**: ☑ Check this box if the step can be skipped
     - If checked, operators can skip this step during execution
     - If unchecked, the step must be completed

3. **Click "Add Step"**
   - The step appears under that phase
   - You can see:
     - Step title
     - Instructions preview
     - Sort order
     - Skip allowed indicator
     - Actions (Edit, Delete)

4. **Repeat for All Steps**
   - Add steps to each phase
   - Ensure sort orders are sequential

#### Example Template Structure

```
Template: S600 - UHF Variant

Phase 1: Preparation (Sort: 10)
  Step 1: Verify materials received (Sort: 10, Skip: No)
  Step 2: Check tools availability (Sort: 20, Skip: No)
  Step 3: Prepare workstation (Sort: 30, Skip: Yes)

Phase 2: Assembly (Sort: 20)
  Step 1: Install chassis base (Sort: 10, Skip: No)
  Step 2: Mount main PCB (Sort: 20, Skip: No)
  Step 3: Install power supply (Sort: 30, Skip: No)
  Step 4: Connect internal wiring (Sort: 40, Skip: No)

Phase 3: Testing (Sort: 30)
  Step 1: Power-on test (Sort: 10, Skip: No)
  Step 2: Functional test (Sort: 20, Skip: No)
  Step 3: Calibration (Sort: 30, Skip: Yes)
```

#### Editing Templates, Phases, and Steps

**To Edit a Template**:
- Currently, you can only change the model/variant by deleting and recreating
- To modify phases/steps, use the Edit buttons

**To Edit a Phase**:
1. Find the phase in the template editor
2. Click **"Edit"** next to the phase
3. Modify name and/or sort order
4. Click **"Save"**

**To Edit a Step**:
1. Find the step in the template editor
2. Click **"Edit"** next to the step
3. Modify title, instructions, sort order, or skip setting
4. Click **"Save"**

#### Deleting Templates, Phases, and Steps

**To Delete a Step**:
1. Go to template editor
2. Find the step
3. Click **"Delete"** next to the step
4. Confirm deletion

**To Delete a Phase**:
1. Go to template editor
2. Find the phase
3. Click **"Delete"** next to the phase
4. ⚠️ **Warning**: This will delete ALL steps in that phase
5. Confirm deletion

**To Delete a Template**:
1. Go to Templates list
2. Click **"Delete"** next to the template
3. ⚠️ **Warning**: This will delete the template but NOT affect existing orders
4. Confirm deletion

**Best Practices**:
- Create templates before creating orders
- Use clear, descriptive step titles
- Write detailed instructions
- Use consistent sort order increments (10, 20, 30)
- Test templates with a sample order before production use
- Document any special requirements in step instructions

---

### 5.5 Step 4: Create Build Orders

**Purpose**: Create an order for manufacturing a specific unit with a serial number

**Access**: Admin, Supervisor (view only for Operator)

#### Understanding Orders

An **Order** represents:
- A specific unit to be manufactured
- A unique serial number
- Reference to a model and variant
- An external reference number (optional)
- Status tracking (Pending, InProgress, Completed, Cancelled)

#### Creating an Order

1. **Navigate to Orders**
   - Click **"Orders"** in the top menu
   - You'll see a list of existing orders

2. **Create a New Order**
   - Click **"Create New Order"** button
   - Fill in the form:

3. **Enter Order Information**
   - **External Reference**: Your reference number (optional but recommended)
     - Examples: `ORD-2026-001`, `PO-12345`, `CUST-ABC-001`
     - Use your company's numbering system
   - **Serial Number**: Unique serial number for this unit (required)
     - Examples: `SN-2026-001`, `S600-UHF-001`, `2026-001`
     - **Important**: Serial numbers must be unique
   - **Machine Model**: Select from dropdown
   - **Machine Variant**: Automatically loads when model is selected
     - Select the appropriate variant

4. **Click "Create"**
   - The order is created with status "Pending"
   - You'll be redirected to the orders list

5. **Verify Order Creation**
   - The new order appears in the list
   - You can see:
     - External reference
     - Serial number
     - Model and variant
     - Status (Pending)
     - Actions (Details, Execute, Summary)

#### Order Statuses

- **Pending**: Order created but manufacturing not started
- **InProgress**: Manufacturing has started
- **Completed**: All steps completed
- **Cancelled**: Order cancelled (cannot be executed)

#### Viewing Order Details

1. **Go to Orders List**
2. **Click "Details"** next to an order
3. **You'll see**:
   - Order information
   - Execution history (if any)
   - Status of each execution

**Best Practices**:
- Use consistent serial number formats
- Include date/year in serial numbers for tracking
- Link external references to your ERP/order system
- Create orders in advance for production planning

---

### 5.6 Step 5: Execute Manufacturing

**Purpose**: Actually perform the manufacturing process and track time spent on each step

**Access**: Admin, Supervisor, Operator

#### Overview of Execution

During execution, you will:
- Start and stop steps (timers track duration)
- Select workstations
- Skip steps (if allowed)
- Add evidence (photos, notes)
- Rework steps if needed
- Add new steps on the fly

#### Starting Manufacturing

1. **Navigate to Orders**
   - Click **"Orders"** in the top menu

2. **Find Your Order**
   - Locate the order with status "Pending"
   - You can filter/search if needed

3. **Start Execution**
   - Click **"Execute"** button next to the order
   - You'll be taken to the execution page

4. **Review Order Information**
   - At the top, you'll see:
     - Order details (serial number, model, variant)
     - External reference
     - Current status

5. **Start Manufacturing**
   - Click **"Start Manufacturing"** button
   - ⚠️ **Important**: This creates a snapshot of the template
   - Status changes to "InProgress"
   - All phases and steps are now visible

#### Working with Steps

##### Starting a Step

1. **Find the Step**
   - Scroll through phases to find the step you want to work on
   - Steps are organized by phase

2. **Click "Start" Button**
   - A popup/modal will appear

3. **Select Workstation (Optional)**
   - Choose a workstation from the dropdown
   - If no workstations are configured, this field may be empty
   - You can proceed without selecting a workstation

4. **Click "Start" in the Popup**
   - The step status changes to "In Progress"
   - A timer starts automatically
   - The step card shows:
     - Status: "In Progress"
     - Elapsed time (updates in real-time)
     - "Stop" button becomes available

5. **Work on the Step**
   - Follow the instructions shown on the step card
   - The timer continues running

##### Stopping a Step

1. **When Finished**
   - Complete the work described in the step instructions

2. **Click "Stop" Button**
   - The timer stops
   - Time is recorded automatically
   - Status changes to "Done"
   - The step card shows:
     - Status: "Done"
     - Total time recorded
     - "Rework" button becomes available (if needed)

3. **Move to Next Step**
   - Proceed to the next step in sequence
   - Steps can be done in any order, but sort order suggests sequence

##### Skipping a Step

**Note**: You can only skip steps that have "Allow Skip" enabled in the template.

1. **Find a Skippable Step**
   - Steps that can be skipped show a "Skip" button
   - If "Skip" button is not visible, the step cannot be skipped

2. **Click "Skip" Button**
   - A popup/modal appears asking for a reason

3. **Enter Skip Reason**
   - **Reason**: Type why you're skipping this step (required)
     - Examples: `Not applicable for this variant`, `Component not available`, `Customer request`
   - Be specific and clear

4. **Click "Skip Step"**
   - Status changes to "Skipped"
   - The reason is recorded
   - The step card shows:
     - Status: "Skipped"
     - Skip reason

##### Reworking a Step (Doing It Again)

**Purpose**: If a step needs to be redone (quality issue, mistake, etc.)

1. **Find a Completed Step**
   - Locate a step with status "Done"
   - The step shows a "Rework" button

2. **Click "Rework" Button**
   - A popup/modal appears

3. **Select Workstation (Optional)**
   - Choose a workstation if different from original

4. **Click "Start"**
   - A new run starts
   - The original run is preserved
   - Status changes to "In Progress"
   - Timer starts

5. **Complete the Rework**
   - Work on the step again
   - Click "Stop" when finished
   - Both runs are tracked separately
   - You can see:
     - Number of runs
     - Time for each run
     - Total time across all runs

**Use Cases for Rework**:
- Quality check failed
- Component installed incorrectly
- Additional testing needed
- Customer requested changes

##### Adding Evidence to Steps

**Purpose**: Attach photos or notes to document work performed

1. **Find the Step**
   - Locate any step (in progress, done, or skipped)

2. **Click "Add Evidence" Button**
   - A popup/modal appears

3. **Upload Image (Optional)**
   - Click "Choose File" or drag and drop
   - Select an image file (JPG, PNG, etc.)
   - **Tips**:
     - Use clear, well-lit photos
     - Show the work performed
     - Include serial numbers or labels if visible

4. **Add Note (Optional)**
   - Type a note describing what's shown or any observations
   - Examples: `Installed correctly`, `Torque verified`, `No issues found`

5. **Click "Upload"**
   - Evidence is saved
   - A thumbnail appears on the step card
   - You can add multiple pieces of evidence

6. **View Evidence**
   - Click the thumbnail to view full-size image
   - Notes are displayed below images

**Best Practices for Evidence**:
- Take photos before and after critical steps
- Document any issues or deviations
- Include measurements or test results in notes
- Use evidence for quality audits

##### Adding New Steps During Execution

**Purpose**: Add steps that weren't in the original template (unplanned work, customizations)

1. **Find the Phase**
   - Locate the phase where you want to add a step
   - Each phase has an "Add Step" button at the top

2. **Click "Add Step" Button**
   - A form appears

3. **Enter Step Information**
   - **Step Title**: Name of the step (required)
     - Example: `Custom wiring modification`, `Additional quality check`
   - **Instructions**: What to do (optional but recommended)
   - **Allow Skip**: ☑ Check if this step can be skipped

4. **Click "Add Step"**
   - The step appears immediately in that phase
   - It's ready to execute
   - Status: "Pending"

5. **Execute the New Step**
   - Click "Start" and proceed as normal

**Use Cases**:
- Customer requested customizations
- Unplanned repairs
- Additional quality checks
- Process improvements discovered during execution

#### Completing Manufacturing

1. **Complete All Required Steps**
   - Finish all steps that cannot be skipped
   - Optional steps can be skipped or completed

2. **Review Progress**
   - Check that all phases show completed steps
   - Verify evidence is added where needed

3. **Finalize**
   - The order status remains "InProgress" until manually completed
   - Or it may auto-complete when all required steps are done (system dependent)

4. **View Summary**
   - Click "Summary" button to see complete execution report
   - Review times, workstations, and evidence

**Best Practices**:
- Work through steps systematically
- Don't skip steps unless necessary
- Add evidence for quality-critical steps
- Document any deviations or issues
- Use rework when quality standards aren't met

---

### 5.7 Step 6: View Summary and Export

**Purpose**: Review complete execution data and export to CSV for analysis

**Access**: Admin, Supervisor, Operator (view only)

#### Viewing Order Summary

1. **Navigate to Orders**
   - Click **"Orders"** in the top menu

2. **Find Your Order**
   - Locate the completed or in-progress order

3. **Click "Summary" Button**
   - You'll see the execution summary page

#### Summary Information Displayed

The summary shows:

1. **Order Information**
   - External reference
   - Serial number
   - Model and variant
   - Status
   - Creation date
   - Completion date (if completed)

2. **Execution Timeline**
   - Start time
   - End time (if completed)
   - Total duration

3. **Phases Breakdown**
   - Each phase listed
   - Steps within each phase
   - For each step:
     - Step title
     - Status (Done, Skipped, In Progress)
     - Total time (sum of all runs)
     - Number of runs (rework count)
     - Workstation(s) used
     - Skip reason (if skipped)
     - Evidence count

4. **Statistics**
   - Total steps
   - Completed steps
   - Skipped steps
   - Total execution time
   - Average time per step

#### Exporting to CSV

1. **On Summary Page**
   - Click **"Export CSV"** button
   - File downloads automatically
   - File name format: `Order_[SerialNumber]_[Date].csv`

2. **Open CSV File**
   - Open in Excel, Google Sheets, or any spreadsheet program
   - The file contains:
     - Order information
     - Phase and step details
     - Times for each step
     - Run counts
     - Workstation information
     - Skip reasons
     - Timestamps

3. **Use CSV Data**
   - Analyze manufacturing times
   - Identify bottlenecks
   - Track rework frequency
   - Generate reports
   - Share with management

#### CSV File Structure

The CSV typically includes columns such as:
- Order Reference
- Serial Number
- Model
- Variant
- Phase Name
- Step Title
- Status
- Total Time (seconds or formatted)
- Number of Runs
- Workstation
- Skip Reason
- Start Time
- End Time

**Best Practices**:
- Export summaries regularly for analysis
- Compare execution times across orders
- Track rework trends
- Use data for process improvement
- Archive CSV files for historical records

---

## 6. Inventory Module

### 6.1 Overview

The Inventory module allows you to:
- Manage inventory items (SKU, name, family, model/type)
- Define warehouse locations and production stations
- Track stock levels by location
- Record inventory movements
- Perform putaway operations (receiving items into locations)
- Perform pick operations (moving items from warehouse to production)
- Generate inventory reports

### 6.2 Step 1: Manage Inventory Items

**Purpose**: Create and manage the catalog of items in your inventory

**Access**: Admin, Inventory

#### Understanding Items

An **Item** represents:
- A product or component in inventory
- SKU (Stock Keeping Unit) - unique identifier
- Name, family, model/type
- Unit of measurement
- Serialization flag (if item is serialized)
- Optional link to process materials

#### Viewing Items List

1. **Navigate to Items**
   - Click **"Items"** in the top menu (Inventory section)
   - You'll see a list of all items

2. **Search Items**
   - Use the search box at the top
   - Search by SKU or name
   - Click "Search" or press Enter
   - Results filter automatically

#### Creating Items - Quick Add

1. **Click "Quick add" Button**
   - A form appears

2. **Enter Basic Information**
   - **SKU**: Unique stock keeping unit (required)
     - Examples: `MOTOR-001`, `PCB-MAIN-001`, `SCREW-M4-20`
   - **Name**: Item name (required)
     - Examples: `DC Motor 12V`, `Main PCB Board`, `Screw M4x20`
   - **Family**: Category or family (optional)
     - Examples: `Motors`, `Electronics`, `Hardware`
   - **Model/Type**: Model or type (optional)
     - Examples: `D600`, `Standard`, `Premium`
   - **Unit**: Unit of measurement (default: `ud`)
     - Examples: `ud` (units), `pcs` (pieces), `kg`, `m`

3. **Click "Create"**
   - Item is created
   - You're redirected to item details

#### Creating Items - Full Form (With Photo)

1. **Navigate to Items**
2. **Click "Create" or "Create with Photo"** (if available)
3. **Fill in All Fields**:
   - SKU, Name, Family, Model/Type, Unit (as above)
   - **Is Serialized**: ☑ Check if each unit has a unique serial number
   - **Photo**: Upload item photo (optional)
4. **Click "Save"**

#### Item Details Page

When you view an item, you'll see:
- Item information (SKU, name, family, etc.)
- Current stock by location
- Tags associated with the item
- Movement history
- Linked material (if linked to process material)

#### Editing Items

1. **Go to Items List**
2. **Click "View" or item name**
3. **Click "Edit"** (if available)
4. **Modify fields**
5. **Click "Save"**

#### Linking Items to Materials

**Purpose**: Link inventory items to process materials so they can be used in BOM and moved to production

1. **View Item Details**
2. **Find "Link to Material" section** (if available)
3. **Select Material** from dropdown
4. **Save**

**Note**: This feature may require Admin access and proper material setup.

#### Managing Tags

**Tags** are RFID codes or QR codes attached to items or packs.

**To View Tags**:
- Go to Item Details
- See "Tags" section
- View tag codes and types

**To Add Tags**:
- Tags are typically added during putaway operations
- Or through dedicated tag management (if available)

**Best Practices**:
- Use consistent SKU naming conventions
- Include specifications in item names
- Use families for grouping related items
- Link items to materials for production integration
- Keep item information up to date

---

### 6.3 Step 2: Manage Locations

**Purpose**: Define warehouse locations and production stations where items are stored

**Access**: Admin, Inventory

#### Understanding Locations

A **Location** represents:
- A physical place where items are stored
- Warehouse shelves, bins, racks
- Production workstations
- Receiving/shipping areas
- Has a unique code (often QR-scannable)
- Has coordinates (X, Y, Z) for warehouse management
- Has a type (Shelf, Workstation, Receiving, Shipping, etc.)
- Has capacity and blocking status

#### Viewing Locations List

1. **Navigate to Locations**
   - Click **"Locations"** in the top menu (Inventory section)
   - You'll see a list of all locations

2. **Filter by Zone**
   - Use the Zone dropdown
   - Select a zone or "(all)"
   - Click "Filter"
   - Locations filter by zone

#### Creating a Single Location

1. **Click "New" Button**
   - A form appears

2. **Enter Location Information**
   - **Code**: Unique location code (required)
     - Examples: `A-01-01`, `WS-01`, `REC-001`
     - **Important**: Use consistent format
     - Often matches QR code on physical location
   - **Zone**: Zone name (optional)
     - Examples: `A`, `B`, `WAREHOUSE`, `PRODUCTION`
   - **X**: X coordinate (optional, default: 0)
   - **Y**: Y coordinate (optional, default: 0)
   - **Z**: Z coordinate (optional, for multi-level storage)
   - **Type**: Location type (required)
     - Options: `Shelf`, `Workstation`, `Receiving`, `Shipping`, `Other`
     - **Important**: Use `Workstation` for production stations
   - **Capacity Units**: Maximum capacity (optional, default: 100000)
   - **Is Blocked**: ☑ Check if location is blocked (cannot receive items)

3. **Click "Create" or "Save"**
   - Location is created
   - Appears in locations list

#### Bulk Generating Locations

**Purpose**: Create multiple locations at once (e.g., entire warehouse grid)

1. **Click "Bulk generate" Button**
   - A form appears

2. **Enter Generation Parameters**
   - **Zone**: Zone name for all locations
   - **Prefix**: Code prefix (e.g., `A-`)
   - **Start X, End X**: X coordinate range
   - **Start Y, End Y**: Y coordinate range
   - **Type**: Location type for all
   - **Capacity**: Capacity for all locations

3. **Click "Generate"**
   - Multiple locations are created
   - Codes follow pattern: `[Prefix][X]-[Y]`
   - Example: `A-01-01`, `A-01-02`, `A-02-01`, etc.

#### Location Details

When you view a location, you'll see:
- Location information (code, zone, coordinates, type)
- Current stock (items and quantities)
- Capacity usage
- Blocking status
- QR code (for printing)

#### Printing Location QR Codes

1. **Go to Locations List**
2. **Click "QR" Button** next to a location
3. **QR Code Modal Opens**
   - QR code is displayed
4. **Print the QR Code**
   - Right-click → Print
   - Or use browser print function
5. **Attach QR Code**
   - Print and attach to physical location
   - Use for scanning during putaway/pick operations

#### Editing Locations

1. **Go to Locations List**
2. **Click "Details"** next to a location
3. **Click "Edit"** (if available)
4. **Modify fields**
5. **Click "Save"**

#### Blocking/Unblocking Locations

**Purpose**: Prevent items from being put away to a location (maintenance, full, etc.)

1. **Edit Location**
2. **Check/Uncheck "Is Blocked"**
3. **Save**

**When Blocked**:
- Location cannot receive new items via putaway
- Existing items can still be picked
- Useful for maintenance or when location is full

#### Location Types Explained

- **Shelf**: Standard warehouse storage location
- **Workstation**: Production station (items can be moved here from warehouse)
- **Receiving**: Receiving area for incoming goods
- **Shipping**: Shipping area for outgoing goods
- **Other**: Custom location type

**Best Practices**:
- Use consistent location code formats
- Print and attach QR codes to physical locations
- Use zones to organize large warehouses
- Set appropriate capacity limits
- Block locations when needed (maintenance, full)
- Use Workstation type for production stations

---

### 6.4 Step 3: View Stock Levels

**Purpose**: See current inventory levels by location

**Access**: Admin, Inventory

#### Viewing Stock

1. **Navigate to Stock**
   - Click **"Stock"** in the top menu (Inventory section)
   - You'll see current stock levels

#### Stock Display

The stock view shows:
- Item SKU and name
- Location code
- Current quantity
- Last updated timestamp
- Links to item and location details

#### Filtering Stock

- Filter by item (if search available)
- Filter by location
- Filter by zone
- View low stock items
- View zero stock items

#### Understanding Stock Snapshots

- Stock levels are updated automatically when movements occur
- Each item-location combination has a stock snapshot
- Stock is reduced on "from" location during movements
- Stock is increased on "to" location during movements

**Best Practices**:
- Regularly review stock levels
- Set up alerts for low stock (if available)
- Verify physical stock matches system stock
- Investigate discrepancies immediately

---

### 6.5 Step 4: Perform Putaway Operations

**Purpose**: Receive items into warehouse locations (put items away)

**Access**: Admin, Inventory

#### Understanding Putaway

**Putaway** is the process of:
- Receiving items into the warehouse
- Assigning items to specific locations
- Recording the movement
- Updating stock levels

#### Putaway Workflow

**Recommended Order**: TAG → LOCATION

1. **Navigate to Operations**
   - Click **"Operations"** in the top menu (Inventory section)

2. **Click "Putaway"**
   - Putaway form appears

3. **Scan or Enter TAG**
   - **TAG**: RFID/QR code of item or box
     - Scan with barcode scanner
     - Or type manually
   - **Note**: If tag doesn't exist, you may need to create item first

4. **Scan or Enter LOCATION**
   - **LOCATION**: Shelf or workstation QR code
     - Scan location QR code
     - Or type location code manually
   - **Note**: Location must exist (create it first if needed)

5. **Enter Quantity**
   - **Quantity**: Number of units (default: 1)
   - **Note**: 
     - For serialized items: Always use 1
     - For packs: Leave at 1 if tag represents a pack (pack quantity applied automatically)
     - For loose items: Enter actual quantity

6. **Review Information**
   - System may display:
     - Item SKU and name
     - Item ID
     - Whether item is serialized
     - Default pack quantity

7. **Click "Save"**
   - Movement is recorded
   - Stock is updated
   - Success message appears

#### Putaway Scenarios

**Scenario 1: Receiving New Items**
1. Items arrive at receiving dock
2. Scan item tag (or create item if new)
3. Scan receiving location
4. Enter quantity
5. Save
6. Later, move from receiving to shelf (another putaway or movement)

**Scenario 2: Moving to Production Station**
1. Items are in warehouse shelf
2. Scan item tag
3. Scan workstation location (type: Workstation)
4. Enter quantity
5. Save
6. Items are now at production station

**Scenario 3: Pack Putaway**
1. Box of items arrives
2. Box has a tag representing the pack
3. Scan box tag
4. Scan shelf location
5. Quantity = 1 (pack quantity is applied automatically)
6. Save

#### Common Putaway Errors

**Error: "Tag not found"**
- **Solution**: Create the item first, or attach tag to item

**Error: "Location not found"**
- **Solution**: Create the location first

**Error: "Location is blocked"**
- **Solution**: Unblock the location, or use a different location

**Error: "Insufficient stock"** (if moving from another location)
- **Solution**: Verify source location has enough stock

**Best Practices**:
- Always scan tags and locations (don't type if possible)
- Verify item information before saving
- Use receiving locations for incoming goods
- Move to final locations after verification
- Document any discrepancies

---

### 6.6 Step 5: Perform Pick Operations

**Purpose**: Move items from warehouse locations to production or other destinations

**Access**: Admin, Inventory

#### Understanding Pick

**Pick** is the process of:
- Taking items from a warehouse location
- Moving them to production, workshop, or specific order
- Recording the movement
- Updating stock levels

#### Pick Workflow

**Recommended Order**: LOCATION → TAG

1. **Navigate to Operations**
   - Click **"Operations"** in the top menu

2. **Click "Pick"**
   - Pick form appears

3. **Scan or Enter LOCATION**
   - **LOCATION**: Shelf QR code where item is located
     - Scan location QR code
     - Or type location code manually

4. **Scan or Enter TAG**
   - **TAG**: RFID/QR code of item or box to pick
     - Scan item tag
     - Or type tag code manually

5. **Enter Quantity**
   - **Quantity**: Number of units to pick (default: 1)
   - **Note**: 
     - For packs: Leave at 1 if tag represents a pack
     - Enter actual quantity for loose items

6. **Select Destination**
   - **Destination**: Where items are going
     - Options:
       - **Production**: General production area
       - **Workshop**: Workshop area
       - **Station**: Specific workstation (enter reference)
       - **Order**: Specific order (enter reference)
       - **Other**: Custom destination (enter code)

7. **Enter Destination Reference** (if Station/Order/Other)
   - **Destination Reference**: 
     - For Station: e.g., `WS-01`
     - For Order: e.g., `2026-0012`
     - For Other: Custom code
   - **Note**: System creates destination location automatically if needed

8. **Click "Pick" Button**
   - Movement is recorded
   - Stock is reduced at source location
   - Stock is increased at destination (or destination location created)
   - Success message appears

#### Pick Scenarios

**Scenario 1: Pick for Production Order**
1. Order requires specific materials
2. Go to Pick operation
3. Scan location where material is stored
4. Scan material tag
5. Enter quantity
6. Select "Order" destination
7. Enter order reference (e.g., serial number)
8. Pick
9. Material is now allocated to that order

**Scenario 2: Pick to Workstation**
1. Production station needs materials
2. Scan source location
3. Scan item tag
4. Enter quantity
5. Select "Station" destination
6. Enter workstation code (e.g., `WS-01`)
7. Pick
8. Material is now at workstation

**Scenario 3: Pick Pack to Workshop**
1. Workshop needs a box of items
2. Scan shelf location
3. Scan box tag
4. Quantity = 1 (pack)
5. Select "Workshop" destination
6. Pick
7. Box is moved to workshop

#### Destination Types Explained

- **Production (PROD)**: General production area
  - Creates/uses location: `DEST:PROD`
- **Workshop (TALLER)**: Workshop area
  - Creates/uses location: `DEST:TALLER`
- **Station (WS)**: Specific workstation
  - Creates/uses location: `DEST:WS:[REF]`
  - Example: `DEST:WS:WS-01`
- **Order (ORDER)**: Specific order
  - Creates/uses location: `DEST:PEDIDO:[REF]`
  - Example: `DEST:PEDIDO:2026-0012`
- **Other (CUSTOM)**: Custom destination
  - Creates/uses location with your code

#### Common Pick Errors

**Error: "Item not found at location"**
- **Solution**: Verify item is actually at that location, check stock levels

**Error: "Insufficient stock"**
- **Solution**: Verify available quantity, pick less, or check another location

**Error: "Location not found"**
- **Solution**: Verify location code is correct

**Best Practices**:
- Verify location before picking
- Check stock levels before picking
- Use order references for traceability
- Document picks for specific orders
- Verify picks match production needs

---

### 6.7 Step 6: View Movements History

**Purpose**: See complete history of all inventory movements

**Access**: Admin, Inventory

#### Viewing Movements

1. **Navigate to Movements**
   - Click **"Movements"** in the top menu (Inventory section)
   - You'll see movement history

#### Movements Display

The movements view shows:
- Movement type (Putaway, Pick, Transfer, etc.)
- Item SKU and name
- From location
- To location
- Quantity
- Performed by (user)
- Timestamp
- Notes (if any)

#### Filtering Movements

- Filter by date range
- Filter by item
- Filter by location
- Filter by movement type
- Filter by user

#### Understanding Movement Types

- **Putaway**: Items received into location
- **Pick**: Items moved from warehouse to production/destination
- **Transfer**: Items moved between locations (if available)
- **Adjustment**: Manual stock adjustments (if available)

**Best Practices**:
- Review movements regularly
- Investigate unusual movements
- Use movements for audit trails
- Export movements for reporting

---

### 6.8 Step 7: Generate Inventory Reports

**Purpose**: View inventory reports and analytics

**Access**: Admin, Inventory

#### Available Reports

1. **Navigate to Reports**
   - Click **"Reports"** in the top menu (Inventory section)
   - Or go to **Inventory → Reports**

#### Report Types

**Report by Item**:
- Shows stock levels for each item across all locations
- Total quantity per item
- Location breakdown

**Report by Model**:
- Groups items by model/type
- Shows quantities by model
- Useful for product line analysis

**Report by Warehouse**:
- Shows stock by location/warehouse
- Capacity usage
- Location details

**Stock Report**:
- Current stock levels
- Low stock alerts
- Zero stock items

#### Using Reports

1. **Select Report Type**
2. **Apply Filters** (if available):
   - Date range
   - Item family
   - Location zone
   - Model/type
3. **View Results**
4. **Export** (if available):
   - Export to CSV
   - Export to PDF (if available)
   - Print report

**Best Practices**:
- Run reports regularly
- Use reports for inventory planning
- Identify slow-moving items
- Plan reorder points
- Share reports with management

---

## 7. Integration Workflows

### 7.1 Overview

The system allows integration between Time Tracking and Inventory modules through shared items.

### 7.2 Linking Inventory Items to Process Materials

**Purpose**: Use the same product in both warehouse (inventory) and production (materials)

1. **Create Material** (Time Tracking module)
   - Go to Resources → Materials
   - Create material with name and unit

2. **Create Item** (Inventory module)
   - Go to Items
   - Create item with same name/specifications

3. **Link Item to Material**
   - Edit item
   - Select material from dropdown
   - Save

### 7.3 Moving Items from Warehouse to Production

**Workflow**:

1. **Items in Warehouse**
   - Items are stored in warehouse locations (type: Shelf)

2. **Create Production Order**
   - Go to Orders (Time Tracking)
   - Create order for specific unit

3. **Pick Items for Order**
   - Go to Operations → Pick (Inventory)
   - Pick items from warehouse
   - Select "Order" destination
   - Enter order serial number/reference
   - Items are moved to order-specific location

4. **Items Available at Production**
   - Items are now at `DEST:PEDIDO:[ORDER_REF]` location
   - Production can access items

5. **Execute Manufacturing**
   - Go to Orders → Execute
   - Start manufacturing process
   - Materials are available (linked items)

### 7.4 Moving Items to Workstations

**Workflow**:

1. **Create Workstation Location**
   - Go to Locations (Inventory)
   - Create location with type "Workstation"
   - Code: e.g., `WS-01`

2. **Pick Items to Workstation**
   - Go to Operations → Pick
   - Pick from warehouse location
   - Select "Station" destination
   - Enter workstation code
   - Items moved to workstation

3. **Use at Workstation**
   - Items are now at `DEST:WS:WS-01`
   - Production operators can use items

### 7.5 Best Practices for Integration

- Link materials and items consistently
- Use order references for traceability
- Move items before starting production
- Verify items are at correct locations
- Document movements for audit

---

## 8. Reports and Analytics

### 8.1 Time Tracking Reports

#### Execution Summary
- View per order
- Shows all phases and steps
- Times and workstations
- Export to CSV

#### Order History
- All orders for a model/variant
- Execution times
- Completion rates
- Rework frequency

### 8.2 Inventory Reports

#### Stock Reports
- Current stock levels
- By item, location, or model
- Low stock alerts

#### Movement Reports
- All movements
- By date, item, location
- User activity

### 8.3 Exporting Data

#### CSV Export
- Execution data
- Stock levels
- Movements
- Open in Excel for analysis

#### Best Practices
- Export regularly
- Archive historical data
- Use for KPI tracking
- Share with stakeholders

---

## 9. Troubleshooting

### 9.1 Common Issues

#### Can't Login
**Symptoms**: Login fails, wrong password error

**Solutions**:
1. Verify email and password are correct
2. Check for typos (case-sensitive)
3. Ensure database was created (first run takes longer)
4. Try default admin account: `admin@test.com` / `Admin123!`
5. Clear browser cache and cookies
6. Try incognito/private browsing mode

#### Database Errors
**Symptoms**: "Invalid object name 'Locations'", "Table doesn't exist"

**Solutions**:
1. Wait for application to fully start (first run creates database)
2. Refresh the page after 30 seconds
3. Run migrations manually:
   ```bash
   dotnet ef database update
   ```
4. Check SQL Server LocalDB is running
5. Verify connection string in `appsettings.Development.json`

#### Application Won't Start
**Symptoms**: Error on startup, port already in use

**Solutions**:
1. Stop Visual Studio debugger
2. Close any running instances
3. Check if port is in use:
   ```bash
   netstat -ano | findstr :5173
   ```
4. Change ports in `appsettings.Development.json`:
   ```json
   "Kestrel": {
     "HttpPort": 5174,
     "HttpsPort": 7246
   }
   ```
5. Rebuild solution: Build → Rebuild Solution

#### Can't Add Variant
**Symptoms**: Variant not saving, validation errors

**Solutions**:
1. Ensure Name and Code fields are filled
2. Verify Code is unique (not already used)
3. Check you're on the Model Details page
4. Refresh page and try again
5. Check browser console for JavaScript errors

#### Can't Delete Item
**Symptoms**: Delete button doesn't work, error on delete

**Solutions**:
1. Verify item isn't used in any templates or orders
2. Check you clicked Delete and confirmed
3. Ensure you have proper permissions (Admin)
4. Check for related records (tags, movements, stock)

#### Stock Not Updating
**Symptoms**: Stock levels don't change after movement

**Solutions**:
1. Verify movement was saved successfully
2. Refresh stock page
3. Check movement was recorded in Movements list
4. Verify source and destination locations are correct
5. Check for errors in browser console

#### Putaway/Pick Not Working
**Symptoms**: Error saving putaway/pick operation

**Solutions**:
1. Verify tag exists (create item/tag first if needed)
2. Verify location exists (create location first if needed)
3. Check location is not blocked
4. Verify sufficient stock (for picks)
5. Check all required fields are filled
6. Review error message for specific issue

#### Steps Not Starting/Stopping
**Symptoms**: Timer doesn't start, step status doesn't change

**Solutions**:
1. Refresh the page
2. Check you're logged in
3. Verify order status is "InProgress"
4. Check browser console for errors
5. Try different browser
6. Clear browser cache

#### Export Not Working
**Symptoms**: CSV doesn't download, export button doesn't work

**Solutions**:
1. Check browser popup blocker settings
2. Allow downloads for the site
3. Try different browser
4. Check browser console for errors
5. Verify order has execution data

### 9.2 Getting Help

If you encounter issues:
1. **Check this guide** - Review relevant sections
2. **Review error messages** - Read error details carefully
3. **Check troubleshooting section** - Look for similar issues
4. **Contact support** - Provide:
   - Error message
   - Steps to reproduce
   - User role
   - Browser and version
   - Screenshots if possible

---

## 10. Best Practices

### 10.1 Time Tracking Best Practices

#### Template Creation
- Create templates before orders
- Use clear, descriptive step titles
- Write detailed instructions
- Set appropriate skip settings
- Test templates with sample orders
- Document special requirements

#### Order Management
- Use consistent serial number formats
- Include date/year in serial numbers
- Link to external order systems
- Create orders in advance
- Review orders before execution

#### Execution
- Work through steps systematically
- Don't skip steps unless necessary
- Add evidence for quality steps
- Document deviations
- Use rework for quality issues
- Select appropriate workstations

#### Data Management
- Export summaries regularly
- Archive historical data
- Review execution times
- Track rework trends
- Use data for improvements

### 10.2 Inventory Best Practices

#### Item Management
- Use consistent SKU formats
- Include specifications in names
- Use families for grouping
- Link items to materials
- Keep information updated
- Document item details

#### Location Management
- Use consistent location codes
- Print and attach QR codes
- Use zones for organization
- Set appropriate capacity
- Block locations when needed
- Keep location map updated

#### Operations
- Always scan tags/locations
- Verify information before saving
- Use receiving locations first
- Move to final locations after verification
- Document discrepancies
- Verify picks match needs

#### Stock Management
- Review stock regularly
- Set up low stock alerts
- Verify physical vs system stock
- Investigate discrepancies
- Plan reorder points
- Track movement patterns

### 10.3 Integration Best Practices

- Link materials and items consistently
- Use order references for traceability
- Move items before production starts
- Verify items at correct locations
- Document all movements
- Coordinate between modules

### 10.4 General Best Practices

#### User Management
- Use appropriate user roles
- Don't share accounts
- Change default passwords
- Regular access reviews
- Train users properly

#### Data Quality
- Enter complete information
- Use consistent formats
- Verify data accuracy
- Regular data cleanup
- Archive old data

#### Security
- Use strong passwords
- Don't share credentials
- Log out when done
- Report security issues
- Keep software updated

#### Performance
- Regular database maintenance
- Archive old data
- Optimize queries (if admin)
- Monitor system performance
- Plan for growth

---

## 11. Appendix

### 11.1 Glossary

**Admin**: User role with full system access

**Evidence**: Photos or notes attached to execution steps

**Item**: Inventory product with SKU, name, and properties

**Location**: Physical place where items are stored (warehouse shelf, workstation)

**Material**: Consumable item used in manufacturing process

**Model**: Type of machine being manufactured

**Order**: Manufacturing order for a specific unit with serial number

**Phase**: Major stage of manufacturing process (e.g., Assembly, Testing)

**Pick**: Operation to move items from warehouse to production

**Putaway**: Operation to receive items into warehouse locations

**Rework**: Doing a step again (multiple runs tracked separately)

**SKU**: Stock Keeping Unit - unique identifier for inventory items

**Step**: Individual task within a manufacturing phase

**Tag**: RFID or QR code attached to items or packs

**Template**: Manufacturing process definition with phases and steps

**Variant**: Different configuration or version of a model

**Workstation**: Production station where manufacturing occurs

### 11.2 Keyboard Shortcuts

- **Tab**: Move to next field
- **Enter**: Submit form (when in form field)
- **Esc**: Close modal/popup
- **Ctrl+F**: Search page (browser)
- **F5**: Refresh page

### 11.3 File Formats

**Supported Image Formats** (for evidence):
- JPG/JPEG
- PNG
- GIF
- BMP

**Export Formats**:
- CSV (Comma-Separated Values)
- Can be opened in Excel, Google Sheets, etc.

### 11.4 System Limits

- **Item SKU**: Max 64 characters
- **Item Name**: Max 256 characters
- **Location Code**: Max length varies
- **Step Instructions**: Max length varies
- **File Upload Size**: Check system configuration

### 11.5 Contact Information

For technical support or questions:
- Check this guide first
- Review error messages
- Contact your system administrator
- Provide detailed error information

### 11.6 Version History

- **Version 1.0** (February 2026): Initial comprehensive guide

### 11.7 Additional Resources

- README.md: Quick start guide
- System documentation (if available)
- Training materials (if available)
- Video tutorials (if available)

---

## Document Information

**Title**: Manufacturing Time Tracking & Inventory System - Complete User Guide  
**Version**: 1.0  
**Last Updated**: February 2026  
**Pages**: Comprehensive guide covering all features  
**Format**: Markdown (can be converted to PDF)

---

**End of User Guide**

For questions or feedback, please contact your system administrator.
