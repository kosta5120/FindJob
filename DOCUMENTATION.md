# FindJob — Project Documentation

## Table of Contents
1. [Project Overview](#1-project-overview)
2. [Technology Stack](#2-technology-stack)
3. [Project Structure](#3-project-structure)
4. [Database Schema](#4-database-schema)
5. [Application Flow](#5-application-flow)
6. [Pages & Views](#6-pages--views)
7. [Controllers & Business Logic](#7-controllers--business-logic)
8. [API Endpoints (AJAX)](#8-api-endpoints-ajax)
9. [Email Service](#9-email-service)
10. [Authentication](#10-authentication)
11. [Configuration](#11-configuration)
12. [Security Issues](#12-security-issues)
13. [Migration Plan: React + ASP.NET API](#13-migration-plan-react--aspnet-api)

---

## 1. Project Overview

**FindJob** is a Hebrew-language job board platform for the Israeli market. It allows:
- **Employers** to post job openings with full details
- **Job Seekers** to browse, search, filter, and apply for jobs via email (CV submission)
- **Administrators** to monitor activity, view statistics, and moderate reported posts

Posts automatically expire after **14 days** (cleaned on each homepage visit).

---

## 2. Technology Stack

| Layer         | Technology                          |
|---------------|-------------------------------------|
| Backend       | ASP.NET MVC 5.2.4 (.NET Framework 4.6.1) |
| ORM           | Entity Framework 6.2.0              |
| Database      | SQL Server (MSSQL)                  |
| Frontend      | HTML5 + Razor Templates             |
| CSS Framework | Bootstrap 4.1.3                     |
| JavaScript    | jQuery 3.3.1                        |
| Dropdowns     | Select2.js 4.0.5                    |
| Pagination    | PagedList.Mvc 4.5.0.0               |
| JSON          | Newtonsoft.Json 11.0.1              |
| Charts        | System.Web.Helpers (server-side)    |

---

## 3. Project Structure

```
FindJob/
├── FJweb/                          # Main ASP.NET MVC application
│   ├── Controllers/
│   │   ├── HomeController.cs       # All public-facing pages & logic
│   │   └── AdminPageController.cs  # Admin dashboard & moderation
│   ├── Models/                     # Entity Framework auto-generated models
│   │   ├── FJDB.cs                 # Main DbContext (FindJobEntities1)
│   │   ├── FJ_JobsPosts.cs         # Job posting entity
│   │   ├── FJ_Administrators.cs    # Admin user entity
│   │   ├── FJ_Categorys_Of_Works.cs
│   │   ├── FJ_SubCategory_Work.cs
│   │   ├── FJ_Region.cs
│   │   ├── FJ_Citys.cs
│   │   ├── FJ_JobType.cs
│   │   ├── FJ_BadReports.cs
│   │   ├── FJ_Attachment.cs        # (unused)
│   │   ├── FJ_Sing_up.cs           # (unused)
│   │   ├── PostDetails.cs          # ViewModel for job posting form
│   │   ├── SearchFromHomePage.cs   # Search form model
│   │   ├── MailModel.cs            # Email message model
│   │   └── Paging.cs               # Custom pagination helper
│   ├── ModelView/
│   │   ├── VieModelPostJobs.cs     # Combined PostDetails + MailModel
│   │   └── CategoryAndSubcategoryViewModel.cs
│   ├── Views/
│   │   ├── Home/
│   │   │   ├── HomePage.cshtml     # Landing page
│   │   │   ├── AdvertisingPage.cshtml  # Post a job form
│   │   │   ├── AllJobs.cshtml      # Search results + filters
│   │   │   └── ModalPopup.cshtml   # Filter details partial view
│   │   ├── AdminPage/
│   │   │   ├── AdminHomePage.cshtml  # Admin dashboard (charts)
│   │   │   ├── BadPost.cshtml        # Reported posts list
│   │   │   └── ViewPostPopup.cshtml  # Post preview partial view
│   │   └── Shared/
│   │       ├── _Layout.cshtml      # Public layout (navbar + admin login modal)
│   │       ├── _AdminLayout.cshtml # Admin layout
│   │       └── Error.cshtml
│   ├── App_Start/
│   │   ├── RouteConfig.cs          # URL routing rules
│   │   ├── BundleConfig.cs         # JS/CSS bundling
│   │   └── FilterConfig.cs         # Global MVC filters
│   ├── Content/                    # CSS stylesheets
│   ├── Scripts/                    # jQuery, Bootstrap, Select2 JS
│   ├── ACC/                        # Images/assets
│   ├── Web.config                  # App configuration (DB connection, etc.)
│   └── Global.asax.cs              # App startup entry point
└── FJweb.sln                       # Visual Studio solution
```

---

## 4. Database Schema

**Connection:** SQL Server on `DESKTOP-F6L4PRL`, database `FindJob`

### Tables

#### `FJ_JobsPosts` — Job Postings
| Column            | Type         | Description                         |
|-------------------|--------------|-------------------------------------|
| ID                | int (PK)     | Auto-increment primary key          |
| Post_Title        | nvarchar     | Job title                           |
| Name_of_Publisher | nvarchar     | Employer/publisher name             |
| Address           | nvarchar     | Job location (city name)            |
| Region            | nvarchar     | Region name                         |
| Salary            | nvarchar     | Salary range / description          |
| Typ_of_job        | nvarchar     | Job type name                       |
| Email             | nvarchar     | Contact email for applications      |
| Description       | nvarchar     | Full job description                |
| Experience        | nvarchar     | Required experience / qualifications|
| CategoryPosition  | nvarchar     | Category name                       |
| Subcategory       | nvarchar     | Subcategory name                    |
| NumberOfPosition  | int          | Number of open positions            |
| dateTime          | datetime     | Post creation timestamp             |

#### `FJ_Administrators` — Admin Users
| Column    | Type     | Description          |
|-----------|----------|----------------------|
| ID        | int (PK) | Auto-increment       |
| FirstName | nvarchar | Admin first name     |
| LastName  | nvarchar | Admin last name      |
| UserName  | nvarchar | Login username       |
| Password  | nvarchar | Plain text password  |

#### `FJ_Categorys_Of_Works` — Job Categories
| Column       | Type     | Description         |
|--------------|----------|---------------------|
| ID           | int (PK) | Auto-increment      |
| CategoryWork | nvarchar | Category name (Hebrew) |

#### `FJ_SubCategory_Work` — Subcategories
| Column         | Type     | Description                    |
|----------------|----------|--------------------------------|
| ID             | int (PK) | Auto-increment                 |
| IDCategoryWork | int (FK) | References FJ_Categorys_Of_Works.ID |
| Subcategory    | nvarchar | Subcategory name (Hebrew)      |

#### `FJ_JobType` — Employment Types
| Column         | Type     | Description                  |
|----------------|----------|------------------------------|
| IDTypePosition | int (PK) | Auto-increment               |
| jobType        | nvarchar | Type name (e.g. Full-time)   |

#### `FJ_Region` — Geographic Regions
| Column   | Type     | Description               |
|----------|----------|---------------------------|
| idRegion | int (PK) | Auto-increment            |
| Region   | nvarchar | Region name in Hebrew     |

Regions: צפון (North), מרכז (Center), דרום (South), ירושלים (Jerusalem), שרון, שפלה, חוץ לארץ (Abroad)

#### `FJ_Citys` — Cities
| Column   | Type     | Description                    |
|----------|----------|--------------------------------|
| idCity   | int (PK) | Auto-increment                 |
| idRegion | int (FK) | References FJ_Region.idRegion  |
| City     | nvarchar | City name in Hebrew            |

#### `FJ_BadReports` — Reported Posts
| Column          | Type     | Description                       |
|-----------------|----------|-----------------------------------|
| ID              | int (PK) | Auto-increment                    |
| IDPost          | int (FK) | References FJ_JobsPosts.ID        |
| NumberOfReports | int      | Number of times post was reported |

#### `FJ_Attachment` — CV Attachments *(unused in app)*
| Column    | Type      | Description               |
|-----------|-----------|---------------------------|
| ID        | int (PK)  | Auto-increment            |
| Name      | nvarchar  | Applicant name            |
| Email     | nvarchar  | Applicant email           |
| CV        | varbinary | CV file bytes             |
| Date_Time | datetime  | Upload timestamp          |

#### `FJ_Sing_up` — User Registrations *(unused in app)*
| Column       | Type     | Description           |
|--------------|----------|-----------------------|
| ID           | int (PK) | Auto-increment        |
| E_Mail       | nvarchar | User email            |
| Password     | nvarchar | Plain text password   |
| Phone_Number | nvarchar | Phone number          |

### Stored Procedures
| Name                        | Returns                                  |
|-----------------------------|------------------------------------------|
| `CountPoistPerMonth`        | Post count grouped by month              |
| `QuantityOfJobsInEachCity`  | Post count grouped by city               |
| `JobsPosts()`               | All job postings                         |
| `AllCategorysWork()`        | All work categories                      |
| `SavePostJob()`             | Saves a new job posting                  |
| `GetSubcategoryWork()`      | All subcategories                        |
| `Attachments()`             | Saves attachment record                  |

---

## 5. Application Flow

### Job Seeker Journey
```
HomePage
  └─ Browse categories / types / regions
  └─ Enter search ("What" + "Where")
       ↓
  AllJobs (search results)
  └─ Sidebar: refine by Category, Subcategory, Type, Region, City
  └─ View job card (title, location, type, salary, description)
  └─ Click "Apply" (הגש מועמדות)
       ↓
  Email sent to employer with cover letter + CV attachment
  └─ Success alert shown
  └─ Can also report inappropriate post → BadReport created
```

### Employer (Job Poster) Journey
```
HomePage
  └─ Click "Post Job" → AdvertisingPage
       ↓
  Fill form:
    - Category → Subcategory auto-loads via AJAX
    - Region → Cities auto-load via AJAX
    - Title, Description, Requirements, Salary, Email, etc.
       ↓
  Submit → post saved to FJ_JobsPosts
  └─ Confirmation modal shown
  └─ Redirected to HomePage
```

### Admin Journey
```
HomePage Navbar
  └─ Click "Admin Login" → Login modal
  └─ Enter username + password
       ↓
  AdminHomePage (dashboard)
  └─ Chart: Jobs posted per month (bar chart)
  └─ Chart: Jobs distributed by city (horizontal bar)
       ↓
  BadPost page
  └─ Table of all reported posts
  └─ Preview post → modal popup (partial view)
  └─ Delete post → removes FJ_JobsPosts + FJ_BadReports entries
  └─ Dismiss report → removes only FJ_BadReports entry
```

### Automatic Housekeeping
- On every `HomePage` load: all `FJ_JobsPosts` older than **14 days** are deleted, along with their associated `FJ_BadReports` entries.

---

## 6. Pages & Views

### Public Pages

| Route                    | View                     | Purpose                                    |
|--------------------------|--------------------------|--------------------------------------------|
| `/` or `/Home/HomePage`  | `Home/HomePage`          | Landing page, category/type/region overview |
| `/Home/AdvertisingPage`  | `Home/AdvertisingPage`   | Post a job form                            |
| `/Home/AllJobs`          | `Home/AllJobs`           | Search results with filter sidebar         |
| `/Home/ModalPopup`       | `Home/ModalPopup`        | Partial view for modal filter dialogs      |

### Admin Pages

| Route                         | View                           | Purpose                                  |
|-------------------------------|--------------------------------|------------------------------------------|
| `/AdminPage/AdminHomePage`    | `AdminPage/AdminHomePage`      | Dashboard with charts                    |
| `/AdminPage/BadPost`          | `AdminPage/BadPost`            | Reported posts management table          |
| `/AdminPage/ViewPostPopup`    | `AdminPage/ViewPostPopup`      | Partial view: post preview modal         |

### Shared Layouts

| File                    | Used By      | Purpose                                     |
|-------------------------|--------------|---------------------------------------------|
| `_Layout.cshtml`        | Public pages | Navbar + admin login modal + footer         |
| `_AdminLayout.cshtml`   | Admin pages  | Admin nav + footer                          |

---

## 7. Controllers & Business Logic

### HomeController

| Action               | HTTP  | Description                                                    |
|----------------------|-------|----------------------------------------------------------------|
| `HomePage()`         | GET   | Load landing page; delete posts >14 days; count per category/type |
| `GetSubcategory(id)` | GET   | JSON — return subcategories for given category ID             |
| `GetCitysByRegion(id)`| GET  | JSON — return cities for given region ID                      |
| `SubmitPost(pd)`     | POST  | Save new job posting to DB, redirect to HomePage              |
| `AllJobs(...)`       | GET   | Complex search/filter with pagination                         |
| `BadPost(id)`        | GET   | Create or increment a FJ_BadReports record for post ID        |
| `SendCV(...)`        | POST  | Send CV email to job poster via Gmail SMTP                    |
| `ConnectToAdmin()`   | POST  | Validate admin credentials, redirect to admin dashboard       |
| `ModalPopup(val)`    | GET   | Return partial view with filtered items for modal             |

#### `AllJobs()` Search Logic
The method accepts multiple parameters (what, where, category, subcategory, type, region, city) and builds a filtered LINQ query. It handles up to 5 combined filter dimensions, computing counts for the sidebar refinement panel. Results are paginated (2 items/page default).

#### `SendCV()` Email Logic
Uses `System.Net.Mail.SmtpClient` with Gmail SMTP (`smtp.gmail.com:587`). Sends the applicant's cover letter and optional CV attachment to the employer's email stored in the job post.

### AdminPageController

| Action                       | HTTP | Description                                             |
|------------------------------|------|---------------------------------------------------------|
| `AdminHomePage()`            | GET  | Load admin dashboard view                               |
| `BadPost()`                  | GET  | Moderate reported posts; handle delete/dismiss actions  |
| `ChartPage()`                | GET  | Render monthly posts bar chart (binary image response)  |
| `QuantityOfJobsInEachCity()` | GET  | Render city jobs bar chart (binary image response)      |
| `ViewPostPopup(id)`          | GET  | Return partial view for post preview modal              |

---

## 8. API Endpoints (AJAX)

These are the server-side endpoints consumed via jQuery AJAX on the frontend:

| Endpoint                        | Method | Parameters          | Returns                      | Used In            |
|---------------------------------|--------|---------------------|------------------------------|--------------------|
| `/Home/GetSubcategory`          | GET    | `ID` (category id)  | JSON array of subcategories  | AdvertisingPage    |
| `/Home/GetCitysByRegion`        | GET    | `idRegion`          | JSON array of cities         | AdvertisingPage    |
| `/AdminPage/ViewPostPopup`      | GET    | `id` (post id)      | HTML partial view            | BadPost admin page |

---

## 9. Email Service

- **Provider:** Gmail SMTP
- **Host:** `smtp.gmail.com`
- **Port:** 587 (TLS)
- **Account:** `find.job.cv.2019@gmail.com`
- **Trigger:** Job seeker clicks "Apply" and submits form
- **Payload:** Cover letter text + optional CV file attachment
- **Recipient:** Email stored in `FJ_JobsPosts.Email`

---

## 10. Authentication

- **Type:** Simple form-based login (modal dialog in navbar)
- **Validation:** Username + password matched against `FJ_Administrators` table
- **Storage:** Plain text passwords in DB
- **Session:** None implemented — no cookie/session token
- **Logout:** Not implemented
- **Authorization:** No `[Authorize]` attribute on admin controllers — pages accessible directly by URL

---

## 11. Configuration

**Web.config key settings:**

```xml
<!-- Database Connection -->
<connectionStrings>
  <add name="FindJobEntities1"
       connectionString="...Data Source=DESKTOP-F6L4PRL;Initial Catalog=FindJob;
                         User ID=qlsa;Password=22732wsxqls$RFV;..."
       providerName="System.Data.EntityClient" />
</connectionStrings>

<!-- URL limits (for long search query strings) -->
<httpRuntime maxQueryStringLength="32768" maxUrlLength="65536" />
```

**BundleConfig.cs bundles:**
- `~/bundles/jquery` — jQuery core
- `~/bundles/jqueryval` — jQuery Validation
- `~/bundles/modernizr` — Modernizr
- `~/bundles/bootstrap` — Bootstrap + Popper.js
- `~/Content/css` — All stylesheets

**RouteConfig.cs:**
```
Default route: {controller}/{action}/{id?}
Default: HomeController → HomePage
```

---

## 12. Security Issues

| Issue                        | Severity | Description                                             |
|------------------------------|----------|---------------------------------------------------------|
| Hardcoded DB password        | High     | Password in Web.config plain text                       |
| Hardcoded Gmail password     | High     | Email credentials hardcoded in HomeController           |
| Plain text admin passwords   | High     | Passwords stored without hashing in DB                  |
| No session management        | High     | Admin pages accessible directly by URL                  |
| No `[Authorize]` attributes  | High     | No server-side authorization enforcement                |
| Potential XSS                | Medium   | User input displayed without consistent encoding        |
| SQL via Contains()           | Low      | LINQ Contains may allow injection in edge cases         |

---

## 13. Migration Plan: React + ASP.NET API

### Architecture Overview

```
┌─────────────────────────────┐     HTTP/REST      ┌──────────────────────────────┐
│   React Frontend (SPA)      │ ◄────────────────► │   ASP.NET Core Web API       │
│   - Vite + React 18         │                    │   - .NET 8                   │
│   - React Router v6         │                    │   - Entity Framework Core     │
│   - Axios for HTTP          │                    │   - JWT Authentication        │
│   - Bootstrap 5 / Tailwind  │                    │   - SQL Server               │
│   - RTL layout (Hebrew)     │                    │                              │
└─────────────────────────────┘                    └──────────────────────────────┘
```

### Frontend Pages (React Components)

| Current View          | React Route              | Component                     |
|-----------------------|--------------------------|-------------------------------|
| HomePage              | `/`                      | `HomePage`                    |
| AdvertisingPage       | `/post-job`              | `PostJobPage`                 |
| AllJobs               | `/jobs`                  | `JobsPage` + `JobCard` + `FilterSidebar` |
| AdminHomePage         | `/admin`                 | `AdminDashboard` + `Charts`   |
| BadPost               | `/admin/reports`         | `ReportsPage`                 |

### ASP.NET API Endpoints to Build

| Method | Endpoint                            | Description                          |
|--------|-------------------------------------|--------------------------------------|
| GET    | `/api/jobs`                         | Get all jobs (with filters + paging) |
| POST   | `/api/jobs`                         | Create a new job posting             |
| DELETE | `/api/jobs/{id}`                    | Delete a job post (admin)            |
| GET    | `/api/categories`                   | Get all categories                   |
| GET    | `/api/categories/{id}/subcategories`| Get subcategories by category        |
| GET    | `/api/regions`                      | Get all regions                      |
| GET    | `/api/regions/{id}/cities`          | Get cities by region                 |
| GET    | `/api/job-types`                    | Get all job types                    |
| POST   | `/api/apply`                        | Submit job application (send email)  |
| POST   | `/api/reports`                      | Report a job post                    |
| DELETE | `/api/reports/{id}`                 | Remove a report (admin)              |
| GET    | `/api/admin/reports`                | Get all bad reports (admin)          |
| GET    | `/api/admin/stats/monthly`          | Posts per month stats                |
| GET    | `/api/admin/stats/cities`           | Posts per city stats                 |
| POST   | `/api/auth/login`                   | Admin login → returns JWT token      |

### Migration Steps

1. **Create ASP.NET Core Web API project** with EF Core, targeting existing SQL Server DB
2. **Scaffold entities** from existing DB schema
3. **Build API controllers** for each endpoint above
4. **Add JWT auth** for admin endpoints
5. **Create React app** using Vite
6. **Implement React Router** with all routes
7. **Build components** for each page (homepage, jobs list, post job, admin)
8. **Connect to API** via Axios with base URL config
9. **Handle RTL** (Hebrew, right-to-left layout) in CSS
10. **Test and verify** feature parity with original
