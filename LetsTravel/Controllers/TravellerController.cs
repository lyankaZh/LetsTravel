using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using LetsTravel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace LetsTravel.Controllers
{
    [Authorize(Roles = "Traveller")]
    public class TravellerController : Controller
    {

        private readonly IExcursionRepository repository;

        public TravellerController(IExcursionRepository repository)
        {
            this.repository = repository;
        }

        public ActionResult ShowSubscribedExcursions()
        {
            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            return View("TravellerView", user.Excursions.ToList());
        }

        public ActionResult Subscribe(Excursion excursion)
        {  
                var user = (User) repository.GetUserById(User.Identity.GetUserId());
                user.Excursions.Add(excursion);
                repository.UpdateUser(user);

                excursion.Users.Add(user);
                repository.UpdateExcursion(excursion);
                repository.Save();

                return ShowSubscribedExcursions();
            
        }

        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        private void AddErrors(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}