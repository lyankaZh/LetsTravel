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
                    if (repository.GetUserById(excursion.Guide).BlockedUser == null)
                    {
                        ExcursionForTraveller excursionForTraveller = new ExcursionForTraveller
                        {
                            ExcursionId = excursion.ExcursionId,
                            City = excursion.City,
                            Date = excursion.Date,
                            Description = excursion.Description,
                            Duration = excursion.Duration,
                            Price = (int)excursion.Price,
                            PeopleLimit = excursion.PeopleLimit,
                            Route = excursion.Route
                        };

                        if (User.IsInRole("Guide") && excursion.Guide == User.Identity.GetUserId())
                        {

                            excursionForTraveller.CouldBeSubscribed = false;
                            excursionForTraveller.ReasonForSubscribingDisability = "It is your excursion";
                            excursionsToDisplay.Add(excursionForTraveller);

                        }
                        else if (excursion.Users.FirstOrDefault(x => x.Id == User.Identity.GetUserId()) != null)
                        {
                            excursionForTraveller.CouldBeSubscribed = false;
                            excursionForTraveller.ReasonForSubscribingDisability = "You have already subcribed";
                            excursionsToDisplay.Add(excursionForTraveller);
                        }
                        else if (excursion.Users.Count == excursion.PeopleLimit)
                        {
                            excursionForTraveller.CouldBeSubscribed = false;
                            excursionForTraveller.ReasonForSubscribingDisability = "There are no free places";
                            excursionsToDisplay.Add(excursionForTraveller);
                        }
                        else
                        {
                            excursionForTraveller.CouldBeSubscribed = true;
                            excursionsToDisplay.Add(excursionForTraveller);
                        }
                    }
                }
                //тобто для юзера всі незаблоковані екскурсії
                //а для заблокованого юзера не виводять жодні екскурсії, а BlockView
                if (repository.GetUserById(User.Identity.GetUserId()).BlockedUser == null)
                {
                    return View("AllExcursionsForTraveller", excursionsToDisplay);
                }
                else
                {
                    return View("BlockView", repository.GetBlockedUsers().FirstOrDefault(x => x.User.Id == User.Identity.GetUserId()));
                }
            }

            //для заблокованого гіда не відображаються усі екскурсії
            else if (User.IsInRole("Guide"))
            {
                if (repository.GetUserById(User.Identity.GetUserId()).BlockedUser == null)
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
                else
                {
                    return View("BlockView", repository.GetBlockedUsers().FirstOrDefault(x => x.User.Id == User.Identity.GetUserId()));
                }
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
