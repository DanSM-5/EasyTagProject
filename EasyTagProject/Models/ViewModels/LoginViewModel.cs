using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EasyTagProject.Models.ViewModels
{
    // Login ViewModel
    public class LoginViewModel
    {
        [Required]
        public string Name { get; set; }
        public int UrlLength { get; set; }
        [Required]
        [UIHint("password")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; } = "/";
    }
}
