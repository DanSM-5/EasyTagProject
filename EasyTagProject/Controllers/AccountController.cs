using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using EasyTagProject.Infrastructure;
using EasyTagProject.Models.Identity;
using EasyTagProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyTagProject.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<EasyTagUser> userManager;
        private SignInManager<EasyTagUser> signInManager;

        public AccountController(UserManager<EasyTagUser> userMgr, SignInManager<EasyTagUser> signInMgr)
        {
            userManager = userMgr;
            signInManager = signInMgr;
        }

        [HttpGet("{controller}/{action}/{returnUrl?}")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = "/")
        {
            if (returnUrl.Contains("Login"))
            {
                returnUrl = "/";
            }
            if (String.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "/";
            }

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost("{controller}/{action}/{returnUrl?}")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                EasyTagUser user = await userManager.FindByNameAsync(model.Name);

                if (user == null)
                {
                    user = await userManager.FindByEmailAsync(model.Name);
                }

                if (user != null)
                {
                    if ((await signInManager.PasswordSignInAsync(user, model.Password, false, false)).Succeeded)
                    {
                        if (model.ReturnUrl == "/Account/AccessDenied" || String.IsNullOrEmpty(model.ReturnUrl)
                            || model.ReturnUrl.Contains("Login"))
                        {
                            return Redirect("/");
                        }

                        return RedirectToAction(nameof(ReturnToPage),new { returnUrl = model.ReturnUrl});
                    }
                }
            }

            ModelState.AddModelError("", "Invalid name of password");
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ReturnToPage(string returnUrl)
        {
            // Get the string back to original state

            returnUrl = HttpUtility.UrlDecode(returnUrl);

            // Redirect to page
            if (String.IsNullOrEmpty(returnUrl))
            {
                return LocalRedirect("~/");
            }
            else
            {
                return LocalRedirect(HttpUtility.UrlDecode(returnUrl));
            }
        }

        [HttpGet("{controller}/{action}/{returnUrl?}")]
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await signInManager.SignOutAsync();

            if (String.IsNullOrEmpty(returnUrl))
            {
                return Redirect("/");
            }
            else if (returnUrl.Contains("ManageUsers"))
            {
                returnUrl = "/";
            }

            return RedirectToAction(nameof(ReturnToPage), new { returnUrl = returnUrl });
        }

        [Authorize(Roles = nameof(UserRoles.Professor))]
        public async Task<ViewResult> AccessDenied(string returnUrl) => 
            View(nameof(AccessDenied), returnUrl);
    }
}