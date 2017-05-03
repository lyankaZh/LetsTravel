using System.Web;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using LetsTravel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Web.WebPages;

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
        public ActionResult CreateExcursion(ExcursionModel model)
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
                    Price = model.Price,
                    PeopleLimit = model.PeopleLimit,
                    Route = model.Route,
                    Guide = id
                };
                repository.InsertExcursion(excursion);
                repository.Save();
                return RedirectToAction("ShowOwnExcursions");
            }
            return View("CreateExcursionView");
        }

        [HttpGet]
        public ViewResult CreateExcursion()
        {
            return View("CreateExcursionView");
        }


        public ViewResult ShowOwnExcursions()
        {
            //var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            var user = repository.GetUserById(User.Identity.GetUserId());
            var excursionsToDisplay = new List<ExcursionModel>();
            foreach (var excursion in repository.GetExcursions().Where(x => x.Guide == user.Id))
            {
                var excursionToDisplay = new ExcursionModel();
                excursionToDisplay.ExcursionId = excursion.ExcursionId;
                excursionToDisplay.City = excursion.City;
                excursionToDisplay.Date = excursion.Date;
                excursionToDisplay.Description = excursion.Description;
                excursionToDisplay.Duration = excursion.Duration;
                excursionToDisplay.PeopleLimit = excursion.PeopleLimit;
                excursionToDisplay.Price = (int)excursion.Price;
                excursionToDisplay.Route = excursion.Route;
                excursionToDisplay.Subscribers = excursion.Users.ToList();
                excursionToDisplay.ModalId = "#" + excursion.ExcursionId;
                excursionsToDisplay.Add(excursionToDisplay);
            }
            if (excursionsToDisplay.Count == 0)
            {
                ViewBag.NoExcursions = "You don't have any excursions yet";
            }
            return View("OwnExcursionsGuideView", excursionsToDisplay);
        }

        [HttpPost]
        public ActionResult DeleteExcursion(int excursionId)
        {
            var excursion = repository.GetExcursionById(excursionId);
            if (excursion != null)
            {
                if (excursion.Users.Count != 0)
                {
                    TempData["excursionDeleted"] = "You can`t delete this excursion because it has subscribers";
                }
                else
                {
                    repository.DeleteExcursion(excursionId);
                    repository.Save();
                    TempData["excursionDeleted"] = "Excursion has been deleted";
                }
            }
            return RedirectToAction("ShowOwnExcursions");
        }

        public ViewResult EditExcursion(ExcursionModel model)
        {
            return View("EditExcursionView", model);
        }

        [HttpPost]
        public ActionResult Edit(ExcursionModel model)
        {
            if (ModelState.IsValid)
            {
                var excursion = repository.GetExcursionById(model.ExcursionId);
                if (excursion != null)
                {
                    if (excursion.Users.Count > model.PeopleLimit)
                    {
                        ModelState.AddModelError("", "There are more subscribers than in new limit of people value");
                        return View("EditExcursionView", model);
                    }
                    excursion.City = model.City;
                    excursion.Date = model.Date;
                    excursion.Description = model.Description;
                    excursion.Duration = model.Duration;
                    excursion.PeopleLimit = model.PeopleLimit;
                    excursion.Price = model.Price;

                    excursion.Route = model.Route;
                    repository.UpdateExcursion(excursion);
                    repository.Save();
                    return RedirectToAction("ShowOwnExcursions");

                }
            }
            return View("EditExcursionView", model);
        }

        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
    }
}