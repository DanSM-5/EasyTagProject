using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EasyTagProject.Models;
using EasyTagProject.Models.ViewModels;
using FluentDate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ViewResult> Room(string code)
        {
            return View(new RoomViewModel
            {
                Room = await GetRoomWithDailyAppointmentsAsync(code, DateTime.Today),
                Pagination = new RoomPagination()
            });
        }

        [HttpGet("{action}/{code}/{pDate}")]
        public async Task<ViewResult> Room(string code, DateTime pDate)
        {
            return View(new RoomViewModel
            {
                Room = await GetRoomWithDailyAppointmentsAsync(code, pDate),
                Pagination = new RoomPagination { CurrentDate = pDate }
            });
        }

        public async Task<Room> GetRoomWithDailyAppointmentsAsync(string code, DateTime date)
        {
            // Get Room
            Room room = await roomRepository.Rooms.FirstOrDefaultAsync(r => r.RoomCode == code);

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

        [HttpGet("{action}/{code}")]
        public ViewResult RoomTag(string code)
        {
            // Identify host string
            var hostString = HttpContext.Request.Host.ToString();
            StringBuilder url = new StringBuilder();
            url.Append(hostString);
            url.Append("/Room/");
            url.Append(code);
            url.Append("/");
            string encodedUrl = HttpUtility.UrlEncode(url.ToString());

            // Creation of url for API
            url.Clear();
            url.Append("http://api.qrserver.com/v1/create-qr-code/?data=");
            url.Append(encodedUrl);
            url.Append("&size=600x600");

            // Information for page RoomTag
            ViewBag.ImageUrl = url.ToString();
            ViewBag.Print = true;
            ViewBag.Code = code;

            return View();
        }
    }
}