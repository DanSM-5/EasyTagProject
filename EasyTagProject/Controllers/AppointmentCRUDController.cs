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

        public AppointmentCRUDController(IAppointmentRepository aRepo, IRoomRepository rRepo, UserManager<EasyTagUser> manager)
        {
            appointmentRopository = aRepo;
            roomRepository = rRepo;
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
                    EasyTagUser tagUser = await userManager.FindByNameAsync(HttpContext.GetLoggedUserName());
                    return View(new Appointment { UserName = tagUser.FullName, RoomCode = code, RoomId = room.Id, ScheduleId = room.Schedule.Id, Start = selectedTime, End = selectedTime + 30.Minutes() }); 
                }
            }
            return Redirect("/");
        }

        [HttpPost("{action}/{code}/{selectedTime}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAppointment(
            [Bind(nameof(Appointment.RowVersion), nameof(Appointment.RoomId), nameof(Appointment.RoomCode), nameof(Appointment.ScheduleId),
                  nameof(Appointment.UserName),nameof(Appointment.Start),nameof(Appointment.End),nameof(Appointment.Course),
                  nameof(Appointment.Description))]
            Appointment appointment)
        {
            appointment.Id = 0;
            await ValidateAppointmentAsync(appointment);

            if (ModelState.IsValid)
            {
                try
                {
                    appointment.UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    await appointmentRopository.SaveAsync(appointment);

                    return RedirectToAction("AppointmentConfirmation", nameof(Appointment), new { code = appointment.RoomCode, id = appointment.Id });
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "The time for the appointment has been used, please select another time");
                    return View(appointment);
                }
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
                    if (HttpContext.IsAccessibleForUserOrAdmin(app.UserId))
                    {
                        app.RoomCode = code;
                        return View(app);
                    } 
                }
            }
            return Redirect("/");
        }

        [HttpPost("{action}/{code}/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAppointment(
            [Bind(nameof(Appointment.RowVersion), nameof(Appointment.RoomId), nameof(Appointment.RoomCode), nameof(Appointment.ScheduleId),
                  nameof(Appointment.UserName),nameof(Appointment.Start),nameof(Appointment.End),nameof(Appointment.Course),
                  nameof(Appointment.Description), nameof(Appointment.Id))]
            Appointment appointment)
        {
            // Redirect to home if there is no existing appointment
            if (!(await appointmentRopository.Appointments.AnyAsync(a => a.Id == appointment.Id)))
            {
                return Redirect("/");
            }

            if (!(await appointmentRopository.Appointments.FirstAsync(a => a.Id == appointment.Id)).InformationChanged(appointment))
            {
                return RedirectToAction(nameof(Room), nameof(Room), new { code = appointment.RoomCode, pDate = appointment.Start.ToString("MM-dd-yyyy") });
            }

            await ValidateAppointmentAsync(appointment);

            if (ModelState.IsValid)
            {
                try
                {
                    await appointmentRopository.SaveAsync(appointment);

                    return RedirectToAction(nameof(Room), nameof(Room), new { code = appointment.RoomCode, pDate = appointment.Start.ToString("MM-dd-yyyy") });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var databaseEntry = ex.Entries.Single().GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError("", "The appointment has been deleted!");
                    }
                    else
                    {
                        Appointment conflicted = databaseEntry.ToObject() as Appointment;

                        if (conflicted.UserName != appointment.UserName)
                        {
                            ModelState.AddModelError(nameof(Appointment.UserName), $"Current name: {conflicted.UserName}");
                        }
                        if (conflicted.Start != appointment.Start)
                        {
                            ModelState.AddModelError(nameof(Appointment.Start), $"Current start value: {conflicted.Start.ToString("MM-dd-yyyy H:mm")}");
                        }
                        if (conflicted.End != appointment.End)
                        {
                            ModelState.AddModelError(nameof(Appointment.End), $"Current end value: {conflicted.End.ToString("MM-dd-yyyy H:mm")}");
                        }
                        if (conflicted.Description != appointment.Description)
                        {
                            ModelState.AddModelError(nameof(Appointment.Description), $"Current description: {conflicted.Description}");
                        }
                        if (conflicted.Course != appointment.Course)
                        {
                            ModelState.AddModelError(nameof(Appointment.Course), $"Current course: {conflicted.Course}");
                        }

                        ModelState.AddModelError("", "The appointment has been modified!");
                    }

                    return View(appointment);
                }
            }

            return View(appointment);
        }

        [HttpGet("{action}/{code}/{id}/{start}/{end}")]
        public async Task<ViewResult> RepeatAppointment(string code, int id, DateTime start, DateTime end)
        {
            return View(new RepeatAppViewModel { Code = code, Id = id, IncludeWeekends = true, Date = start.ToString("MM-dd-yyyy"), TargetDay = start, Start = start.TimeOfDay, End = end.TimeOfDay });
        }

        [HttpPost("{action}/{code}/{id}/{start}/{end}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RepeatAppointment(
            [Bind]
            RepeatAppViewModel model)
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

                var appointments = await RepeatAppointment(app, model, toCompareTime);

                if (appointments.Any(a => a.ErrorHappened))
                {
                    ModelState.Clear();
                    ModelState.AddModelError("", $"An error occurred while processing the request, " +
                                                 $"it is possible that the appointments were not created, " +
                                                 $"please make sure and then try again");
                    return View(model);
                }

                HttpContext.Session.SetJson("repeatResult", appointments);
                
                return RedirectToAction("ReportAppointments", nameof(Appointment)); 
            }

            return View(model);
        }

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

            if (appointment.Start.TimeOfDay.TotalMinutes % 30 != 0 ||
                appointment.End.TimeOfDay.TotalMinutes % 30 != 0)
            {
                ModelState.AddModelError("", "The time selected is not valid");
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
                if (!(HttpContext.IsAccessibleForUserOrAdmin(app.UserId)))
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

        public async Task<IEnumerable<Appointment>> RepeatAppointment(Appointment original, RepeatAppViewModel model, Dictionary<int, Appointment> compareTo)
        {
            var tasks = new List<Task<Appointment>>();
            int count = 1;

            while(tasks.Count < model.RepeatNumber)
            {
                if (!model.IncludeWeekends)
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

                tasks.Add(AddDuplicate(original.Clone(), model, count++, compareTo));
            }

            var appointments = await Task.WhenAll(tasks);


            try
            {
                await appointmentRopository.SaveRangeAsync(appointments.Where(a => a.IsValid == true));
            }
            catch (DbUpdateException ex)
            {
                Parallel.ForEach(appointments.Where(a => a.IsValid && a.Id == 0), a => a.ErrorHappened = true);                
            }

            return appointments;
        }

        public async Task<Appointment> AddDuplicate(Appointment newAppointment, RepeatAppViewModel model, int amount, Dictionary<int,Appointment> compareTo)
        {
            switch (model.PeriodType)
            {
                case RepeatTime.Once:

                    //var appointmentDuration = newAppointment.End - newAppointment.Start;
                    newAppointment.Start = model.TargetDay.Value.Date + model.Start.Value;
                    newAppointment.End = model.TargetDay.Value.Date + model.End.Value;
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

            newAppointment.IsValid = await ValidateAppointmentAsync(newAppointment, compareTo);

            return newAppointment;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAppointment(int id, string code)
        {
            if (HttpContext.IsAccessibleForUserOrAdmin(HttpContext.GetLoggedUserId()))
            {
                try
                {
                    Appointment app = await appointmentRopository.DeleteAsync(id);

                    if (app != null)
                    {
                        return RedirectToAction(nameof(Room), nameof(Room), new { code = code, pDate = app.Start.ToString("MM-dd-yyyy") });
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ModelState.AddModelError("", $"The appointment with id {id} was already deleted");                 
                }
            }

            return RedirectToAction(nameof(Room), nameof(Room), new { code = code });
        }
    }
}