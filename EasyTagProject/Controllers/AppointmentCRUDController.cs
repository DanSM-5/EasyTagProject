using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EasyTagProject.Infrastructure;
using EasyTagProject.Models;
using EasyTagProject.Models.Identity;
using FluentDate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EasyTagProject.Controllers
{
    [Authorize(Roles = "Admin,Professor")]
    public class AppointmentCRUDController : Controller
    {
        private IAppointmentRepository appointmentRopository;
        private IRoomRepository roomRepository;
        private UserManager<EasyTagUser> userManager;
        private IHttpContextAccessor ViewContext { get; }

        public AppointmentCRUDController(IRoomRepository rRepo, IAppointmentRepository aRepo, IHttpContextAccessor httpContext, UserManager<EasyTagUser> manager)
        {
            roomRepository = rRepo;
            appointmentRopository = aRepo;
            ViewContext = httpContext;
            userManager = manager;
        }

        [HttpGet("{action}/{code}/{roomId}/{selectedTime}")]
        public async Task<IActionResult> AddAppointment(string code, int roomId, int scheduleId, DateTime selectedTime)
        {
            if (selectedTime.Date.Date > DateTime.Today || (selectedTime.Date == DateTime.Today.Date && DateTime.Now.TimeOfDay < (DateTime.Today + 22.5.Hours()).TimeOfDay))
            {
                // Find logged user and add the name as default for an appointmnt
                EasyTagUser tagUser = await userManager.FindByNameAsync(ViewContext.HttpContext.GetLoggedUserName());
                return View(new Appointment { UserName = tagUser.FullName, RoomCode = code, RoomId = roomId, ScheduleId = scheduleId, Start = selectedTime, End = selectedTime + 30.Minutes() });
            }

            return RedirectToAction(nameof(Room), nameof(Room), new { code = code, pDate = selectedTime.ToString("MM-dd-yyyy") });
        }

        [HttpPost("{action}/{code}/{roomId}/{selectedTime}")]
        public async Task<IActionResult> AddAppointment(Appointment appointment)
        {
            await ValidateAppointmentAsync(appointment);

            if (ModelState.IsValid)
            {
                appointment.UserId = ViewContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await appointmentRopository.SaveAsync(appointment);

                return RedirectToAction(nameof(Room), nameof(Room), new { code = appointment.RoomCode, pDate = appointment.Start.ToString("MM-dd-yyyy") });
            }

            return View(appointment);
        }

        [HttpGet("{action}/{code}/{id}")]
        public async Task<ViewResult> EditAppointment(string code, int id)
        {
            Appointment app = await appointmentRopository.Appointments.FirstOrDefaultAsync(a => a.Id == id);
            app.RoomCode = code;
            return View(app);
        }

        [HttpPost("{action}/{code}/{id}")]
        public async Task<IActionResult> EditAppointment(Appointment appointment)
        {
            await ValidateAppointmentAsync(appointment);

            if (ModelState.IsValid)
            {
                await appointmentRopository.SaveAsync(appointment);

                return RedirectToAction(nameof(Room), nameof(Room), new { code = appointment.RoomCode, pDate = appointment.Start.ToString("MM-dd-yyyy") });
            }

            return View(appointment);
        }

        private async Task ValidateAppointmentAsync(Appointment appointment)
        {
            //  Start task to get appointments from the same room and in the same date
            var getAppointments = appointmentRopository.Appointments.Where(a => a.Start.Date == appointment.Start.Date && a.RoomId == appointment.RoomId).ToListAsync();
            
            if (appointment.Start > appointment.End)
            {
                ModelState.AddModelError("", "Start time must be before end time");
            }

            if (appointment.Start < DateTime.Today.AddDays(-1))
            {
                ModelState.AddModelError("", "The appointment cannot be created in the past");
            }

            //  Identify if any apointment overlaps
            #region AvoidOverlapingAppoinments

            List<Appointment> appointments = await getAppointments;
            
            //  Remove the appointment being validating 
            //  from the list of appointments
            //  if it is not a new appointment
            if (appointments.Any(a => a.Id == appointment.Id))
            {
                appointments.Remove(appointments.Single(a => a.Id == appointment.Id));
            }

            //  Verify if the appoinment overlaps another
            if (appointments.Any(a => a.Start.TimeOfDay < appointment.End.TimeOfDay
                                 && appointment.Start.TimeOfDay < a.End.TimeOfDay))
            {
                ModelState.AddModelError("", "The time you selected is already busy!");
            } 
            #endregion
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAppointment(int id, string code)
        {
            Appointment app = await appointmentRopository.DeleteAsync(id);

            return RedirectToAction(nameof(Room), nameof(Room), new { code = code, pDate = app.Start.ToString("MM-dd-yyyy") });
        }
    }
}