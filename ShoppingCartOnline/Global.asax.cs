using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using System.Web.Security;
using System.Security.Principal;

namespace ShoppingCartOnline
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Application["PageView"] = 0;
            Application["UserOnline"] = 0;

        }

        //thống kê số lượng người truy cập vào website
        protected void Session_Start()
        {
            Application.Lock(); //dùng để đồng bộ hóa khi có nhiều user truy cập vào
            Application["PageView"] = (int)Application["PageView"] + 1;
            Application["UserOnline"] = (int)Application["UserOnline"] + 1;

            Application.UnLock();
        }

        //thống kê số lương người đang online ở website
        protected void Session_End()
        {
            Application.Lock(); //dùng để đồng bộ hóa khi có nhiều user truy cập vào
            Application["PageView"] = (int)Application["PageView"] + 1;
            Application["UserOnline"] = (int)Application["UserOnline"] - 1;

            Application.UnLock();
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            var TaiKhoanCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if(TaiKhoanCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(TaiKhoanCookie.Value);
                var quyen = authTicket.UserData.Split(new char[] { ',' });
                var userPrincipal = new GenericPrincipal(new GenericIdentity(authTicket.Name), quyen);
                Context.User = userPrincipal;
            }
        }
    }
}
