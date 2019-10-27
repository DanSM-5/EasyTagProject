using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models.ViewModels
{
    public class AppointmentViewModel
    {
        public string Date { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        public string UserName { get; set; }
        public string Course { get; set; }
        public string Description { get; set; }
        public int ScheduleId { get; set; }
    }
}
