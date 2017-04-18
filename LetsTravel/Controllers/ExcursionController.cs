using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Concrete;
using Domain.Entities;

namespace LetsTravel.Controllers
{
    [Authorize]
    public class ExcursionController : Controller
    {
        private readonly IExcursionRepository repository;

        public ExcursionController(IExcursionRepository repository)
        {
            this.repository = repository;
        }

        public string Index()
        {
            return "Hello";
        }

     
        [HttpPost]
        public ActionResult AddExcursion(Excursion excursion)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repository.InsertExcursion(excursion);
                    repository.Save();
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
            return View(repository.GetExcursions());
        }
    }
}
