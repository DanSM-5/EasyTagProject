using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyTagProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace EasyTagProject.Controllers
{
    public class RoomCRUDController : Controller
    {
        private IRoomRepository roomRepository;

        public RoomCRUDController(IRoomRepository repo)
        {
            roomRepository = repo;
        }

        [HttpGet("{action}")]//("{controller}/{action}")]
        public ViewResult AddRoom()
        {
            ViewData["Title"] = "Add Room";
            return View(new Room());
        }

        [HttpPost("{action}")]//("{controller}/{action}")]
        public IActionResult AddRoom(Room room)
        {
            if (room.Floor > 4)
            {
                ModelState.AddModelError("Floor", "Floor does not exist");
            }
            if (roomRepository.Rooms.Any(r => r.RoomCode == $"{room.Block}{room.Floor}-{room.Number}"))
            {
                ModelState.AddModelError("", "Room location alredy exists");
            }
            if (ModelState.IsValid)
            {
                room.RoomCode = $"{room.Block}{room.Floor}-{room.Number}";
                roomRepository.Save(room);

                return RedirectToAction(nameof(Room), nameof(Room), new { code = room.RoomCode });
            }

            return View(room);
        }

        [HttpPost]
        public IActionResult DeleteRoom(int id)
        {
            Room room = roomRepository.Delete(id);
            return RedirectToAction("RoomList",nameof(Room),new { });
        }

        [HttpGet ("{action}/{id}")]
        public ViewResult EditRoom(int id)
        {
            ViewData["Title"] = "Edit Room";
            Room room = roomRepository.Rooms.FirstOrDefault( r => r.Id == id);
            return View(nameof(AddRoom), room);
        }
    }
}