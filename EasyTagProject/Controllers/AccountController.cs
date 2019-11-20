using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ViewResult> Login(string returnUrl)
        {
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

                if (user != null)
                {
                    if ((await signInManager.PasswordSignInAsync(user, model.Password, false, false)).Succeeded)
                    {
                        return Redirect(model?.ReturnUrl ?? "/");
                    }
                }
            }

            ModelState.AddModelError("", "Invalid name of password");
            return View(model);
        }

        [HttpGet("{controller}/{action}/{returnUrl?}")]
        public async Task<RedirectResult> Logout(string returnUrl = "/")
        {
            await signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

        [Authorize(Roles = nameof(UserRoles.Professor))]
        public async Task<ViewResult> AccessDenied(string returnUrl) => View(nameof(AccessDenied), returnUrl);
    }
}