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
        public IActionResult Room(string name)
        {
            return View(roomRepository.Rooms.FirstOrDefault(r => r.Name == name));
        }
    }
}