﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyTagProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> AddRoom(Room room)
        {
            Task<bool> isRepeatedLocation;

            if (room.Id != 0)
            {
                isRepeatedLocation = roomRepository.Rooms.AnyAsync(r =>
                            r.RoomCode == $"{room.Block}{room.Floor}-{room.Number}"
                            && r.Id != room.Id);
            }
            else
            {
                isRepeatedLocation = roomRepository.Rooms.AnyAsync(r =>
                            r.RoomCode == $"{room.Block}{room.Floor}-{room.Number}");
            }
            

            if (room.Floor > 4)
            {
                ModelState.AddModelError("Floor", "Floor does not exist");
            }

            if (await isRepeatedLocation)
            {
                ModelState.AddModelError("", "Room location alredy exists");
            }

            if (ModelState.IsValid)
            {
                room.RoomCode = $"{room.Block}{room.Floor}-{room.Number}";
                await roomRepository.SaveAsync(room);

                return RedirectToAction(nameof(Room), nameof(Room), new { code = room.RoomCode });
            }

            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            Room room = await roomRepository.DeleteAsync(id);
            return RedirectToAction("RoomList",nameof(Room),new { });
        }

        [HttpGet ("{action}/{id}")]
        public async Task<ViewResult> EditRoom(int id)
        {
            ViewData["Title"] = "Edit Room";
            Room room = await roomRepository.Rooms.FirstOrDefaultAsync( r => r.Id == id);
            return View(nameof(AddRoom), room);
        }
    }
}