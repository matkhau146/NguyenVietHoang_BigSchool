using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NguyenVietHoang_BigSchool.Startup))]
namespace NguyenVietHoang_BigSchool
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
