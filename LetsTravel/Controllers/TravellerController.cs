using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Entities;
using Microsoft.AspNet.Identity.Owin;

namespace LetsTravel.Controllers
{
    [Authorize(Roles = "Traveller")]
    public class TravellerController : Controller
    {
        
        public ActionResult ShowSubscribedExcursions()
        {
            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            return View("TravellerView", user.Excursions.ToList());
        }

     
        public ActionResult Subscribe()
        {
            throw  new NotImplementedException();
        }
        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

    }
}