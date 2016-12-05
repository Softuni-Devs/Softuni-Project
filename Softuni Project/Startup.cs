using System.Data.Entity;
using Microsoft.Owin;
using Owin;
using Softuni_Project.Migrations;
using Softuni_Project.Models;

[assembly: OwinStartupAttribute(typeof(Softuni_Project.Startup))]
namespace Softuni_Project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BlogDbContext,Configuration>());
            ConfigureAuth(app);
        }
    }
}
