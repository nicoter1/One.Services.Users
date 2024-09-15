# Project Title

Web api to manage Users
# Instructions
1. You must use .NET Core 8 and C# 12
2. The API should be an ASP.NET Core Web API project
3. The API should consume and return data as JSON
4. You do not need to consider any security implications. This includes using HTTPS or attempting to provide any Authorization/Authentication.
5. You can use any persistent storage. SQL Server or in-memory DB are sufficient but should be treated as production data stores.
6. You can use any NuGet package, but be prepared to justify its usage.
7. Please add logging to all the operations. Log all exceptions, and avoid logging sensitive information, such as emails, in plain text.
8. Please make sure the code is testable.
## Installation

Instructions on how to install and set up the project.
open the code in visual studio
open developer powerShell
run 
  dotnet ef migrations add InitialCreate --context UserDbContext  --project ./One.Database.Users --startup-project ./One.Services.Web.Users
  dotnet ef migrations remove  --context UserDbContext  --project ./One.Database.Users --startup-project ./One.Services.Web.Users
  dotnet ef database update --context UserDbContext --project ./One.Database.Users --startup-project ./One.Services.Web.Users
  


  dotnet ef migrations list --context UserDbContext --project ./One.Database.Users --startup-project ./One.Services.Web.Users

  dotnet ef migrations add AddUniqueConstraintToEmail --context UserDbContext --project ./One.Database.Users --startup-project ./One.Services.Web.Users

  dotnet ef database update --context UserDbContext --project ./One.Database.Users --startup-project ./One.Services.Web.Users
# Testing 
See attached postman collection in folder docs


# comments
This is not really production, since in production I would:
 - use a real authentication/authorization server for generating the bearer token, not hardcode the key, and username and password in app settings
 - if the number of users increases, I would not allow one endpoint to return all users, with no search criteria
 - in production I would have history tables to keep the history of all changes to users, and I would not have the delete option for user; I would put a disable flag 
 - I am using SQL server database, EF code first, migrations,  the database can be recreated on any machine
 - I added a postman collection which handles the authentication; this can be used to test all endpoints
 - this service logs in debug window and console; in production I would use at least a db audit log or other audit log service 


