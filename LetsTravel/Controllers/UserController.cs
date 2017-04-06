using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Concrete;
using Domain.Entities;

namespace LetsTravel.Controllers
{
    public class UserController : Controller
    {
        private readonly TravelDbContext context = new TravelDbContext();

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetUsers()
        {
            //return Json(context.Excursions.ToList(), JsonRequestBehavior.AllowGet);
            return View(context.Users.ToList());
        }

        [HttpPost]
        public ActionResult AddUser(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    context.Users.Add(user);
                    context.SaveChanges();
                }
            }
            catch (RetryLimitExceededException)
            {               
                ModelState.AddModelError("",
                    "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return Json(user);
        }
    }
}