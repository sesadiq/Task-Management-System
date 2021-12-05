using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OfficeFinal.Startup))]
namespace OfficeFinal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
