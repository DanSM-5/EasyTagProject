using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models.Identity
{
    // Class that represents the custom IdentityUser for EasyTag
    public class EasyTagUser:IdentityUser, IComparable<EasyTagUser>
    {
        public EasyTagUser(string userName) : base(userName){}

        public EasyTagUser(){}

        [Required]
        public UserRoles? Role { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        public override string ToString()
        {
            return $"{UserName}{this.PhoneNumber}";
        }

        public int CompareTo([AllowNull] EasyTagUser other)
        {
            return this.UserName.CompareTo(other.UserName);
        }
    }
}
