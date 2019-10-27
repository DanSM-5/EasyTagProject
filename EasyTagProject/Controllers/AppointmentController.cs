using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyTagProject.Models;
using FluentDate;
using Microsoft.AspNetCore.Mvc;

namespace EasyTagProject.Controllers
{
    public class AppointmentController : Controller
    {
        private IAppointmentRepository repository;
        public AppointmentController(IAppointmentRepository repo) => repository = repo;

        [HttpGet("{action}/{name}/{roomId}/{scheduleId}/{selectedTime}")]
        public ViewResult Appointment(string name, int roomId, int scheduleId, DateTime selectedTime)
        {
            return View(new Appointment { RoomName = name, RoomId = roomId, ScheduleId = scheduleId, Start = selectedTime, End = selectedTime + 30.Minutes()});
        }

        [HttpPost("{action}/{name}/{roomId}/{scheduleId}/{selectedTime}")]
        public IActionResult Appointment(Appointment appointment)
        {
            if (appointment.Start > appointment.End)
            {
                ModelState.AddModelError("", "Start time must be before end time");
            }
            IEnumerable<Appointment> appointments = repository.Appointments.Where(a => a.Start.Date == appointment.Start.Date);

            if(appointments.Any(a => a.Start.TimeOfDay <= appointment.End.TimeOfDay 
                                && appointment.Start.TimeOfDay <= a.End.TimeOfDay))
            {
                ModelState.AddModelError("", "The time you selected is already busy!");
            }
            if (ModelState.IsValid)
            {
                repository.Save(appointment);

                return RedirectToAction(nameof(Room),nameof(Room), new { name = appointment.RoomName, pDate = appointment.Start.ToString("MM-dd-yyyy") });
            }

            return View(appointment);
        }

        public IActionResult RedirectToRoom(int id, DateTime date)
        {
            return RedirectToAction("AddAppointment", nameof(Room), new { id = id, searchDate = date.ToString("MM-dd-yyyy")});
        }
    }
}