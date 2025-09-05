Overview

This project is a technical assignment built with ASP.NET Core 7 Web API and Razor Pages, using Entity Framework Core (Code First) with SQL Server/Oracle.
It allows you to Add, Edit, Delete, List, Search, Sort, Page, and Export Employee Data with picture upload support.


Tech Stack
Backend: ASP.NET Core Web API
Frontend: Razor Pages
Database: SQL Server (can be adapted to Oracle)
ORM: Entity Framework Core (Code First Migrations)
Export: EPPlus (Excel Export)

CRUD Operations (Add, Edit, Delete Employees)
 Search & Filter Employees
 Paging & Sorting on employee list
 Picture Upload (JPG/PNG stored in DB as varbinary)
 Export filtered/sorted employees to Excel
 REST API + Razor Pages Frontend (can be used independently)


dotnet ef migrations add InitialCreate
dotnet ef database update
