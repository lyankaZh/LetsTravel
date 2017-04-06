using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Concrete;
using Domain.Entities;
using LetsTravel.Models;

namespace LetsTravel.Controllers
{
    public class ExcursionController : Controller
    {
        private readonly TravelDbContext context;

        public ExcursionController()
        {
            context = new TravelDbContext();
        }

        public string Index()
        {
            using (var ctx = new TravelDbContext())
            {
                User user = new User()
                {
                    FirstName = "Ira",
                    LastName = "Bokalo",
                    Email = "irabokalo@gmail.com",
                    PhoneNumber = "068258741422",
                    IsAdmin = true,
                };
                ctx.Users.Add(user);
                ctx.SaveChanges();
            }
            return "Hello";
        }



        [HttpPost]
        public ActionResult AddExcursion(Excursion excursion)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    context.Excursions.Add(excursion);
                    context.SaveChanges();
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("",
                    "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }   
             
            return Json(excursion);
    }

    // GET: Excursions
    public ActionResult GetExcursions()
    {
        //return Json(context.Excursions.ToList(), JsonRequestBehavior.AllowGet);
        return View(context.Excursions.ToList());
    }
}
}
