using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyTagProject.Models;
using EasyTagProject.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyTagProject.Controllers
{
    [Authorize(Roles = nameof(UserRoles.Admin))]
    public class RoomCRUDController : Controller
    {
        private IRoomRepository roomRepository;

        public RoomCRUDController(IRoomRepository repo)
        {
            roomRepository = repo;
        }

        [HttpGet("{action}")]
        public ViewResult AddRoom()
        {
            ViewData["Title"] = "Add Room";
            return View(new Room());
        }

        [HttpPost("{action}")]
        public async Task<IActionResult> AddRoom(Room room)
        {
            Task<bool> isRepeatedLocation;

            // Check if a new room already exists with the same location
            // or if an update conflicts with an existing location
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
            

            if (room.Floor > 4 || room.Floor < 1)
            {
                ModelState.AddModelError("Floor", "Floor does not exist");
            }

            if (await isRepeatedLocation)
            {
                ModelState.AddModelError("", "Room location alredy exists");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    room.RoomCode = $"{room.Block}{room.Floor}-{room.Number.ToString("00")}";
                    await roomRepository.SaveAsync(room);

                    return RedirectToAction(nameof(Room), nameof(Room), new { code = room.RoomCode });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ModelState.AddModelError("", "The room has been modified! Please refresh the page or go to Room List");
                    return View(room);
                }
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