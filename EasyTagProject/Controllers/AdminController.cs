using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EasyTagProject.Models.Identity;
using EasyTagProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyTagProject.Controllers
{
    //[Authorize(Roles = nameof(UserRoles.Admin))]
    [Authorize(Roles = "Admin,Professor")]
    public class AdminController : Controller
    {
        private UserManager<EasyTagUser> userManager;
        public AdminController(UserManager<EasyTagUser> manager)
        {
            userManager = manager;
        }
        [Authorize(Roles = nameof(UserRoles.Admin))]
        public async Task<IActionResult> ManageUsers()
        {
            return View(await userManager.Users.Where(u => u.UserName != "Admin").ToListAsync());
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpGet]
        public async Task<ViewResult> Create()
        {
            return View(new CreateUserViewModel() { Editing = false});
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (model.Password != model.RepeatPass)
            {
                ModelState.AddModelError("", "Password does not match!!");
            }

            if (ModelState.IsValid)
            {
                EasyTagUser user = new EasyTagUser
                {
                    UserName = model.UserName,
                    Email = model.Email.ToLower(),
                    PhoneNumber = model.PhoneNumber,
                    Role = model.Role
                };

                IdentityResult result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, model.Role.ToString());
                    return RedirectToAction(nameof(ManageUsers));
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }

            return View(model);
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            EasyTagUser user = await userManager.FindByNameAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ManageUsers));
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User not found!");
            }
            return RedirectToAction(nameof(ManageUsers));
        }

        //[Authorize(Roles = "Admin,Professor")]
        [HttpGet]
        public async Task<ViewResult> Edit(string id, string returnUrl)
        {
            EasyTagUser tagUser = await userManager.FindByIdAsync(id);

            if (tagUser != null)
            {
                CreateUserViewModel user = new CreateUserViewModel
                {
                    Email = tagUser.Email,
                    UserName = tagUser.UserName,
                    PhoneNumber = tagUser.PhoneNumber,
                    Role = tagUser.Role,
                    Id = tagUser.Id,
                    Editing = true
                };

                if (!String.IsNullOrEmpty(returnUrl))
                {
                    user.ReturnUrl = returnUrl;
                }

                return View(user);
            }
            else
            {
                ModelState.AddModelError("", "User not found!!");
            }       

            return View(nameof(ManageUsers), await userManager.Users.Where(u => u.UserName != "Admin").ToListAsync());
        }

        //[Authorize(Roles = "Admin,Professor")]
        [HttpPost]
        public async Task<IActionResult> Edit(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                EasyTagUser tagUser = await userManager.FindByIdAsync(model.Id);
                if (tagUser != null)
                {
                    if (tagUser.Role != model.Role)
                    {
                        UserRoles? oldRole = tagUser.Role;
                        await userManager.RemoveFromRoleAsync(tagUser, oldRole.ToString());
                        tagUser.Role = model.Role;
                        var changeRoleResult = await userManager.AddToRoleAsync(tagUser, model.Role.ToString());
                        if (!changeRoleResult.Succeeded)
                        {
                            tagUser.Role = oldRole;
                            ModelState.AddModelError("", $"Unexpected error occurred setting role for user with ID {tagUser.Id}");
                            AddErrorsFromResult(changeRoleResult);
                        }
                    }

                    if (tagUser.Email != model.Email)
                    {
                        //var emailToken = await userManager.GenerateChangeEmailTokenAsync(tagUser, model.Email);
                        var changeEmailResult = await userManager.SetEmailAsync(tagUser, model.Email.ToLower());
                        if (!changeEmailResult.Succeeded)
                        {
                            ModelState.AddModelError("", $"Unexpected error occurred setting email for user with ID {tagUser.Id}");
                            AddErrorsFromResult(changeEmailResult);
                        }
                    }

                    if (tagUser.PhoneNumber != model.PhoneNumber)
                    {
                        //var phoneToken = await userManager.GenerateChangePhoneNumberTokenAsync(tagUser, model.PhoneNumber);
                        var changePhoneNumberResult = await userManager.SetPhoneNumberAsync(tagUser, model.PhoneNumber);
                        if (!changePhoneNumberResult.Succeeded)
                        {
                            ModelState.AddModelError("", $"Unexpected error occurred setting phone number for user with ID {tagUser.Id}");
                            AddErrorsFromResult(changePhoneNumberResult);
                        }
                    }

                    if (tagUser.UserName != model.UserName)
                    {
                        var changeUserNameResult = await userManager.SetUserNameAsync(tagUser, model.UserName);
                        if (!changeUserNameResult.Succeeded)
                        {
                            ModelState.AddModelError("", $"Unexpected error occurred setting user name for user with ID {tagUser.Id}");
                            AddErrorsFromResult(changeUserNameResult);
                        }
                    }

                    var updateResult = await userManager.UpdateAsync(tagUser);
                    if (!updateResult.Succeeded)
                    {
                        ModelState.AddModelError("", $"Unexpected error occurred updating user with ID {tagUser.Id}");
                        AddErrorsFromResult(updateResult);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User not found!");

                    return View(model);
                }

                if (ModelState.IsValid)
                {
                    if (!String.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction(nameof(ManageUsers));
                } 
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(CreateUserViewModel model)
        {
            EasyTagUser tagUser = await userManager.FindByIdAsync(model.Id);
            if (tagUser != null)
            {
                if (String.IsNullOrEmpty(model.OldPassword))
                {
                    ModelState.AddModelError("OldPassword", "Please type your corrent password!");
                }
                if (String.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError("Password", "Please type a valid password!");
                }
                if (String.IsNullOrEmpty(model.RepeatPass))
                {
                    ModelState.AddModelError("RepeatPass", "Please repeat the new password!");
                }
                if (model.Password != model.RepeatPass)
                {
                    ModelState.AddModelError("", "Password does not match!!");
                }

                var correctPasswordResult = userManager.PasswordHasher.VerifyHashedPassword(tagUser, tagUser.PasswordHash, model.OldPassword);
                if (correctPasswordResult == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError("", "Incorrect Password!");
                }
                if (ModelState.IsValid)
                {
                    if (model.OldPassword != model.Password)
                    {
                        TempData["PasswordChanged"] = "Password changed successfully";
                        var changePasswordResult = await userManager.ChangePasswordAsync(tagUser, model.OldPassword, model.Password);
                        if (!changePasswordResult.Succeeded)
                        {
                            TempData["PasswordChanged"] = "";
                            AddErrorsFromResult(changePasswordResult);
                        }
                        
                    }
                } 
            }
            return View(nameof(Edit), model);
        }

        //public async Task<ViewResult> ChangePasswordRedirect(CreateUserViewModel model)
        //{
        //    return View(nameof(Edit), model);
        //}


        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }


        //public async Task<IdentityUser> GetCurrentUserAsync()
        //{
        //    return await userManager.GetUserAsync(HttpContext.User);
        //}
    }
}