using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public Room Room { get; set; }

    }
}
