using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models.Identity
{
    public enum UserRoles { Admin, Professor }
    public class EasyTagUser:IdentityUser
    {
        public EasyTagUser(string userName) : base(userName)
        {
        }

        public EasyTagUser(){}

        [Required]
        public UserRoles? Role { get; set; }
        public override string ToString()
        {
            return $"{UserName}{this.PhoneNumber}";
        }
    }
}
