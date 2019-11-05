using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyTagProject.Models;
using EasyTagProject.Models.ViewModels;
using FluentDate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyTagProject.Controllers
{
    public class AppointmentController : Controller
    {
        private IAppointmentRepository appointmentRopository;
        private IRoomRepository roomRepository;

        public AppointmentController(IAppointmentRepository aRepo, IRoomRepository rRepo) 
        { 
            appointmentRopository = aRepo;
            roomRepository = rRepo;
        }

        public IActionResult RedirectToAppointmentList(int id, DateTime date)
        {
            return RedirectToAction(nameof(AppointmentList), nameof(Appointment), new { id = id, searchDate = date.ToString("MM-dd-yyyy")});
        }

        public IActionResult RedirectToRoom(string code, DateTime pDate)
        {
            return RedirectToAction(nameof(Room), nameof(Room), new { code = code, pDate = pDate.ToString("MM-dd-yyyy") });
        }

        [HttpGet("{action}/{Id}/{searchDate}")]
        public async Task<IActionResult> AppointmentList(int Id, DateTime searchDate)
        {
            Room room = await roomRepository.Rooms.FirstOrDefaultAsync(r => r.Id == Id);
            
            if (searchDate >= DateTime.Today)
            {
                room.Schedule.Appointments = room.Schedule.GetAppointments(searchDate);

                return View(new AppointmentListViewModel { Room = room, Date = searchDate.Date + DateTime.Now.TimeOfDay }); 
            }

            return RedirectToAction(nameof(Room), nameof(Room), new { code = room.RoomCode, pDate = searchDate.ToString("MM-dd-yyyy") });
        }
    }
}