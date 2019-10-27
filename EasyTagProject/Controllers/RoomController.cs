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
        public RoomController(IRoomRepository rRepo) => roomRepository = rRepo;

        /// <summary>
        /// Action Method that returns a view with the room information
        /// </summary>
        /// <param name="name">
        /// Name of the room to identify
        /// </param>
        /// <returns>
        /// View with the room information
        /// </returns>
        [HttpGet("{action}/{name}")]
        public ViewResult Room(string name)
        {
            Room room = roomRepository.Rooms.FirstOrDefault(r => r.Name == name);
            room.Schedule.Appointments = GetAppointments(room, DateTime.Today);

            return View(
                new RoomViewModel 
                {
                    Room = room,
                    Pagination = new RoomPagination()
                });
        }

        [HttpGet("{action}/{name}/{pDate}")]
        public ViewResult Room(string name, DateTime pDate)
        {
            Room room = roomRepository.Rooms.FirstOrDefault(r => r.Name == name);
            RoomPagination pagination = new RoomPagination { CurrentDate = pDate };

            room.Schedule.Appointments = GetAppointments(room, pDate);                    

            return View( new RoomViewModel { Pagination = pagination, Room = room });
        }

        [HttpPost]
        public IActionResult FindRoom(string name, DateTime pDate)
        {

            string date = pDate.Date.ToString("MM-dd-yyyy");

            return RedirectToAction(nameof(Room), nameof(Room), new { name = name, pDate = date });
        }

        public List<Appointment> GetAppointments(Room room, DateTime date)
        {
            Console.WriteLine(date);
            return (room.Schedule.Appointments
                    .Where(a => a.Start.Date == date.Date)
                    .OrderBy(a => a.Start))
                    .ToList();
        }

        [HttpGet("{action}/{Id}/{searchDate}")]
        public ViewResult AddAppointment(int Id, DateTime searchDate)
        {

            Room room = roomRepository.Rooms.FirstOrDefault(r => r.Id == Id);

            room.Schedule.Appointments = room.Schedule.GetAppointments(searchDate);

            return View(new AddAppointmentViewModel { Room = room, Date = searchDate.Date + DateTime.Now.TimeOfDay});
        }

        [HttpGet("{controller}/{action}")]
        public ViewResult AddRoom() => View(new Room());

        [HttpPost("{controller}/{action}")]
        public IActionResult AddRoom(Room room)
        {
            if (ModelState.IsValid)
            {
                roomRepository.Save(room);
            }

            return View(room);
        }
    }
}