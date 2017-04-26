using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using System.Web;
using LetsTravel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace LetsTravel.Controllers
{

    public class ExcursionController : Controller
    {
        private readonly IExcursionRepository excursionRepository;

        public ExcursionController(IExcursionRepository repository)
        {
            excursionRepository = repository;
        }

        //public string Index()
        //{
        //    return "Hello";
        //}

     
        //[HttpPost]
        //public ActionResult AddExcursion(Excursion excursion)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            repository.InsertExcursion(excursion);
        //            repository.Save();
        //        }
        //    }
        //    catch (RetryLimitExceededException)
        //    {
        //        ModelState.AddModelError("",
        //            "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
        //    }

        //    return Json(excursion);
        //}
       
        //public ActionResult GetExcursions()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        if (User.IsInRole("Guide"))
        //        {
        //            ViewBag.ButtonInMenu = "Create excursion";
        //            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
        //            return View("GuideView", repository.GetExcursionsByGuideId(user.Id));
        //        }
        //        else if (User.IsInRole("Traveller"))
        //        {
        //            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;                
        //            return View("TravellerView", user.Excursions.ToList());
        //        }
        //        else
        //        {
        //            return View("AdminView", repository.GetExcursions());
        //        }
        //    }
        //    else
        //    {
        //        return View("GuestPageView", repository.GetExcursions());
        //    }
        //}

        [AllowAnonymous]
        public ActionResult GetAllExcursionsForGuest()
        {
            return View("AllExcursionsForGuest", excursionRepository.GetExcursions().ToList());
        }

        [Authorize(Roles = "Guide, Traveller")]
        public ActionResult GetAllExcursions()
        {
            if (User.IsInRole("Traveller"))
            {
                List<ExcursionForTraveller> excursionsToDisplay = new List<ExcursionForTraveller>();
                foreach (var excursion in excursionRepository.GetExcursions())
                {
                    if (excursion.Guide == User.Identity.GetUserId())
                    {
                        excursionsToDisplay.Add(new ExcursionForTraveller()
                        {
                            Excursion = excursion,
                            CouldBeSubscribed = false,
                            ReasonForSubscribingDisability = "It is your excursion"
                        });
                    }
                    else if (excursion.Users.Count == excursion.PeopleLimit)
                    {
                        excursionsToDisplay.Add(new ExcursionForTraveller()
                        {
                            Excursion = excursion,
                            CouldBeSubscribed = false,
                            ReasonForSubscribingDisability = "There are no free places"
                        });
                    }
                    else if (excursion.Users.FirstOrDefault(x => x.Id == User.Identity.GetUserId()) != null)
                    {
                        excursionsToDisplay.Add(new ExcursionForTraveller()
                        {
                            Excursion = excursion,
                            CouldBeSubscribed = false,
                            ReasonForSubscribingDisability = "You have already subcribed"
                        });
                    }
                    else
                    {
                        excursionsToDisplay.Add(new ExcursionForTraveller()
                        {
                            Excursion = excursion,
                            CouldBeSubscribed = true
                        });
                    }
                }
                                         
                return View("AllExcursionsForTraveller", excursionsToDisplay);
            }
            else
            {
                return View("AllExcursionsForGuide", excursionRepository.GetExcursions().ToList());
            }
        }


        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        //[HttpPost]
        //[Authorize(Roles = "Guide")]
        //public ActionResult CreateExcursion(ExcursionCreationModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        string id = User.Identity.GetUserId();
        //        Excursion excursion = new Excursion()
        //        {
        //            City = model.City,
        //            Date = model.Date,
        //            Description = model.Description,
        //            Duration = model.Duration,
        //            PeopleLimit = model.PeopleLimit,
        //            Route = model.Route,
        //            Guide = id
        //        };
        //        repository.InsertExcursion(excursion);
        //        repository.Save();
        //        //var user = UserManager.FindByIdAsync(User.Identity.GetUserId()).Result;
        //        //user.OwnedExcursions.Add(excursion);
        //        //var result = await UserManager.UpdateAsync(user);

        //        //if (!result.Succeeded)
        //        //{
        //        //    AddErrors(result);
        //        //}

        //        return GetExcursions();
        //    }

        //    return View("_PartialCreateExcursionView");
        //}

        //[HttpGet]
        //public ViewResult CreateExcursion()
        //{
        // return View("_PartialCreateExcursionView");   
        //}

        private void AddErrors(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        //public ActionResult Subscribe(Excursion excursion)
        //{
        //    var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
        //    user.Excursions.Add(excursion);
        //    //return ShowSubscribedExcursions();
        //    return View("AllExcursions");
        //}
    }

}
