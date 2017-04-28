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
using System.Globalization;

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


        public ActionResult ShowOwnExcursions()
        {
            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            var excursionsToDisplay = new List<ExcursionModel>();
            foreach (var excursion in repository.GetExcursionsByGuideId(user.Id))
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
                excursionToDisplay.Subscribers = repository.GetSubscribersByExcursionId(excursion.ExcursionId, User.Identity.GetUserId());
                excursionToDisplay.ModalId = "#" + excursion.ExcursionId;
                excursionsToDisplay.Add(excursionToDisplay);
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

        public ActionResult EditExcursion(ExcursionModel model)
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
            //if (ModelState.IsValid)
            //{
            //    User user = (User)repository.GetUserById(model.Id);
            //    var amountOfUsersWithSameNick =
            //        (from u in repository.GetUsers()
            //         where u.UserName == model.UserName && u.UserName != user.UserName
            //         select u).Count();
            //    if (amountOfUsersWithSameNick >= 1)
            //    {
            //        ModelState.AddModelError("", "Such nickname already exists");
            //        return Edit();
            //    }
            //    var amountOfUsersWithSameEmail =
            //        (from u in repository.GetUsers()
            //         where u.Email == model.Email && u.Email != user.Email
            //         select u).Count();

            //    if (amountOfUsersWithSameEmail >= 1)
            //    {
            //        ModelState.AddModelError("", "Such email already exists");
            //        return Edit();
            //    }

            //    user.UserName = model.UserName;
            //    user.FirstName = model.FirstName;
            //    user.LastName = model.LastName;
            //    user.Email = model.Email;
            //    if (!string.IsNullOrEmpty(model.AboutMyself))
            //    {
            //        user.AboutMyself = model.AboutMyself;
            //    }
            //    if (image != null)
            //    {
            //        user.ImageMimeType = image.ContentType;
            //        user.ImageData = new byte[image.ContentLength];
            //        image.InputStream.Read(user.ImageData, 0, image.ContentLength);
            //    }
            //    repository.UpdateUser(user);
            //    repository.Save();
            //    return ShowProfile();
            //}
            //return Edit();
        }

        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
    }
}