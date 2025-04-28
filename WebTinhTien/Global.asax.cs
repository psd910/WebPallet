using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using CoreWebTinhTien.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebTinhTien
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var container = new WindsorContainer(new XmlInterpreter("services.config"));
            IoC.Initialize(container);
        }
    }
}
