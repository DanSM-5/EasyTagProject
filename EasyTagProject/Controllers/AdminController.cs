using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using EasyTagProject.Infrastructure;
using EasyTagProject.Models.Identity;
using EasyTagProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyTagProject.Controllers
{
    [Authorize(Roles = "Admin,Professor")]
    public class AdminController : Controller
    {
        public const string ComplexEmailPattern4 = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" // local-part
                                                  + "@"
                                                  + @"((([\w]+([-\w]*[\w]+)*\.)+[a-zA-Z]+)|" // domain
                                                  + @"((([01]?[0-9]{1,2}|2[0-4][0-9]|25[0-5]).){3}[01]?[0-9]{1,2}|2[0-4][0-9]|25[0-5]))\z";// or IP Address

        public const string phonePattern = @"^\(?([0-9]{3})\)?[-.●\s]?([0-9]{3})[-.●\s]?([0-9]{4})$";
        private UserManager<EasyTagUser> userManager;
        IHttpContextAccessor viewContext;
        public AdminController(UserManager<EasyTagUser> manager, IHttpContextAccessor viewContext)
        {
            userManager = manager;
            this.viewContext = viewContext;
        }

        [Authorize(Roles = nameof(UserRoles.Admin))]
        public async Task<IActionResult> ManageUsers()
        {
            // Return the list of users except Admin
            return View(await userManager.Users.Where(u => u.UserName != UserRoles.Admin.ToString()).ToListAsync());
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

            ValidateFields(model);

            if (ModelState.IsValid)
            {
                EasyTagUser user = new EasyTagUser
                {
                    UserName = model.UserName,
                    Email = model.Email.ToLower(),
                    PhoneNumber = model.PhoneNumber,
                    Role = model.Role,
                    FirstName = model.FirstName,
                    LastName = model.LastName
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

        [HttpGet]
        public async Task<IActionResult> Edit(string id, string returnUrl, string message = "")
        {
            // Validate permission to edit
            if(viewContext.HttpContext.IsAccessibleForUserOrAdmin(id))
            {
                EasyTagUser tagUser = await userManager.FindByIdAsync(id);

                // Admin user cannot be changed
                if (tagUser.UserName != UserRoles.Admin.ToString())
                {
                    if (tagUser != null)
                    {
                        CreateUserViewModel user = new CreateUserViewModel
                        {
                            Email = tagUser.Email,
                            UserName = tagUser.UserName,
                            PhoneNumber = tagUser.PhoneNumber,
                            Role = tagUser.Role,
                            Id = tagUser.Id,
                            FirstName = tagUser.FirstName,
                            LastName = tagUser.LastName,
                            Editing = true
                        };

                        if (!String.IsNullOrEmpty(returnUrl))
                        {
                            user.ReturnUrl = HttpUtility.UrlDecode(returnUrl);
                        }

                        if (message == "success")
                        {
                            TempData["PasswordChanged"] = $"Password changed successfully!";
                        }

                        return View(user);
                    }
                    else
                    {
                        ModelState.AddModelError("", "User not found!!");
                    }

                    // return a list of users except Admin
                    return View(nameof(ManageUsers), await userManager.Users.Where(u => u.UserName != UserRoles.Admin.ToString()).ToListAsync());
                } 
            }
           
            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreateUserViewModel model)
        {
            ValidateFields(model);

            if (ModelState.IsValid)
            {
                // Edit only if user exists
                EasyTagUser tagUser = await userManager.FindByIdAsync(model.Id);
                if (tagUser != null)
                {
                    // Only admin can change roles
                    if (viewContext.HttpContext.User.IsInRole(UserRoles.Admin.ToString()))
                    {
                        // Update only if role changes
                        if (tagUser.Role != model.Role)
                        {
                            // Identify old role
                            UserRoles? oldRole = tagUser.Role;

                            // Remove from old role
                            await userManager.RemoveFromRoleAsync(tagUser, oldRole.ToString());

                            // Assign new role
                            tagUser.Role = model.Role;
                            var changeRoleResult = await userManager.AddToRoleAsync(tagUser, model.Role.ToString());
                            if (!changeRoleResult.Succeeded)
                            {
                                // recover old role if new assignment fails
                                tagUser.Role = oldRole;
                                ModelState.AddModelError("", $"Unexpected error occurred setting role for user with ID {tagUser.Id}");
                                AddErrorsFromResult(changeRoleResult);
                            }
                        } 
                    }
                    
                    // Update only if email changes
                    if (tagUser.Email != model.Email)
                    {
                        var changeEmailResult = await userManager.SetEmailAsync(tagUser, model.Email.ToLower());
                        if (!changeEmailResult.Succeeded)
                        {
                            ModelState.AddModelError("", $"Unexpected error occurred setting email for user with ID {tagUser.Id}");
                            AddErrorsFromResult(changeEmailResult);
                        }
                    }

                    // Update only if phone number changes
                    if (tagUser.PhoneNumber != model.PhoneNumber)
                    {
                        var changePhoneNumberResult = await userManager.SetPhoneNumberAsync(tagUser, model.PhoneNumber);
                        if (!changePhoneNumberResult.Succeeded)
                        {
                            ModelState.AddModelError("", $"Unexpected error occurred setting phone number for user with ID {tagUser.Id}");
                            AddErrorsFromResult(changePhoneNumberResult);
                        }
                    }

                    // Update only if user name changes
                    if (tagUser.UserName != model.UserName)
                    {
                        var changeUserNameResult = await userManager.SetUserNameAsync(tagUser, model.UserName);
                        if (!changeUserNameResult.Succeeded)
                        {
                            ModelState.AddModelError("", $"Unexpected error occurred setting user name for user with ID {tagUser.Id}");
                            AddErrorsFromResult(changeUserNameResult);
                        }
                    }

                    // Update only if first name changes
                    if (tagUser.FirstName != model.FirstName)
                    {
                        tagUser.FirstName = model.FirstName;
                    }

                    // Update only if last name changes
                    if (tagUser.LastName != model.LastName)
                    {
                        tagUser.LastName = model.LastName;
                    }

                    // Update user values
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
                        return Redirect(HttpUtility.UrlDecode(model.ReturnUrl));
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
                    ModelState.AddModelError("OldPassword", "Please type your current password!");
                    return View(nameof(Edit), model);
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
                        var changePasswordResult = await userManager.ChangePasswordAsync(tagUser, model.OldPassword, model.Password);
                        if (!changePasswordResult.Succeeded)
                        {
                            AddErrorsFromResult(changePasswordResult);
                        }
                        else
                        {
                            return RedirectToAction(nameof(Edit), new { id = tagUser.Id, returnUrl = "/", message = "success" });
                        }
                    }
                } 
            }
            return View(nameof(Edit), model);
        }

        // Admin users can change password without old password
        [HttpPost]
        public async Task<IActionResult> ChangePasswordAdmin(CreateUserViewModel model)
        {
            EasyTagUser tagUser = await userManager.FindByIdAsync(model.Id);
            if (tagUser != null)
            {
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

                if (ModelState.IsValid)
                {
                    TempData["PasswordFlagSuccess"] = "Success";

                    var token = await userManager.GeneratePasswordResetTokenAsync(tagUser);

                    var changePasswordResult = await userManager.ResetPasswordAsync(tagUser, token, model.Password);

                    if (!changePasswordResult.Succeeded)
                    {
                        TempData["PasswordFlagSuccess"] = null;
                        AddErrorsFromResult(changePasswordResult);
                    }
                    else
                    {
                        return RedirectToAction(nameof(Edit), new { id = tagUser.Id, returnUrl = "/", message = "success" });
                    }
                }
            }
            return View(nameof(Edit), model);
        }

        // Validate Email and Phone number
        private void ValidateFields(CreateUserViewModel model)
        {
            Regex emailRegex = new Regex(ComplexEmailPattern4);
            if (!emailRegex.IsMatch(model.Email))
            {
                ModelState.AddModelError("Email", "Invalid email address");
            }

            Regex phoneRegex = new Regex(phonePattern);
            if (!phoneRegex.IsMatch(model.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Invalid phone number");
            }
        }

        // Add all errors to model state
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}