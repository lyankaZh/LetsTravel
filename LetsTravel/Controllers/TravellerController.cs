using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using Domain.Abstract;
using LetsTravel.Models;
using Microsoft.AspNet.Identity;

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

        public ViewResult ShowSubscribedExcursions()
        {
            //var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            if (repository.GetUserById(User.Identity.GetUserId()).BlockedUser != null)
            {
                return View("BlockView",
                    repository.GetBlockedUsers().FirstOrDefault(x => x.User.Id == User.Identity.GetUserId()));
            }

            var user = repository.GetUserById(User.Identity.GetUserId());

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
                excursionToDisplay.ModalId = "#" + excursion.ExcursionId;
                excursionToDisplay.Guide = repository.GetUsers().First(u => u.Id == excursion.Guide);
                subscribedExcursions.Add(excursionToDisplay);
            }
            if (subscribedExcursions.Count == 0)
            {
                ViewBag.NoExcursions = "You haven't subscribed to any excursion yet";
            }

            return View("TravellerView", subscribedExcursions);
        }

        public RedirectToRouteResult Subscribe(ExcursionForTraveller model)
        {
            var user = repository.GetUserById(User.Identity.GetUserId());
            var excursion = repository.GetExcursionById(model.ExcursionId);
            user.Excursions.Add(excursion);
            repository.UpdateUser(user);
            excursion.Users.Add(user);
            repository.UpdateExcursion(excursion);
            repository.Save();
            return RedirectToAction("ShowSubscribedExcursions");
        }

        public RedirectToRouteResult UnSubscribe(ExcursionWithGuideInfoViewModel excursion)
        {
            var user = repository.GetUserById(User.Identity.GetUserId());
            var excursionToDelete = user.Excursions.Find(x => x.ExcursionId == excursion.ExcursionId);
            user.Excursions.Remove(excursionToDelete);
            repository.UpdateUser(user);
            repository.Save();
            return RedirectToAction("ShowSubscribedExcursions");
        }
    }
}