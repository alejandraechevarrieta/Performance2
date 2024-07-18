﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Net.Http;

namespace Performance
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.MapHttpRoute(
            //               name: "DefaultApi",
            //              routeTemplate: "api/{controller}/{id}",
            //               defaults: new { id = RouteParameter.Optional }
            //           );

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/PerformanceApp/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
