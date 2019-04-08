using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(bugTracker.Startup))]
namespace bugTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
