using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SportsStore.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //здесь настраиваем перенаправление на нужный метод действия при запуске сайта
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //url "/" первая страница со всеми товарами 
            routes.MapRoute(
                null,
                "",
                new
                {
                    controller = "Product",
                    action = "List",
                    category = (string)null, 
                    page = 1
                });
            
            //здесь заменяем page=номер страницы на Page1...
            //url "/Page2"
            routes.MapRoute(
                name: null,
                url: "Page{page}",
                new { controller = "Product", action = "List", category=(string)null },
                new {page= @"\d+"}
                );
            
            //метод по умолчанию, указывает на наш контроллер ProductController
            //url "/Soccer"
            routes.MapRoute(
                name: "null",
                url: "{category}",
                //defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                defaults: new { controller = "Product", action = "List", page=1 }
            );
            //url "/Soccer/Page2"
            routes.MapRoute(
                null,
                "{category}/ Page{ page}",
                new { controller = "Product", action = "List" },
                new { page = @"\d+" }
                );

            routes.MapRoute(null, "{controller}/{action}");
        }
    }
}
