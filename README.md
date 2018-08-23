
[VehicleCostsMonitor](https://github.com/msotiroff/Asp.Net-Core-Projects/tree/master/VehicleCostsMonitor "JustMonitor") or [JustMonitor](https://github.com/msotiroff/Asp.Net-Core-Projects/tree/master/VehicleCostsMonitor "JustMonitor")
is a system for monitoring vehicle costs(fuel, maintenance, tax, insuranse, ect.) in which the users can register and login in order to add vehicles to their garage, for each vehicle its owner can add a picture, fuel and another cost entries. Vehicle details page show many statistics about it displayed in different charts. In the project are also implemented different user roles and areas separated in private sections for admins and regular users. Depending on its role every user can access different sections of the application and has specific permissions of what can or cannot do. Every visitor can see statistics about most economic vehicles by fuel type and export the result to Excel worksheet.


Used technologies:
- Asp.Net Core 2.1
- Entity Framework Core

Some features:
- Guest users can view vehicles profile page
- Guest users can search vehicles
- Guest users can register and login (incl. Facebook and Google+ authentication)
- Registered users can add vehicles to their profile
- Registered users can add fuel and cost entries to their vehicles choosing of over 30 currencies
- Registered users can create/update/delete their vehicles and entries
- Registered users can set up the display currency for showing their statistics
- Registered users can export their fuelings and costs to Excel worksheet
- Admin users can create/update/delete all vehicle manufacturers and models
- Admin users can see all users details (incl. vehicles, roles, ect.)
- Admin users can add/remove users to roles
- All Admin actions are logged

#### Getting started:
Just type your server name in the ConnectionStrings section at appsettings.json and run! There is a seed method, which will insert all manufacturers, models, vehicle elements and about 100 sample users, 800 vehicles with more than 30'000 fuel and cost entries. Database seeding should take about 2-3 minutes, depending on your machine.

#### Enjoy :)



