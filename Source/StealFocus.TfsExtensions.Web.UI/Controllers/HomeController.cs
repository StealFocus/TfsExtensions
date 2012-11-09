namespace StealFocus.TfsExtensions.Web.UI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return this.View();
        }

        public ActionResult About()
        {
            return this.View();
        }
    }
}
