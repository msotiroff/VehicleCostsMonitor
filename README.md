# Asp.Net-Core-Projects
Projects made using Asp.Net Core 2.0 and above

------------
### 1. [JustMonitor](https://github.com/msotiroff/Asp.Net-Core-Projects/tree/master/VehicleCostsMonitor "JustMonitor")(under construction)
The application represents a system for monitoring vehicle costs(fuel, maintenance, tax, insuranse, ect.) in which the users can register and login in order to add vehicles to their garage, for each vehicle its owner can add a picture, fuel and another cost entries. Vehicle details page show many statistics about it. In the project are also implemented different user roles and areas separated in private sections for admins and regular users. Depending on its role every user can access different sections of the application and has specific permissions of what can or cannot do.


Used technologies:
- Asp.Net Core 2.1.1
- Entity Framework Core

Some features:
- Guest users can view vehicles profile page
- Guest users can search vehicles
- Guest users can register and login
- Registered users can add vehicles to their profile
- Registered users can add fuel and cost entries to their vehicles
- Registered users can create/update/delete their vehicles
- Registered users can send private messages to another registered users
- Admin users can create/update/delete all vehicle manufacturers and models
- Admin users can see all users details (incl. vehicles, roles, ect.)
- Admin users can add/remove users to roles
- All Admin actions are logged

#### Getting started:
Just type your server name in the ConnectionStrings section at appsettings.json and run! There is a seed method, which will insert all manufacturers, models, vehicle elements and about 500 sample users, 1000 vehicles with more than 30'000 fuel entries and nearly 8'000 cost entries. The seed method should take about 0.5-1 minute, depending on your machine.
#### Enjoy :)

### 2. [OnlineStore](https://github.com/msotiroff/Asp.Net-Core-Projects/tree/master/OnlineStore "OnlineStore")
E-Commerce system, made using ASP.NET Web API. The Asp.Net MVC application in this solution has made only to demonstrate how WEB API can be consumed by another Asp.Net Core application. The API is absolutely decoupled by the MVC project and it can be consumed by any client-side application.

Some features:
- Guest users can view categories and products in them.
- Guest users can use the shopping cart and purchase some products after fill out a form
- Registered users can send feedback messages
- Admin users can create/update/delete categories
- Admin users can create/update/delete products


