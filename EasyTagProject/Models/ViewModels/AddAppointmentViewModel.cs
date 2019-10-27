using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentDate;

namespace EasyTagProject.Models.ViewModels
{
    public class AddAppointmentViewModel
    {
        public DateTime StartRange { get; set; } = DateTime.Parse("08:30:00");
        public DateTime EndRange { get; set; } = DateTime.Parse("22:30:00");
        public DateTime Date { get; set; }
        public Room Room { get; set; }
    }
}
