using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LetsTravel.Helpers;

namespace LetsTravel.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.LogInOrOutText = "Log out";
                if (User.IsInRole("Guide") || User.IsInRole("Traveller"))
                {
                    return View("GuideAndTravellerHomeView");
                }
                else
                {
                    return View("AdminHomeView");
                }
            }
            else
            {
                ViewBag.LogInOrOutText = "Log in";
                return View("GuestHomeView");
            }
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

        public ActionResult ChangeCurrentCulture(int id)
        {
            //  
            // Change the current culture for this user.  
            //  
            CultureHelper.CurrentCulture = id;
            //  
            // Cache the new current culture into the user HTTP session.   
            //  
            Session["CurrentCulture"] = id;
            //  
            // Redirect to the same page from where the request was made!   
            //  
            return Redirect(Request.UrlReferrer.ToString());
        }

    }
}