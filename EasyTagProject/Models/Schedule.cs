using FluentDate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    /*
     * Model class for Schedule
     * Contains a reference of the room that is attached to and a list of appointments
     * for that room.
     */
    public class Schedule
    {
        [Key]
        public int Id { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        [ForeignKey("Id")]
        public Room Room { get; set; }
        [NotMapped]
        public bool IsBusy => Appointments.Any(a => a.Start < DateTime.Now && a.End > DateTime.Now);

        // Get the appointments in the date provided
        public List<Appointment> GetAppointmentsInDate(DateTime start) =>
            Appointments.Where(a => a.Start > start && a.Start < start.AddDays(1))
                .OrderBy(a => a.Start)
                .ToList();

        public string GetOverlappingAppointmentUserName(DateTime time) => 
            Appointments
                .Where(a =>
                    a.Start.Date == time.Date &&
                    a.Start < (time + 30.Minutes()) && 
                    a.End > time)
                .FirstOrDefault()
                .UserName;

        public string GetOverlappingAppointmentUserId(DateTime time) =>
            Appointments
                .Where(a =>
                    a.Start.Date == time.Date &&
                    a.Start < (time + 30.Minutes()) &&
                    a.End > time)
                .FirstOrDefault()
                .UserId;

        public Appointment GetOverlappingAppointment(DateTime time) =>
            Appointments
                .Where(a =>
                    a.Start.Date == time.Date &&
                    a.Start < (time + 30.Minutes()) &&
                    a.End > time)
                .FirstOrDefault();

        public List<Appointment> GetOverlappingAppointmentsList(DateTime time) =>
            Appointments
                .Where(a =>
                    a.Start.Date == time.Date &&
                    a.Start < (time + 30.Minutes()) &&
                    a.End > time).ToList();

        public bool IsOverlapingInDate(DateTime time) =>
            Appointments
                .Where(a => a.Start.Date == time.Date)
                .Any(a => a.Start.TimeOfDay < (time.TimeOfDay + 30.Minutes()) && a.End.TimeOfDay > time.TimeOfDay);


    }
}
