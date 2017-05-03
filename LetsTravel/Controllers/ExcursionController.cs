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
        private readonly ITravelRepository repository;

        public ExcursionController(ITravelRepository repository)
        {
            this.repository = repository;
        }

        [AllowAnonymous]
        public ViewResult GetAllExcursionsForGuest()
        {
             List<Excursion> unblockedExcursions = new List<Excursion>();
            foreach (var excursion in repository.GetExcursions())
            {
                if (repository.GetUserById(excursion.Guide).BlockedUser == null)
                {
                    unblockedExcursions.Add(excursion);
                }
            }
            return View("AllExcursionsForGuest", unblockedExcursions);
        }

        [Authorize(Roles = "Guide, Traveller, Admin")]
        public ViewResult GetAllExcursions()
        {
            if (User.IsInRole("Traveller"))
            {
                List<ExcursionForTraveller> excursionsToDisplay = new List<ExcursionForTraveller>();
                foreach (var excursion in repository.GetExcursions())
                {
                    if (repository.GetUserById(excursion.Guide) == null)
                    {
                        if (User.IsInRole("Guide"))
                        {
                            if (excursion.Guide == User.Identity.GetUserId())
                            {
                                excursionsToDisplay.Add(new ExcursionForTraveller()
                                {
                                    ExcursionId = excursion.ExcursionId,
                                    City = excursion.City,
                                    Date = excursion.Date,
                                    Description = excursion.Description,
                                    Duration = excursion.Duration,
                                    Price = (int) excursion.Price,
                                    PeopleLimit = excursion.PeopleLimit,
                                    Route = excursion.Route,
                                    CouldBeSubscribed = false,
                                    ReasonForSubscribingDisability = "It is your excursion"
                                });
                            }
                        }
                        if (excursion.Users.Count == excursion.PeopleLimit)
                        {

                            excursionsToDisplay.Add(new ExcursionForTraveller()
                            {
                                ExcursionId = excursion.ExcursionId,
                                City = excursion.City,
                                Date = excursion.Date,
                                Description = excursion.Description,
                                Duration = excursion.Duration,
                                Price = (int) excursion.Price,
                                PeopleLimit = excursion.PeopleLimit,
                                Route = excursion.Route,
                                CouldBeSubscribed = false,
                                ReasonForSubscribingDisability = "There are no free places",
                            });
                        }
                        else if (excursion.Users.FirstOrDefault(x => x.Id == User.Identity.GetUserId()) != null)
                        {

                            excursionsToDisplay.Add(new ExcursionForTraveller()
                            {
                                ExcursionId = excursion.ExcursionId,
                                City = excursion.City,
                                Date = excursion.Date,
                                Description = excursion.Description,
                                Duration = excursion.Duration,
                                Price = (int) excursion.Price,
                                PeopleLimit = excursion.PeopleLimit,
                                Route = excursion.Route,
                                CouldBeSubscribed = false,
                                ReasonForSubscribingDisability = "You have already subcribed",
                            });
                        }


                        else
                        {

                            excursionsToDisplay.Add(new ExcursionForTraveller()
                            {
                                ExcursionId = excursion.ExcursionId,
                                City = excursion.City,
                                Date = excursion.Date,
                                Description = excursion.Description,
                                Duration = excursion.Duration,
                                Price = (int) excursion.Price,
                                PeopleLimit = excursion.PeopleLimit,
                                Route = excursion.Route,
                                CouldBeSubscribed = true,
                            });
                        }
                    }
                }
            
                if (repository.GetUserById(User.Identity.GetUserId()).BlockedUser == null)
                {
                    return View("AllExcursionsForTraveller", excursionsToDisplay);
                }
                else
                {
                    return View("BlockView", repository.GetBlockedUsers().FirstOrDefault(x => x.User.Id == User.Identity.GetUserId()));
                }
            }
            else if (User.IsInRole("Guide"))
            {
                List<Excursion> unblockedExcursions = new List<Excursion>();
                foreach (var excursion in repository.GetExcursions())
                {
                    if (repository.GetUserById(excursion.Guide).BlockedUser == null)
                    {
                        unblockedExcursions.Add(excursion);
                    }
                }
                return View("AllExcursionsForGuide", unblockedExcursions);
            }
            return View("AllExcursionsForAdmin", repository.GetExcursions().ToList());

        }


        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

    }

}
