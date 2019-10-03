using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public class Schedule
    {
        public ICollection<Appointment> Appointments { get; set; }
        public Room Room { get; set; }

    }
}
