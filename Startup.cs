using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ApprovalPortal.Startup))]
namespace ApprovalPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
