using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyTagProject.Models;
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
            return View(roomRepository.Rooms.FirstOrDefault(r => r.Name == name));
        }

        [HttpGet]
        public ViewResult AddRoom() => View(new Room());

        [HttpPost]
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