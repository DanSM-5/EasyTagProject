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
        [ForeignKey("Id")]
        public Room Room { get; set; }
        public bool IsBusy => Appointments.Any(a => a.Start < DateTime.Now && a.End > DateTime.Now);


        public List<Appointment> GetAppointments(DateTime start)
        {
            var schedules = Appointments.Where(a => a.Start > start && a.Start < start.AddDays(1))
                                    .OrderBy(a => a.Start)
                                    .ToList();

            return schedules;
        }
    }
}
