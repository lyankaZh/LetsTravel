using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using System.Web;
using LetsTravel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace LetsTravel.Controllers
{

    public class ExcursionController : Controller
    {
        private readonly IExcursionRepository repository;

        public ExcursionController(IExcursionRepository repository)
        {
            this.repository = repository;
        }

        //public string Index()
        //{
        //    return "Hello";
        //}

     
        //[HttpPost]
        //public ActionResult AddExcursion(Excursion excursion)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            repository.InsertExcursion(excursion);
        //            repository.Save();
        //        }
        //    }
        //    catch (RetryLimitExceededException)
        //    {
        //        ModelState.AddModelError("",
        //            "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
        //    }

        //    return Json(excursion);
        //}
       
        public ActionResult GetExcursions()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Guide"))
                {
                    ViewBag.ButtonInMenu = "Create excursion";
                    var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
                    return View("GuideView", repository.GetExcursionsByGuideId(user.Id));
                }
                else if (User.IsInRole("Traveller"))
                {
                    return View("TravellerView", repository.GetExcursions());
                }
                else
                {
                    return View("AdminView", repository.GetExcursions());
                }
            }
            else
            {
                return View("GuestPageView", repository.GetExcursions());
            }
        }

        [Authorize]
        public ActionResult GetAllExcursions()
        {
            return View("AllExcursions", repository.GetExcursions().ToList());
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Guide")]
        public ActionResult CreateExcursion(ExcursionCreationModel model)
        {
            if (ModelState.IsValid)
            {
                string id = User.Identity.GetUserId();
                Excursion excursion = new Excursion()
                {
                    City = model.City,
                    Date = model.Date,
                    Description = model.Description,
                    Duration = model.Duration,
                    PeopleLimit = model.PeopleLimit,
                    Route = model.Route,
                    Guide = id
                };
                repository.InsertExcursion(excursion);
                repository.Save();
                //var user = UserManager.FindByIdAsync(User.Identity.GetUserId()).Result;
                //user.OwnedExcursions.Add(excursion);
                //var result = await UserManager.UpdateAsync(user);

                //if (!result.Succeeded)
                //{
                //    AddErrors(result);
                //}

                return GetExcursions();
            }

            return View("_PartialCreateExcursionView");
        }

        [HttpGet]
        public ViewResult CreateExcursion()
        {
         return View("_PartialCreateExcursionView");   
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }

}
