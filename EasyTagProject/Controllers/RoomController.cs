using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyTagProject.Models;
using EasyTagProject.Models.ViewModels;
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
            room.Schedule.Appointments = GetAppointments(room, DateTime.Today.ToString("MM/dd/yyyy"));

            return View(
                new RoomViewModel 
                {
                    Room = room,
                    Pagination = new RoomPagination()
                });
        }

        [HttpGet("{action}/{name}/{date}")]
        public ViewResult Room(string name, string date)
        {
            date = date.Replace("-","/");

            Room room = roomRepository.Rooms.FirstOrDefault(r => r.Name == name);
            RoomPagination pagination = new RoomPagination { CurrentDate = DateTime.ParseExact(date, "MM/dd/yyyy", null) };

            room.Schedule.Appointments = GetAppointments(room, date);

            return View( new RoomViewModel { Pagination = pagination, Room = room });
        }

        public List<Appointment> GetAppointments(Room room, string date)
        {
            return (room.Schedule.Appointments
                    .Where(a => a.Start.ToString("MM/dd/yyyy") == date))
                    .ToList();
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