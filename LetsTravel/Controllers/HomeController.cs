using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LetsTravel.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    ViewBag.MenuItemName = "Administration";
                }
                else
                {
                    ViewBag.MenuItemName = "My excursions";
                }
                ViewBag.LogInOrOutText = "Log out";
            }
            else
            {
                ViewBag.MenuItemName = "Explore it";
                ViewBag.LogInOrOutText = "Log in";
            }
            return View();
        }

        public ActionResult Register()
        {
           return new RedirectResult("/Account/Register");
        }

        public ActionResult About()
        {
            return View();
        }
    }
}