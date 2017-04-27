using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Concrete;
using Domain.Entities;
using LetsTravel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace LetsTravel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly TravelRepository repository;

        public AdminController(TravelRepository repo)
        {
            repository = repo;
        }

        public ActionResult ShowUsersForAdmin()
        {
            var users = from user in repository.GetUsers() select (User)user;
            var usersToDisplay = new List<User>();
            foreach (var user in users)
            {
                usersToDisplay.Add(user);
            }
            return View("Users",usersToDisplay);
        }

        public ActionResult ShowExcursionsForAdmin()
        {
            var excursions = from exc in repository.GetExcursions() select exc;
            var excursionsToDisplay = new List<ExcursionModelForAdmin>();
            foreach (var exc in excursions)
            {
                var guide = (User)repository.GetUserById(exc.Guide);
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
            User user = (User)repository.GetUserById(userId);
            if (user != null)
            {
                if (UserManager.IsInRole(user.Id, "Guide"))
                {
                    var excursionsOfGuide = repository.GetExcursionsByGuideId(user.Id);
                    foreach (var excursion in excursionsOfGuide)
                    {
                        foreach (var u in excursion.Users)
                        {
                            u.Excursions.Remove(excursion);
                        }
                        repository.DeleteExcursion(excursion.ExcursionId);
                    }
                }
                if (UserManager.IsInRole(user.Id, "Traveller"))
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

            return ShowUsersForAdmin();
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
            return ShowExcursionsForAdmin();
        }


        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
        //public ActionResult Index()
        //{
        //    return View(UserManager.Users);
        //}

        //public ActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<ActionResult> Delete(string id)
        //{
        //    AppUser user = await UserManager.FindByIdAsync(id);
        //    if (user != null)
        //    {
        //        IdentityResult result = await UserManager.DeleteAsync(user);
        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            return View("Error", result.Errors);
        //        }
        //    }
        //    else
        //    {
        //        return View("Error", new string[] { "User Not Found" });
        //    }
        //}



        //public async Task<ActionResult> Edit(string id)
        //{
        //    AppUser user = await UserManager.FindByIdAsync(id);
        //    if (user != null)
        //    {
        //        return View(user);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index");
        //    }
        //}
        //[HttpPost]
        //public async Task<ActionResult> Edit(string id, string email, string password)
        //{
        //    AppUser user = await UserManager.FindByIdAsync(id);
        //    if (user != null)
        //    {
        //        user.Email = email;
        //        IdentityResult validEmail
        //        = await UserManager.UserValidator.ValidateAsync(user);
        //        if (!validEmail.Succeeded)
        //        {
        //            AddErrorsFromResult(validEmail);
        //        }
        //        IdentityResult validPass = null;
        //        if (password != string.Empty)
        //        {
        //            validPass
        //            = await UserManager.PasswordValidator.ValidateAsync(password);
        //            if (validPass.Succeeded)
        //            {
        //                user.PasswordHash =
        //                UserManager.PasswordHasher.HashPassword(password);
        //            }
        //            else
        //            {
        //                AddErrorsFromResult(validPass);
        //            }
        //        }
        //        if ((validEmail.Succeeded && validPass == null) || (validEmail.Succeeded
        //        && password != string.Empty && validPass.Succeeded))
        //        {
        //            IdentityResult result = await UserManager.UpdateAsync(user);
        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction("Index");
        //            }
        //            else
        //            {
        //                AddErrorsFromResult(result);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "User Not Found");
        //    }
        //    return View(user);
        //}
        //[HttpPost]
        //public async Task<ActionResult> Create(CreateModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        AppUser user = new AppUser { UserName = model.Name, Email = model.Email };
        //        IdentityResult result = await UserManager.CreateAsync(user,
        //        model.Password);

        //    if (result.Succeeded)
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        AddErrorsFromResult(result);
        //    }
        //    return View(model);
        //}
        //private void AddErrorsFromResult(IdentityResult result)
        //{
        //    foreach (string error in result.Errors)
        //    {
        //        ModelState.AddModelError("", error);
        //    }
        //}

        //private AppUserManager UserManager
        //{
        //    get
        //    {
        //        return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
        //    }
        //}
    }
}