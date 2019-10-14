using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public class Appointment
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string UserName { get; set; }
        public string Course { get; set; }
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
    }
}