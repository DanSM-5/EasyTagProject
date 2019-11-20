using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EasyTagProject.Models.ViewModels
{
    public class LoginViewModel
    {
        private string returnUrl = "%2F";

        [Required]
        public string Name { get; set; }

        [Required]
        [UIHint("password")]
        public string Password { get; set; }
        public string ReturnUrl /*{ get; set; } = "/";*/
        {
            get { return HttpUtility.UrlDecode(returnUrl); }
            set { returnUrl = HttpUtility.UrlEncode(value); }
        }
    }
}
