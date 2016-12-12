using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Softuni_Project.Models;

namespace Softuni_Project.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<Softuni_Project.Models.BlogDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "Softuni_Project.Models.BlogDbContext";
        }

        protected override void Seed(Softuni_Project.Models.BlogDbContext context)
        {

            if (!context.Roles.Any())
            {
                this.CreateRole("Admin", context);
                this.CreateRole("User", context);

            }

            if (!context.Users.Any())
            {

                this.CreateUser("admin@admin.com", "Admin", "123", context);
                this.SetUserRole("admin@admin.com", "Admin", context);

            }


        }

        private void CreateRole(string roleName, BlogDbContext context)
        {
           var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var result = roleManager.Create(new IdentityRole(roleName));

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(";", result.Errors));

            }

        }

        private void SetUserRole(string email, string role, BlogDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var userId = context.Users.FirstOrDefault(u => u.Email.Equals(email)).Id;
            var result = userManager.AddToRole(userId, role);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(";", result.Errors));

            }


        }

        private void CreateUser(string email, string fullName, string password, BlogDbContext context)
        {
            
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            userManager.PasswordValidator = new PasswordValidator()
            {
                RequireDigit = false,
                RequiredLength = 1,
                RequireLowercase = false,
                RequireNonLetterOrDigit = false,
                RequireUppercase = false
            };
            var user = new ApplicationUser()
            {
                Email =  email,
                FullName = fullName,
                UserName = email
            };

            // Setting the password from the UserManager rather than storing it in the db as plain string
            var result = userManager.Create(user,password);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(";", result.Errors));
            }
        }
    }
}
