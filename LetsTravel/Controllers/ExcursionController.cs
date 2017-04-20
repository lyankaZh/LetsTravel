﻿using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Concrete;
using Domain.Entities;

namespace LetsTravel.Controllers
{

    public class ExcursionController : Controller
    {
        private readonly IExcursionRepository repository;

        public ExcursionController(IExcursionRepository repository)
        {
            this.repository = repository;
        }

        public string Index()
        {
            return "Hello";
        }

     
        [HttpPost]
        public ActionResult AddExcursion(Excursion excursion)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repository.InsertExcursion(excursion);
                    repository.Save();
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("",
                    "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return Json(excursion);
        }


       
        
        public ActionResult GetExcursions()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Guide"))
                {
                    return View("GuideView",repository.GetExcursions());
                }
                else if (User.IsInRole("Traveller"))
                {
                    return View("TravellerView", repository.GetExcursions());
                }
                else
                {
                    return View("AdminView", repository.GetExcursions());
                }
            }
            else
            {
                return View("GuestPageView", repository.GetExcursions());
            }
        }
    }
}
