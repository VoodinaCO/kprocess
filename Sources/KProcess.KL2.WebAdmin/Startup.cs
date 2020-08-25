using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(KProcess.KL2.WebAdmin.Startup))]
namespace KProcess.KL2.WebAdmin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
