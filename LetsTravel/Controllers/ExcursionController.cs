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
    public class ExcursionController : Controller
    {
        private readonly TravelDbContext context;

        public ExcursionController()
        {
            context = new TravelDbContext();
        }

        public string Index()
        {
            //using (var ctx = new TravelDbContext())
            //{
            //    User user = new User()
            //    {
            //        FirstName = "Ira",
            //        LastName = "Bokalo",
            //        Email = "irabokalo@gmail.com",
            //        PhoneNumber = "068258741422",
            //        IsAdmin = true,
            //    };
            //    ctx.Users.Add(user);
            //    ctx.SaveChanges();
            //}
            return "Hello";
        }

        [HttpPost]
        public ActionResult AddUser()
        {
           User user = new User()
           { 
               FirstName = "Is",
                  LastName = "Bokalo",
                 Email = "irabokalo@gmail.com",
                  PhoneNumber = "068258741422",
                   IsAdmin = true
               };
          
                try
                {
                    if (ModelState.IsValid)
                    {
                        context.Users.Add(user);
                        context.SaveChanges();
                       
                    }
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.)
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
                return Json(user);
              

            }
        }
    
}