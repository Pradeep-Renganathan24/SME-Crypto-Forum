using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CryptoForum.Startup))]
namespace CryptoForum
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
