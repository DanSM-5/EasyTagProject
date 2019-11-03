using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyTagProject.Models;
using EasyTagProject.Models.ViewModels;
using FluentDate;
using Microsoft.AspNetCore.Mvc;

namespace EasyTagProject.Controllers
{
    public class RoomController : Controller
    {
        private IRoomRepository roomRepository;

        public RoomController(IRoomRepository rRepo)
        {
            roomRepository = rRepo;
        }

        [HttpGet("{action}/{code}")]
        public ViewResult Room(string code)
        {
            return View(new RoomViewModel
            {
                Room = GetRoomWithDailyAppointments(code, DateTime.Today),
                Pagination = new RoomPagination()
            });
        }

        [HttpGet("{action}/{code}/{pDate}")]
        public ViewResult Room(string code, DateTime pDate)
        {
            return View(new RoomViewModel
            {
                Room = GetRoomWithDailyAppointments(code, pDate),
                Pagination = new RoomPagination { CurrentDate = pDate }
            });
        }

        public Room GetRoomWithDailyAppointments(string code, DateTime date)
        {
            // Get Room
            Room room = roomRepository.Rooms.FirstOrDefault(r => r.RoomCode == code);

            // Set appontments
            room.Schedule.Appointments = (room.Schedule.Appointments
                                         .Where(a => a.Start.Date == date)
                                         .OrderBy(a => a.Start)).ToList();

            return room;
        }

        [HttpPost]
        public IActionResult FindRoomByDate(string code, DateTime pDate)
        {
            if (code != "" && pDate != default(DateTime))
            {
                return RedirectToAction(nameof(Room), nameof(Room), new { code = code, pDate = pDate.ToString("MM-dd-yyyy") });
            }

            return Redirect("/Home/Error");
        }

        [HttpGet("Administration/{action}")]//("{controller}/{action}")]
        public ViewResult RoomList()
        {
            return View(roomRepository.Rooms.OrderBy(r => r.RoomCode));
        }
    }
}