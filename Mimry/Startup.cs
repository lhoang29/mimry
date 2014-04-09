using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Mimry.Startup))]
namespace Mimry
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
