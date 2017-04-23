using System;
using System.Web;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using LetsTravel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace LetsTravel.Controllers
{
    [Authorize(Roles = "Guide")]
    public class GuideController : Controller
    {
        private readonly IExcursionRepository repository;

        public GuideController(IExcursionRepository repository)
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
            return View("GuideView", repository.GetExcursionsByGuideId(user.Id));
        }

        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
    }
}