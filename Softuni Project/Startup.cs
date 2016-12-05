using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Softuni_Project.Startup))]
namespace Softuni_Project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
