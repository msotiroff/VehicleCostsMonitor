namespace OnlineStore.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using OnlineStore.Data;
    using OnlineStore.Models;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public static class ApplicationBuilderExtensions
    {
        private const string CategoriesJsonFilePath = @"wwwroot\seedfiles\JsonFormattedCategories.txt";
        private const string ProductsJsonFilePath = @"wwwroot\seedfiles\JsonFormattedProducts.txt";
        private const string PicturesJsonFilePath = @"wwwroot\seedfiles\JsonFormattedPictures.txt";

        /// <summary>
        /// This is a workaround for missing seed functionality in EF Core
        /// </summary>
        /// <param name="app">
        /// An instance that provides the mechanisms to get instance of the database context.
        /// </param>
        public static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var database = serviceScope.ServiceProvider.GetService<OnlineStoreDbContext>();
                database.Database.Migrate();

                var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                var isDbSown = database.Categories.Any();
                if (!isDbSown)
                {
                    database.Database.EnsureDeleted();
                    database.Database.Migrate();

                    SeedDefaultRoles(userManager, roleManager);

                    SeedCategories(database);
                    SeedProducts(database);
                    SeedPictures(database);
                }

                return app;
            }
        }

        private static void SeedCategories(OnlineStoreDbContext database)
        {
            Task
                .Run(async () =>
                {
                    var jsonCategories = File.ReadAllText(CategoriesJsonFilePath);
                    var categories = JsonConvert.DeserializeObject<Category[]>(jsonCategories);

                    await database.Categories.AddRangeAsync(categories);
                    await database.SaveChangesAsync();
                })
                .Wait();
        }

        private static void SeedProducts(OnlineStoreDbContext database)
        {
            Task
                .Run(async () =>
                {
                    var jsonProducts = File.ReadAllText(ProductsJsonFilePath);
                    var products = JsonConvert.DeserializeObject<Product[]>(jsonProducts);

                    await database.Products.AddRangeAsync(products);
                    await database.SaveChangesAsync();
                })
                .Wait();
        }

        private static void SeedPictures(OnlineStoreDbContext database)
        {
            Task
                .Run(async () =>
                {
                    var jsonPictures = File.ReadAllText(PicturesJsonFilePath);
                    var pictures = JsonConvert.DeserializeObject<Picture[]>(jsonPictures);

                    await database.Pictures.AddRangeAsync(pictures);
                    await database.SaveChangesAsync();
                })
                .Wait();
        }

        private static void SeedDefaultRoles(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            Task
                .Run(async () =>
                {
                    var adminRole = WebConstants.AdministratorRole;

                    var roleExist = await roleManager.RoleExistsAsync(adminRole);

                    if (!roleExist)
                    {
                        await roleManager.CreateAsync(new IdentityRole(adminRole));
                    }

                    await AddAdminUser(userManager, adminRole);

                })
                .Wait();
        }

        private static async Task AddAdminUser(UserManager<User> userManager, string adminRole)
        {
            var adminEmail = WebConstants.AdminEmail;
            var adminFirstName = WebConstants.AdministratorFirstName;
            var adminLastName = WebConstants.AdministratorLastName;
            var adminPassword = WebConstants.AdministratorPassowrd;

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    FirstName = adminFirstName,
                    LastName = adminLastName
                };

                await userManager.CreateAsync(adminUser, adminPassword);

                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}
