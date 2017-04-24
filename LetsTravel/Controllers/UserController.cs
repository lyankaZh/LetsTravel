using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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
    public class UserController : Controller
    {
        private ExcursionRepository repository;

        public UserController(ExcursionRepository repo)
        {
            repository = repo;
        }

        [HttpPost]
        public ActionResult LoadImage(HttpPostedFileBase image = null)
        {

            if (image != null)
            {
                string userId = User.Identity.GetUserId();
                var user = (User)repository.GetUserById(userId);
                user.ImageMimeType = image.ContentType;
                user.ImageData = new byte[image.ContentLength];
                image.InputStream.Read(user.ImageData, 0, image.ContentLength);
                repository.UpdateUser(user);
                repository.Save();
            }
            return View("/Views/Excursion/AllExcursionsForTraveller.cshtml");
        }

        public FileContentResult GetImage(string userId)
        {
            var user = (User)repository.GetUsers().FirstOrDefault(p => p.Id == userId);
            if (user != null)
            {
                return File(user.ImageData, user.ImageMimeType);
            }
            else
            {
                return null;
            }
        }

        public ActionResult ShowProfile()
        {
            var user = (User)repository.GetUserById(User.Identity.GetUserId());
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

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }
        //private readonly TravelDbContext _context = new TravelDbContext();

        //// GET: User
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //public ActionResult GetUsers()
        //{
        //    //return Json(context.Excursions.ToList(), JsonRequestBehavior.AllowGet);
        //    return View(_context.Users.ToList());
        //}

        //[HttpPost]
        //public ActionResult AddUser(User user)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _context.Users.Add(user);
        //            _context.SaveChanges();
        //        }
        //    }
        //    catch (RetryLimitExceededException)
        //    {               
        //        ModelState.AddModelError("",
        //            "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
        //    }
        //    return Json(user);
        //}
    }
}