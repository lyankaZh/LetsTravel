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
                returnUrl = "/Excursion/GetExcursions";
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
                returnUrl = "/Excursion/GetExcursions";
            }
            if (ModelState.IsValid)
            {
                User user = await UserManager.FindByNameAsync(model.Nickname);
                if (user == null)
                {
                    IdentityResult creationResult = await UserManager.CreateAsync(
                        new User  { UserName = model.Nickname, FirstName = model.FirstName,
                                    LastName = model.LastName, Email = model.Email},
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

                IdentityResult result = await UserManager.AddToRoleAsync(user.Id, model.Role);
                if (result.Succeeded)
                {
                    return new RedirectResult(returnUrl);
                }
                else
                {
                    return View("Error", result.Errors);
                }
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
            return new RedirectResult("/Account/Login");
        }

        private IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }
    }
}