using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HealthTrackPro.Web.Startup))]
namespace HealthTrackPro.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
