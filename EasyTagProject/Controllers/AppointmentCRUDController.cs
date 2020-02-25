using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EasyTagProject.Infrastructure;
using EasyTagProject.Models;
using EasyTagProject.Models.Identity;
using EasyTagProject.Models.ViewModels;
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
        private readonly IRoomRepository roomRepository;
        private UserManager<EasyTagUser> userManager;
        private IHttpContextAccessor ViewContext { get; }

        public AppointmentCRUDController(IAppointmentRepository aRepo, IRoomRepository rRepo, IHttpContextAccessor httpContext, UserManager<EasyTagUser> manager)
        {
            appointmentRopository = aRepo;
            roomRepository = rRepo;
            ViewContext = httpContext;
            userManager = manager;
        }

        [HttpGet("{action}/{code}/{selectedTime}")]
        public async Task<IActionResult> AddAppointment(string code, DateTime selectedTime)
        {
            // Protection to avoid add appointments out of the time range
            if (selectedTime.Date.Date > DateTime.Today 
                || (selectedTime.Date == DateTime.Today.Date && 
                    DateTime.Now.TimeOfDay < (DateTime.Today + 22.5.Hours()).TimeOfDay))
            {
                Room room = await roomRepository.Rooms.FirstOrDefaultAsync(r => r.RoomCode == code);
                if (room != null)
                {
                    // Find logged user and add the name as default for an appointmnt
                    EasyTagUser tagUser = await userManager.FindByNameAsync(ViewContext.HttpContext.GetLoggedUserName());
                    return View(new Appointment { UserName = tagUser.FullName, RoomCode = code, RoomId = room.Id, ScheduleId = room.Schedule.Id, Start = selectedTime, End = selectedTime + 30.Minutes() }); 
                }
            }
            return Redirect("/");
        }

        [HttpPost("{action}/{code}/{selectedTime}")]
        public async Task<IActionResult> AddAppointment(Appointment appointment)
        {
            appointment.Id = 0;
            await ValidateAppointmentAsync(appointment);

            if (ModelState.IsValid)
            {
                appointment.UserId = ViewContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await appointmentRopository.SaveAsync(appointment);

                return RedirectToAction("AppointmentConfirmation", nameof(Appointment), new { code = appointment.RoomCode, id = appointment.Id });
            }

            return View(appointment);
        }

        [HttpGet("{action}/{code}/{id}")]
        public async Task<IActionResult> EditAppointment(string code, int id)
        {
            Appointment app = await appointmentRopository.Appointments.FirstOrDefaultAsync(a => a.Id == id);
            if (app != null)
            {
                Room room = await roomRepository.Rooms.FirstOrDefaultAsync(r => r.RoomCode == code);
                if (room != null)
                {
                    if (ViewContext.HttpContext.IsAccessibleForUserOrAdmin(app.UserId))
                    {
                        app.RoomCode = code;
                        return View(app);
                    } 
                }
            }
            return Redirect("/");
        }

        [HttpPost("{action}/{code}/{id}")]
        public async Task<IActionResult> EditAppointment(Appointment appointment)
        {
            // Redirect to home if there is no existing appointment
            if (!(appointmentRopository.Appointments.Any(a => a.Id == appointment.Id)))
            {
                return Redirect("/");
            }

            if ((await appointmentRopository.Appointments.FirstAsync(a => a.Id == appointment.Id)).InformationChanged(appointment))
            {
                return RedirectToAction(nameof(Room), nameof(Room), new { code = appointment.RoomCode, pDate = appointment.Start.ToString("MM-dd-yyyy") });
            }

            await ValidateAppointmentAsync(appointment);

            if (ModelState.IsValid)
            {
                await appointmentRopository.SaveAsync(appointment);

                return RedirectToAction(nameof(Room), nameof(Room), new { code = appointment.RoomCode, pDate = appointment.Start.ToString("MM-dd-yyyy") });
            }

            return View(appointment);
        }

        [HttpGet("{action}/{code}/{id}/{dateTime}")]
        public async Task<ViewResult> RepeatAppointment(string code, int id, DateTime dateTime)
        {
            return View(new RepeatAppViewModel { Code = code, Id = id, IncludeWeekends = true, Date = dateTime.ToString("MM-dd-yyyy") });
        }

        [HttpPost("{action}/{code}/{id}/{dateTime}")]
        public async Task<IActionResult> RepeatAppointment(RepeatAppViewModel model)
        {
            if (model.PeriodType == RepeatTime.Unselect || model.PeriodType == RepeatTime.Never)
            {
                ModelState.AddModelError("", "Select the repetition type");
            }
            if (model.RepeatNumber == 0)
            {
                ModelState.AddModelError("", "You must repeat the appointment at least once");
            }
            if (model.PeriodType == RepeatTime.Once && model.TargetDay == null ||
                model.PeriodType == RepeatTime.Once && model.TargetDay < DateTime.Today)
            {
                ModelState.AddModelError("", "Date is not valid");
            }

            if (ModelState.IsValid)
            {
                Appointment app = await appointmentRopository.Appointments.FirstOrDefaultAsync(a => a.Id == model.Id);
                app.RoomCode = model.Code;

                var toCompareTime = await appointmentRopository.Appointments
                                        .Where(a =>
                                            a.Start.Date > app.Start.Date
                                            && a.RoomId == app.RoomId
                                            || a.Id == app.Id)
                                        .ToDictionaryAsync(a => a.Id);

                var appointments = await RepeatAppointment(app, model.PeriodType, model.RepeatNumber, model.IncludeWeekends, toCompareTime, model.TargetDay?.Date ?? default(DateTime));

                TempData.SetObject("app", appointments);
                return RedirectToAction("ReportAppointments", nameof(Appointment)); 
            }

            return View(model);
        }

        // Changes
        private async Task<bool> ValidateAppointmentAsync(Appointment appointment, Dictionary<int, Appointment> appointmentsToCompare = null)
        {
            bool success = true;
            Task<Dictionary<int, Appointment>> getAppointments = null;

            if (appointmentsToCompare == null)
            {
                //  Start task to get appointments from the same room and in the same date
                getAppointments = appointmentRopository.Appointments
                                        .Where(a =>
                                            a.Start.Date == appointment.Start.Date
                                            && a.RoomId == appointment.RoomId
                                            || a.Id == appointment.Id)
                                        .ToDictionaryAsync(a => a.Id); 
            }
            
            // Starting time must be always before ending time
            if (appointment.Start > appointment.End)
            {
                ModelState.AddModelError("", "Start time must be before end time");
                success = false;
            }

            // Starting time cannot be the same as ending time
            if(appointment.Start == appointment.End)
            {
                ModelState.AddModelError("", "Start must be different than end time");
                success = false;
            }

            // Appointment cannot be set in the past
            if (appointment.Start < DateTime.Today)
            {
                ModelState.AddModelError("", "The appointment cannot be set in the past");
                success = false;
            }

            // New appointments cannot be created in the past
            if (appointment.Start < DateTime.Now && appointment.Id == 0)
            {
                ModelState.AddModelError("", "The appointment cannot be created in the past");
                success = false;
            }

            // Appointment range cannot last more than one day
            if (appointment.Start.Date != appointment.End.Date)
            {
                ModelState.AddModelError("", "The appointment must be created in the same day");
                success = false;
            }

            //  Identify if any apointment overlaps
            #region AvoidOverlapingAppoinments

            Dictionary<int, Appointment> appointments = appointmentsToCompare == null ? await getAppointments : appointmentsToCompare;
            
            // Validate that the appointment being eddited 
            // is not in the past and belongs to the same user or is an admin
            if (appointments.Any(a => a.Key == appointment.Id))
            {
                var app = appointments[appointment.Id];
                appointments.Remove(appointment.Id);

                // Protect editting appointments in the past
                if (app.Start < DateTime.Today)
                {
                    ModelState.AddModelError("", "You cannot edit an appointment in the past!");
                    success = false;
                }

                // Protect editting appointments of another user. Only admins are allowed to do that.
                if (!(ViewContext.HttpContext.IsAccessibleForUserOrAdmin(app.UserId)))
                {
                    ModelState.AddModelError("", "You do not have the rights to edit this appointment");
                    success = false;
                }

                // Start date cannot be moved to the past
                if (app.Start < DateTime.Now && app.Start != appointment.Start)
                {
                    appointment.Start = app.Start;

                }
            }

            //  Verify if the appoinment overlaps another
            if (appointments.Any(a => a.Value.Start < appointment.End
                                 && appointment.Start < a.Value.End))
            {
                ModelState.AddModelError("", "The time you selected is already busy!");
                success = false;
            }
            #endregion

            return success;
        }

        public async Task<IEnumerable<Appointment>> RepeatAppointment(Appointment original, RepeatTime periodType, int repeatNumber, bool includeWeekends, Dictionary<int, Appointment> compareTo, DateTime targetDay = default(DateTime))
        {
            var tasks = new List<Task<Appointment>>();
            int count = 1;

            while(tasks.Count < repeatNumber)
            {
                if (!includeWeekends)
                {
                    var tempDate = original.Start + count.Days();

                    if (tempDate.DayOfWeek == DayOfWeek.Saturday)
                    {
                        count += 2;
                        continue;
                    }
                    else if (tempDate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        count++;
                        continue;
                    }
                }

                tasks.Add(AddDuplicate(original.Clone(), periodType, count++, compareTo, targetDay));
            }

            var appointments = await Task.WhenAll(tasks);
            await appointmentRopository.SaveRangeAsync(appointments.Where(a => a.isValid == true));

            return appointments;
        }

        public async Task<Appointment> AddDuplicate(Appointment newAppointment, RepeatTime periodType, int amount, Dictionary<int,Appointment> compareTo, DateTime targetDay = default(DateTime))
        {
            switch (periodType)
            {
                case RepeatTime.Once:
                    newAppointment.Start = targetDay.Date + newAppointment.Start.TimeOfDay;
                    newAppointment.End = targetDay.Date + newAppointment.End.TimeOfDay;
                    break;
                case RepeatTime.Daily:
                    newAppointment.Start += amount.Days();
                    newAppointment.End += amount.Days();
                    break;
                case RepeatTime.Weekly:
                    newAppointment.Start += amount.Weeks();
                    newAppointment.End += amount.Weeks();
                    break;
                case RepeatTime.Monthly:
                    newAppointment.Start += amount.Months();
                    newAppointment.End += amount.Months();
                    break;
                case RepeatTime.Never:
                    break;
                default:
                    break;
            }

            newAppointment.isValid = await ValidateAppointmentAsync(newAppointment, compareTo);

            return newAppointment;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAppointment(int id, string code)
        {
            Appointment app = await appointmentRopository.DeleteAsync(id);

            return RedirectToAction(nameof(Room), nameof(Room), new { code = code, pDate = app.Start.ToString("MM-dd-yyyy") });
        }
    }
}