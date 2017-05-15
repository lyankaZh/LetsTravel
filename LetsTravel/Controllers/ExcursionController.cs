using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using LetsTravel.Models;

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
            return View("AllExcursionsForGuest", GetExcursionsOfUnblockedUsers());
        }

        public List<Excursion> GetExcursionsOfUnblockedUsers()
        {
            List<Excursion> unblockedExcursions = new List<Excursion>();
            foreach (var excursion in repository.GetExcursions())
            {
                if (repository.GetUserById(excursion.Guide).BlockedUser == null)
                {
                    unblockedExcursions.Add(excursion);
                }
            }
            return unblockedExcursions;
        }

        [Authorize(Roles = "Guide, Traveller, Admin")]
        public ViewResult GetAllExcursions()
        {
            if (User.IsInRole("Admin"))
            {
                return View("AllExcursionsForAdmin", repository.GetExcursions().ToList());
            }

            if (repository.GetUsers().First(x => x.UserName == User.Identity.Name).BlockedUser != null)
            {
                return View("BlockView", repository.GetBlockedUsers().FirstOrDefault(x => x.User.UserName == User.Identity.Name));
            }

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

                        if (User.IsInRole("Guide")
                            && excursion.Guide == repository.GetUsers().
                            First(x => x.UserName == User.Identity.Name).Id)
                        {
                            excursionForTraveller.CouldBeSubscribed = false;
                            excursionForTraveller.ReasonForSubscribingDisability = "It is your excursion";
                            excursionsToDisplay.Add(excursionForTraveller);
                        }
                        else if (excursion.Users.FirstOrDefault(x => x.UserName == User.Identity.Name) != null)
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

                return View("AllExcursionsForTraveller", excursionsToDisplay);

            }

            else if (User.IsInRole("Guide"))
            {
                return View("AllExcursionsForGuide", GetExcursionsOfUnblockedUsers());
            }
            else
            {
                return View("AllExcursionsForAdmin", repository.GetExcursions().ToList());
            }
        }
    }

}
