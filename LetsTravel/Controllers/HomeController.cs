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
            FormCorrectTextOnMenuButtons();
            return View();
        }

      

        public ActionResult Register()
        {
           return new RedirectResult("/Account/Register");
        }

        public ActionResult About()
        {
            FormCorrectTextOnMenuButtons();
            return View();
        }

        public void FormCorrectTextOnMenuButtons()
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
        }

    }
}