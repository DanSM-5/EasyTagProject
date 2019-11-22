using EasyTagProject.Models.Identity;
using ExpressiveAnnotations.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models.ViewModels
{
    // ViewModel to generate Create View
    public class CreateUserViewModel
    {
        public bool? Editing { get; set; } = false;
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public UserRoles? Role { get; set; }
        [UIHint("password")]
        [RequiredIf("Editing == false", ErrorMessage = "Password is mandatory!")]
        public string Password { get; set; }
        [UIHint("password")]
        [Display(Name ="Repeat Password")]
        [RequiredIf("Editing == false", ErrorMessage = "Please repeat your password")]
        public string RepeatPass { get; set; }
        public string Id { get; set; }
        [UIHint("password")]
        [Display(Name ="Current Password")]
        public string OldPassword { get; set; }
        public string ReturnUrl { get; set; }
    }
}
