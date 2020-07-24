using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyTagProject.Infrastructure;
using EasyTagProject.Models;
using EasyTagProject.Models.ViewModels;
using FluentDate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyTagProject.Controllers
{
    [Authorize(Roles = "Admin,Professor")]
    public class AppointmentController : Controller
    {
        private readonly IRoomRepository roomRepository;
        private readonly IAppointmentRepository aRepo;

        public AppointmentController(IRoomRepository rRepo, IAppointmentRepository aRepo) 
        { 
            roomRepository = rRepo;
            this.aRepo = aRepo;
        }

        public async Task<IActionResult> RedirectToAppointmentList(string code, DateTime pDate)
        {
            return RedirectToAction(nameof(AppointmentList), nameof(Appointment), new { code, pDate = pDate.ToString("MM-dd-yyyy")});
        }

        public async Task<IActionResult> RedirectToRoom(string code, DateTime pDate)
        {
            return RedirectToAction(nameof(Room), nameof(Room), new { code = code, pDate = pDate.ToString("MM-dd-yyyy") });
        }

        [HttpGet("{action}/{code}/{pDate}")]
        public async Task<IActionResult> AppointmentList(string code, DateTime pDate)
        {
            if (pDate >= DateTime.Today)
            {
                Room room = await roomRepository.Rooms.FirstOrDefaultAsync(r => r.RoomCode == code);
                room.Schedule.Appointments = room.Schedule.GetAppointmentsInDate(pDate);


                var pagination = new RoomPagination
                {
                    CurrentDate = pDate,
                    RoomCode = code,
                    Action = "AppointmentList",
                    Controller = "Appointment",
                    AllowPrevious = pDate > DateTime.Today ? true : false 
                };

                var appointmentListModel = new AppointmentListViewModel
                {
                    Room = room,
                    Date = pDate.Date + DateTime.Now.TimeOfDay,
                    RoomPagination = pagination,
                    BtnNew = new BtnNewAppViewModel
                    {
                        RoomCode = room.RoomCode,
                        Message = "Free Time"
                    }                    
                };

                if (room.Schedule.Appointments.Count > 0 &&
                    room.Schedule.Appointments.Any(a => a.Start > DateTime.Now))
                {
                    appointmentListModel.ResponsiveTable = "table-responsive-sm";
                }

                return View(appointmentListModel); 
            }

            return RedirectToAction(nameof(Room), nameof(Room), new { code = code, pDate = pDate.ToString("MM-dd-yyyy") });
        }

        [HttpGet("{action}/{code}/{id}")]
        public async Task<ViewResult> AppointmentConfirmation(string code, int id)
        {
            Appointment appointment = await aRepo.Appointments.FirstOrDefaultAsync(a => a.Id == id);
            appointment.RoomCode = code;
            return View(appointment);
        }

        [HttpGet]
        public async Task<ViewResult> ReportAppointments(string date)
        {
            var model = new ReportAppointmentsViewModel
            {
                Appointments = HttpContext.Session.GetJson<IEnumerable<Appointment>>("repeatResult") ?? new List<Appointment>(),
                BtnNew = new BtnNewAppViewModel
                {
                    Message = "Try another time?"
                },
                Date = String.IsNullOrEmpty(date) ? DateTime.Today.ToString("MM-dd-yyyy") : date
            };

            return View(model);
        }
    }
}