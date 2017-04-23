using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Domain.Entities;
using LetsTravel.Identity;
using LetsTravel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace LetsTravel.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
        {
            
        }
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel details, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                //returnUrl = "/Excursion/GetExcursions";
                returnUrl = "/Home/Index";
            }
            if (ModelState.IsValid)
            {
                User user = await UserManager.FindAsync(details.Nickname, details.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid name or password.");
                }
                else
                {
                    ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user,
                    DefaultAuthenticationTypes.ApplicationCookie);
                    AuthManager.SignOut();
                    AuthManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = false
                    }, ident);
                    return Redirect(returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(details);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterModel model, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "/Account/Login";
            }
            if (ModelState.IsValid)
            {
                if (!model.IsTraveller && !model.IsGuide)
                {
                    ModelState.AddModelError("", "Select at least one role.");
                    return View(model);
                }
                User user = await UserManager.FindByNameAsync(model.Nickname);
                if (user == null)
                {
                    IdentityResult creationResult = await UserManager.CreateAsync(
                        new User
                        {
                            UserName = model.Nickname,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email
                        },
                        model.Password);
                    if (creationResult.Succeeded)
                    {
                        user = await UserManager.FindByNameAsync(model.Nickname);
                    }
                    else
                    {
                        AddErrorsFromResult(creationResult);
                        return View(model);
                    }
                }
                if (model.IsGuide)
                {
                    IdentityResult result = await UserManager.AddToRoleAsync(user.Id, "Guide");
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
                if (model.IsTraveller)
                {
                    IdentityResult result = await UserManager.AddToRoleAsync(user.Id, "Traveller");
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
                return new RedirectResult(returnUrl);
            }
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public RedirectResult LogOut()
        {
            AuthManager.SignOut();
            return new RedirectResult("/Home/Index");
        }

        private IAuthenticationManager AuthManager => HttpContext.GetOwinContext().Authentication;

        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
    }
}