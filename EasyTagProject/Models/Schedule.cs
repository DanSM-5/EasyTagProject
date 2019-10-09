using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public class Schedule
    {
        [Key]
        public int Id { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        [ForeignKey("Room_FK")]
        public Room Room { get; set; }

    }
}
