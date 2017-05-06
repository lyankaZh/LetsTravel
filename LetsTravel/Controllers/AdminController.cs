using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using LetsTravel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace LetsTravel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ITravelRepository repository;

        public AdminController(ITravelRepository repo)
        {
            repository = repo;
        }

        public ViewResult ShowUsersForAdmin()
        {
            List<UserForAdminViewModel> usersToDisplay = new List<UserForAdminViewModel>();
            foreach (var user in repository.GetUsers())
            {           
                usersToDisplay.Add(
                    new UserForAdminViewModel
                    {
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Id = user.Id,
                        Nickname = user.UserName,
                        CollapseId = "#"+user.Id,
                        IsBlocked = repository.GetBlockedUsers().FirstOrDefault(x => x.User.Id == user.Id) != null
            });
            }
            return View("Users",usersToDisplay);
        }

        public ViewResult ShowExcursionsForAdmin()
        {
            var excursions = from exc in repository.GetExcursions() select exc;
            var excursionsToDisplay = new List<ExcursionModelForAdmin>();
            foreach (var exc in excursions)
            {
                var guide = repository.GetUserById(exc.Guide);
                excursionsToDisplay.Add(
                    new ExcursionModelForAdmin()
                    {
                       ExcursionId = exc.ExcursionId,
                       City = exc.City,
                       Date = exc.Date,
                       Price = exc.Price,
                       GuideNickname = guide.UserName,
                       GuideFirstName = guide.FirstName,
                       GuideLastName = guide.LastName
                    });
            }
            return View("Excursions", excursionsToDisplay);
        }


        [HttpPost]
        public ActionResult DeleteUser(string userId)
        {
            User user = repository.GetUserById(userId);
            if (user != null)
            {
                if (repository.IsInRole("Guide", user))
                {
                    var excursionsOfGuide = repository.GetExcursions().Where(x => x.Guide == user.Id);
                    foreach (var excursion in excursionsOfGuide)
                    {
                        foreach (var u in excursion.Users)
                        {
                            u.Excursions.Remove(excursion);
                        }
                        repository.DeleteExcursion(excursion.ExcursionId);
                    }
                }
                if (repository.IsInRole("Traveller", user))
                {
                    foreach (var excursion in user.Excursions)
                    {
                        excursion.Users.Remove(user);
                    }
                    user.Excursions.Clear();
                }
                repository.DeleteUser(userId);
                repository.Save();
                TempData["userDeleted"] = "User has been deleted";
            }

            return RedirectToAction("ShowUsersForAdmin");
        }

        [HttpPost]
        public ActionResult DeleteExcursion(int excursionId)
        {
            var excursion = repository.GetExcursionById(excursionId);
            if (excursion != null)
            {
                foreach (var user in excursion.Users)
                {
                    user.Excursions.Remove(excursion);
                }
                repository.DeleteExcursion(excursionId);
                repository.Save();
                TempData["excursionDeleted"] = "Excursion has been deleted";
            }
            return RedirectToAction("ShowExcursionsForAdmin");
        }

        [HttpPost]
        public ActionResult Block(UserForAdminViewModel user)
        {
            if (ModelState.IsValid)
            {
                var u = repository.GetUserById(user.Id);
                repository.InsertBlockedUser(
                    new BlockedUser
                    {
                        User = u,
                        Reason = user.Reason
                    });
                repository.Save();
                TempData["userDeleted"] = "User has been blocked";
            }
            else
            {
                TempData["userDeleted"] = "Specify reason for blocking";
            }
            return RedirectToAction("ShowUsersForAdmin");
        }

        public ActionResult Unblock(UserForAdminViewModel user)
        {
            var userToDelete = repository.GetBlockedUsers().FirstOrDefault(x => x.User.Id == user.Id);
            if (userToDelete != null)
            {
                repository.DeleteBlockedUser(userToDelete.BlockedUserId);
                repository.Save();
                TempData["userDeleted"] = "User has been unblocked";
            }
            return RedirectToAction("ShowUsersForAdmin");
        }      
    }
}