using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyTagProject.Models;
using FluentDate;
using Microsoft.AspNetCore.Mvc;

namespace EasyTagProject.Controllers
{
    public class AppointmentCRUDController : Controller
    {
        private IAppointmentRepository appointmentRopository;
        private IRoomRepository roomRepository;

        public AppointmentCRUDController(IRoomRepository rRepo, IAppointmentRepository aRepo)
        {
            roomRepository = rRepo;
            appointmentRopository = aRepo;
        }

        [HttpGet("{action}/{code}/{roomId}/{selectedTime}")]
        public IActionResult AddAppointment(string code, int roomId, int scheduleId, DateTime selectedTime)
        {
            if (selectedTime.Date.Date > DateTime.Today || (selectedTime.Date == DateTime.Today.Date && DateTime.Now.TimeOfDay < (DateTime.Today + 22.5.Hours()).TimeOfDay))
            {
                return View(new Appointment { RoomCode = code, RoomId = roomId, ScheduleId = scheduleId, Start = selectedTime, End = selectedTime + 30.Minutes() });
            }

            return RedirectToAction(nameof(Room), nameof(Room), new { code = code, pDate = selectedTime.ToString("MM-dd-yyyy") });
        }

        [HttpPost("{action}/{code}/{roomId}/{selectedTime}")]
        public IActionResult AddAppointment(Appointment appointment)
        {
            if (appointment.Start > appointment.End)
            {
                ModelState.AddModelError("", "Start time must be before end time");
            }

            IEnumerable<Appointment> appointments = appointmentRopository.Appointments.Where(a => a.Start.Date == appointment.Start.Date && a.RoomId == appointment.RoomId);

            if (appointments.Any(a => a.Start.TimeOfDay <= appointment.End.TimeOfDay
                                 && appointment.Start.TimeOfDay <= a.End.TimeOfDay))
            {
                ModelState.AddModelError("", "The time you selected is already busy!");
            }
            if (ModelState.IsValid)
            {
                appointmentRopository.Save(appointment);

                return RedirectToAction(nameof(Room), nameof(Room), new { code = appointment.RoomCode, pDate = appointment.Start.ToString("MM-dd-yyyy") });
            }

            return View(appointment);
        }

        [HttpGet("{action}/{code}/{id}")]
        public ViewResult EditAppointment(string code, int id)
        {
            Appointment app = appointmentRopository.Appointments.FirstOrDefault(a => a.Id == id);
            app.RoomCode = code;
            return View(nameof(EditAppointment), app);
        }

        [HttpPost("{action}/{code}/{id}")]
        public IActionResult EditAppointment(Appointment appointment)
        {
            if (appointment.Start > appointment.End)
            {
                ModelState.AddModelError("", "Start time must be before end time");
            }

            IEnumerable<Appointment> appointments = appointmentRopository.Appointments.Where(a => a.Start.Date == appointment.Start.Date && a.RoomId == appointment.RoomId);

            if (appointments.Any(a => a.Start.TimeOfDay <= appointment.End.TimeOfDay
                                 && appointment.Start.TimeOfDay <= a.End.TimeOfDay))
            {
                ModelState.AddModelError("", "The time you selected is already busy!");
            }
            if (ModelState.IsValid)
            {
                appointmentRopository.Save(appointment);

                return RedirectToAction(nameof(Room), nameof(Room), new { code = appointment.RoomCode, pDate = appointment.Start.ToString("MM-dd-yyyy") });
            }

            return View(appointment);
        }

        [HttpPost]
        public IActionResult DeleteAppointment(int id, string code)
        {
            Appointment app = appointmentRopository.Delete(id);

            return RedirectToAction(nameof(Room), nameof(Room), new { code = code, pDate = app.Start.ToString("MM-dd-yyyy") });
        }
    }
}