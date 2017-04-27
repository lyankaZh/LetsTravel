﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using LetsTravel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace LetsTravel.Controllers
{
    [Authorize(Roles = "Traveller")]
    public class TravellerController : Controller
    {

        private readonly ITravelRepository repository;

        public TravellerController(ITravelRepository repository)
        {
            this.repository = repository;
        }

        public ActionResult ShowSubscribedExcursions()
        {

            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            var subscribedExcursions = new List<ExcursionWithGuideInfoViewModel>();

            foreach (var excursion in user.Excursions.ToList())
            {
                var excursionToDisplay = new ExcursionWithGuideInfoViewModel();
                excursionToDisplay.ExcursionId = excursion.ExcursionId;
                excursionToDisplay.City = excursion.City;
                excursionToDisplay.Date = excursion.Date;
                excursionToDisplay.Description = excursion.Description;
                excursionToDisplay.Duration = excursion.Duration;
                excursionToDisplay.PeopleLimit = excursion.PeopleLimit;
                excursionToDisplay.Price = (int)excursion.Price;
                excursionToDisplay.Route = excursion.Route;
                excursionToDisplay.ExcursionId = excursion.ExcursionId;
                excursionToDisplay.ModalId = "#" + excursion.ExcursionId.ToString();
                excursionToDisplay.Guide = (User)repository.GetUsers().First(u => u.Id == excursion.Guide);
                subscribedExcursions.Add(excursionToDisplay);
            }
            return View("TravellerView", subscribedExcursions);
        }

        public ActionResult Subscribe(ExcursionForTraveller model)
        {
            var user = (User)repository.GetUserById(User.Identity.GetUserId());
            var excursion = repository.GetExcursionById(model.ExcursionId);
            user.Excursions.Add(excursion);
            repository.UpdateUser(user);
            excursion.Users.Add(user);
            repository.UpdateExcursion(excursion);
            repository.Save();
            return RedirectToAction("ShowSubscribedExcursions");

        }

        public ActionResult UnSubscribe(ExcursionWithGuideInfoViewModel excursion)
        {
            var user = (User)repository.GetUserById(User.Identity.GetUserId());
            var excursionToDelete = user.Excursions.Find(x => x.ExcursionId == excursion.ExcursionId);
            user.Excursions.Remove(excursionToDelete);
            repository.UpdateUser(user);
            repository.Save();
            return RedirectToAction("ShowSubscribedExcursions");
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