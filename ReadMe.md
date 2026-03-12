# 📚 LMS Razor – Library Management System (Data Layer)

## Overview

A lightweight **Library Management System (LMS)** built using **ASP.NET Razor Pages** and **SQL Server**.

This project contains the **data access layer** responsible for:

- Managing books
- Handling loan records
- Performing database operations
- Executing SQL queries and stored procedures safely


The **Add Book** page allows administrators or librarians to add new books to the Library Management System.  

Users can enter:

-   Book Title
    
-   Author Name
    
-   Number of Available Copies
    

After submitting the form, the system stores the book record in the database.

----------
# 🏗 Project Structure  

**LMS.Razor**  
│  
├── Data  
│ ├── BookRecord.cs  
│ ├── BooksRepository.cs  
│ ├── Database.cs  
│ ├── LoanRecord.cs  
│ └── Models.cs  
│  
└── ReadMe.md

----------
  
# 📂 Data Layer Overview  
  
The **Data** folder contains all models and repository classes responsible for communicating with the database.  
  
**It follows a simple layered design:**  

Razor Pages  
↓  
Repository Layer  
↓  
Database Utility  
↓  
SQL Server

  
---  
  
# 📘 BookRecord.cs  
  
Represents a **book entity** stored in the library database.  
  
### Fields / Properties  
  
| Property | Type | Description |  
|--------|--------|--------|  
| BookId | int | Unique identifier for the book |  
| Title | string | Title of the book |  
| Author | string | Author of the book |  
| Isbn | string | ISBN identifier |  
| Category | string? | Optional book category |  
| TotalCopies | int | Total copies in the library |  
| AvailableCopies | int | Copies currently available |  
| CreatedAt | DateTime | Date when record was created |  
| UpdatedAt | DateTime? | Last updated timestamp |  
  
Example structure:  
  
```csharp  
public class BookRecord  
{  
 public int BookId { get; set; }  
 public string Title { get; set; } = "";  
 public string Author { get; set; } = "";  
 public string Isbn { get; set; } = "";  
 public string? Category { get; set; }  
 public int TotalCopies { get; set; }  
 public int AvailableCopies { get; set; }  
 public DateTime CreatedAt { get; set; }  
 public DateTime? UpdatedAt { get; set; }  
}
```

---



# 📘**BooksRepository.cs**

The **BooksRepository** class is responsible for handling all **book-related database operations**.

It follows the **Repository Pattern**, which separates database logic from application logic.

### Constructor

public  BooksRepository(Database  db)  
{  
  _db  =  db;  
}

The repository receives the **Database class via dependency injection**.

----------

## Methods

### 1️⃣ List Books

Returns all books with optional search functionality.

public  DataTable  List(string?  q)

Search is applied to:

-   Title
    
-   Author
    
-   ISBN
    

SQL Query:

SELECT BookId, Title, Author, AvailableCopies, TotalCopies  
FROM dbo.Books  
WHERE (@q =  ''  OR Title LIKE @p OR Author LIKE @p OR Isbn LIKE @p)  
ORDER  BY Title;

----------

### 2️⃣ Get Book by ID

Fetches a single book record.

public  DataRow?  Get(int  id)

Query used:

SELECT TOP 1  *  FROM dbo.Books WHERE BookId=@id

----------

### 3️⃣ Delete Book

Deletes a book record using its ID.

public  int  Delete(int  id)

SQL:

DELETE  FROM dbo.Books WHERE BookId=@id

----------

# 📘 Database.cs

The **Database class** is a reusable utility for executing SQL commands.

It handles:

-   Opening SQL connections
    
-   Executing queries
    
-   Executing stored procedures
    
-   Adding parameters safely
    

----------

## Connection Initialization

The connection string is loaded from **appsettings.json**.

public  Database(IConfiguration  config)  
{  
  _connStr  =  config.GetConnectionString("LmsDb")  
  ??  throw  new  ArgumentNullException(nameof(config));  
}

Example configuration:

{  
 "ConnectionStrings": {  
 "LmsDb": "Server=.;Database=LMS;Trusted_Connection=True;"  
 }  
}

----------

# 📊 Database Operations

## Query (SELECT)

Returns results in a **DataTable**.

public  DataTable  Query(string  sql, params  SqlParameter[] parameters)

Example usage:

_db.Query("SELECT * FROM Books");

----------

## Execute (INSERT / UPDATE / DELETE)

public  int  Execute(string  sql, params  SqlParameter[] parameters)

Returns number of affected rows.

----------

# ⚙ Stored Procedure Support

### Execute Procedure Returning Data

public  DataTable  ExecProc(string  procName, params  SqlParameter[] parameters)

### Execute Procedure Without Result

public  int  ExecProcNonQuery(string  procName, params  SqlParameter[] parameters)

----------

# 🔧 Parameter Handling

Parameters are safely added to prevent SQL injection.

Example:

new  SqlParameter("@id", id)

Helper method automatically handles null values and string parameters.

----------

# 📘 LoanRecord.cs

Represents a **loan transaction** between a library member and a book.

### Properties

| Property | Type | Description |
|----------|------|-------------|
| LoanId | int | Loan ID |
| MemberId | int | Member borrowing the book |
| BookId | int | Borrowed book ID |
| MemberName | string | Name of member |
| BookTitle | string | Title of the book |
| IssueDate | DateTime | Book issue date |
| DueDate | DateTime | Return deadline |
| ReturnDate | DateTime? | Date book returned |
| FineAmount | decimal? | Fine for late return |

----------

# 📘 SimpleOption Model

Used for simple **dropdown options** in UI.

public  class  SimpleOption  
{  
  public  int  Id { get; set; }  
  public  string  Label { get; set; } =  "";  
}

----------

# 📘 Models.cs

Contains additional DTO models.

----------

## MemberDto

Represents a simple member object.

public  class  MemberDto  
{  
  public  int  MemberId { get; set; }  
  public  string  FullName { get; set; } =  "";  
}

----------

## BookDto

Represents simplified book information.

public  class  BookDto  
{  
  public  int  BookId { get; set; }  
  public  string  Title { get; set; } =  "";  
}

----------

# 🔐 Security Considerations

This project prevents **SQL Injection** by using parameterized queries.

Benefits:

-   Prevents malicious SQL injection
    
-   Ensures safe database interaction
    
-   Improves query reliability
    

Example:

cmd.Parameters.Add(new  SqlParameter("@id", id));


----------

## ⚙️ Features

### 1️⃣ Form Submission

The form uses the **POST method** to submit book information to the server.

```
<form method="post">

```

The request is handled by the `OnPost()` method inside the `CreateModel` page model.

----------

### 2️⃣ Model Binding

ASP.NET automatically binds form fields to the page model properties using:

```
asp-for="PropertyName"

```

Example:

```
<input asp-for="Title" />

```

This binds the input value to the `Title` property in the backend model.

----------

### 3️⃣ Validation Handling

Validation errors are displayed using:

```
@Html.ValidationSummary()

```

If any required fields are missing or invalid, error messages appear in red.

----------

### 4️⃣ Input Constraints

The **Copies field** restricts users to positive numbers.

```
<input asp-for="AvailableCopies" type="number" min="0" />

```

This prevents negative values.

----------

### 5️⃣ Navigation

Two actions are available:

Button

Function

**Save**

Submits the form and creates the book

**Cancel**

Redirects the user to the Book List page

Cancel navigation:

```
<a asp-page="Index">Cancel</a>

```

----------

## 🧠 Backend Model (Example)

The page connects to a Razor Page Model:

```csharp
public class CreateModel : PageModel
{
    [BindProperty]
    public string Title { get; set; }

    [BindProperty]
    public string Author { get; set; }

    [BindProperty]
    public int AvailableCopies { get; set; }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Save book logic here

        return RedirectToPage("Index");
    }
}

```

----------

## 📌 Example Workflow

1.  User opens **Add Book page**
    
2.  User enters book details
    
3.  Clicks **Save**
    
4.  Form sends a POST request
    
5.  Server validates the data
    
6.  Book record is saved
    
7.  User is redirected to the **Book List page**
    

----------

## 🛠 Technologies Used

-   ASP.NET Core Razor Pages
    
-   C#
    
-   HTML
    
-   Tag Helpers
    
-   Model Binding
    
-   Server-side Validation
    

----------


