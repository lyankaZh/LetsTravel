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