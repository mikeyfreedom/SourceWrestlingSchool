using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SourceWrestlingSchool.Startup))]
namespace SourceWrestlingSchool
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
