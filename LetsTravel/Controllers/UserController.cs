using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Concrete;
using Domain.Entities;
using LetsTravel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace LetsTravel.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ITravelRepository repository;

        public UserController(ITravelRepository repo)
        {
            repository = repo;
        }

        [HttpPost]
        public ActionResult LoadImage(HttpPostedFileBase image = null)
        {

            if (image != null)
            {
                string userId = User.Identity.GetUserId();
                var user = repository.GetUserById(userId);
                user.ImageMimeType = image.ContentType;
                user.ImageData = new byte[image.ContentLength];
                image.InputStream.Read(user.ImageData, 0, image.ContentLength);
                repository.UpdateUser(user);
                repository.Save();
            }
            return View("/Views/Excursion/AllExcursionsForTraveller.cshtml");
        }


        public FileContentResult GetImage(string id)
        {
            var user = repository.GetUsers().FirstOrDefault(p => p.Id == id);
            if (user != null)
            {
                return File(user.ImageData, user.ImageMimeType);
            }
            else
            {
                return null;
            }
        }

        public ViewResult ShowProfile()
        {
            var user = repository.GetUserById(User.Identity.GetUserId());
            ProfileModel model = new ProfileModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                AboutMyself = user.AboutMyself,
                ImageData = user.ImageData,
                ImageMimeType = user.ImageMimeType,

            };
            if (User.IsInRole("Traveller"))
            {
                model.SubscribedExcursionsAmount = user.Excursions.Count;
            }
            if (User.IsInRole("Guide"))
            {
                model.OwnedExcursionsAmount = (from u in repository.GetExcursions() where u.Guide == user.Id select u).Count();
            }
            return View("ProfileView", model);
        }

        public ViewResult Edit()
        {
            //User user = UserManager.FindByIdAsync(User.Identity.GetUserId()).Result;
            var user = repository.GetUserById(User.Identity.GetUserId());
            return View("EditProfileView", user);
        }

        [HttpPost]
        public ActionResult Edit(User model, HttpPostedFileBase image = null)
        {
            if (ModelState.IsValid)
            {
                User user = repository.GetUserById(model.Id);
                var amountOfUsersWithSameNick =
                    (from u in repository.GetUsers() where u.UserName == model.UserName && u.UserName!= user.UserName
                     select u).Count();
                if (amountOfUsersWithSameNick >= 1 )
                {
                    ModelState.AddModelError("", "Such nickname already exists");
                    return RedirectToAction("Edit");
                }
                var amountOfUsersWithSameEmail =
                    (from u in repository.GetUsers() where u.Email == model.Email && u.Email != user.Email
                     select u).Count();

                if (amountOfUsersWithSameEmail >= 1)
                {
                    ModelState.AddModelError("", "Such email already exists");
                    return RedirectToAction("Edit");
                }
              
                user.UserName = model.UserName;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                if (!string.IsNullOrEmpty(model.AboutMyself))
                {
                    user.AboutMyself = model.AboutMyself;
                }
                else
                {
                    user.AboutMyself = null;
                }
                if (image != null)
                {
                    user.ImageMimeType = image.ContentType;
                    user.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(user.ImageData, 0, image.ContentLength);
                }
                repository.UpdateUser(user);
                repository.Save();
                return RedirectToAction("ShowProfile");
            }
            return Edit();
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            var user = repository.GetUserById(id);
            if (user != null)
            {
                bool isGuide = User.IsInRole("Guide");
                bool isTraveller = User.IsInRole("Traveller");
                if (isTraveller)
                {
                    //TO DO - unsubscribe only from future excursions
                    if (repository.GetUserById(id).Excursions.Count > 0)
                    {
                        TempData["deleteTravellerErrorMessage"] =
                            "Before deleting profile unsubscribe from all excursions";
                        return RedirectToAction("ShowProfile");
                    }
                }
                if (isGuide)
                {
                    //дозволити видалення коли є екскурсії, на які ніхто не підписався. в такому разі видаляємо і екскурсії
                    //якщо є активні екскурсії, на які хто-небудь підписався - заборонити видаляти
                    var excursionsOfGuideWithDependencies = from exc in repository.GetExcursionsByGuideId(id)
                        where exc.Users.Count > 0
                        select exc;
                    if (excursionsOfGuideWithDependencies.Any())
                    {
                        TempData["deleteGuideErrorMessage"] =
                            "You can`t delete your profile because you have active excursions with subscribers";
                        return RedirectToAction("ShowProfile");
                    }
                    foreach (var excursion in repository.GetExcursionsByGuideId(id))
                    {
                        repository.DeleteExcursion(excursion.ExcursionId);
                    }
                }
                HttpContext.GetOwinContext().Authentication.SignOut();
                repository.DeleteUser(id);
                repository.Save();
            }
            return new RedirectResult("/Home/Index");
        }      
    }
}