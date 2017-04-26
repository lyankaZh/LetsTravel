using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using LetsTravel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;

namespace LetsTravel.Controllers
{
    [Authorize(Roles = "Guide")]
    public class GuideController : Controller
    {
        private readonly ITravelRepository repository;

        public GuideController(ITravelRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        public ActionResult CreateExcursion(ExcursionCreationModel model)
        {
            if (ModelState.IsValid)
            {
                string id = User.Identity.GetUserId();
                Excursion excursion = new Excursion()
                {
                    City = model.City,
                    Date = model.Date,
                    Description = model.Description,
                    Duration = model.Duration,
                    PeopleLimit = model.PeopleLimit,
                    Route = model.Route,
                    Guide = id
                };
                repository.InsertExcursion(excursion);
                repository.Save();
                return ShowOwnExcursions();
            }
            return View("_PartialCreateExcursionView");
        }

        [HttpGet]
        public ViewResult CreateExcursion()
        {
            return View("_PartialCreateExcursionView");
        }

        public ActionResult ShowOwnExcursions()
        {
            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            var excursionsToDisplay = new List <ExcursionSubscribersViewModel> ();        
            foreach (var excursion in repository.GetExcursionsByGuideId(user.Id))
            {
                var excursionToDisplay = new ExcursionSubscribersViewModel();
                excursionToDisplay.ExcursionId = excursion.ExcursionId;
                excursionToDisplay.City = excursion.City;
                excursionToDisplay.Date = excursion.Date;
                excursionToDisplay.Description = excursion.Description;
                excursionToDisplay.Duration = excursion.Duration;
                excursionToDisplay.PeopleLimit = excursion.PeopleLimit;
                excursionToDisplay.Price = excursion.Price;
                excursionToDisplay.Route = excursion.Route;
                excursionToDisplay.Subscribers = repository.GetSubscribersByExcursionId(excursion.ExcursionId, User.Identity.GetUserId());
                excursionToDisplay.ModalId = "#" + excursion.ExcursionId.ToString(); 
                excursionsToDisplay.Add(excursionToDisplay);
            }         
            return View("GuideView", excursionsToDisplay);
        }
         
        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
    }
}