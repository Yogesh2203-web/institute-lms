# Institute LMS — Full-stack starter
Stack: ASP.NET Core 10, EF Core 10, SQL Server, Angular 20.

## Backend
1. Open `backend/InstituteLMS.API/InstituteLMS.API.csproj` in Visual Studio.
2. Edit `appsettings.json`: set SQL Server connection string and a unique JWT key (32+ characters).
3. From `backend/InstituteLMS.API` run:
   `dotnet restore`
   `dotnet tool install --global dotnet-ef` (only if not already installed)
   `dotnet ef migrations add InitialCreate`
   `dotnet ef database update`
   `dotnet run`
4. Open the Swagger URL printed in the terminal.

Use `POST /api/Branches` to create a branch after obtaining an admin account. For first setup,
temporarily enable `AllowPublicBusinessHeadRegistration` in appsettings and register a BusinessHead
using `POST /api/Auth/register`; then disable that setting. Public registration otherwise allows
Student accounts only. A Student requires a valid BranchId.

## Frontend
From `frontend/institute-lms-ui` run `npm install` then `npm start`.
Open `http://localhost:4200`. API URL is in `src/environments/environment.ts`.

## Included
JWT auth/login/register, password hashing, roles, EF Core entities, branch-scoped CRUD foundation,
Swagger, and a responsive Angular dashboard starter.

## Before production
This is a starter project, not a production-certified LMS. Add and test granular branch authorization
for every endpoint, staff provisioning, audit logs, password reset/email verification, refresh-token
rotation, validation, pagination, upload scanning/storage, automated tests, backups, monitoring,
HTTPS, and production secrets. Payment processing must use a compliant provider; do not store card data.
