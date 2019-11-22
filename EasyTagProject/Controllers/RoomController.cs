using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EasyTagProject.Models;
using EasyTagProject.Models.ViewModels;
using FluentDate;
using Microsoft.AspNetCore.Authorization;
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
            room.Schedule.Appointments = room.Schedule.GetAppointmentsInDate(date);

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

        [HttpGet("{action}/{page=1}")]
        public async Task<IActionResult> RoomList(int page = 1)
        {
            if (page == 0)
            {
                return RedirectToAction(nameof(RoomList));
            }

            RoomListPagination pagination = new RoomListPagination { CurrentPage = page };

            IEnumerable<Room> rooms = await Task.Run(() => roomRepository.Rooms.OrderBy(r => r.RoomCode)
                                                    .Skip((page - 1) * pagination.PageSize)
                                                    .Take(pagination.PageSize));

            pagination.Count = roomRepository.Rooms.Count();

            return View( new RoomListViewModel 
            {
                Rooms = rooms, 
                Pagination = pagination 
            });
        }

        [HttpGet("{action}/{page=1}/{searchString?}")]
        public async Task<IActionResult> SearchList(string searchString, int page)
        {
            if (page == 0)
            {
                return RedirectToAction(nameof(RoomList));
            }

            if (String.IsNullOrEmpty(searchString))
            {
                return RedirectToAction(nameof(RoomList));
            }

            RoomListPagination pagination = new RoomListPagination { CurrentPage = page == 0 ? 1 : page };

            IEnumerable<Room> rooms = await Task.Run(() => roomRepository.Rooms
                                            .Where(r => r.RoomCode.Contains(searchString) || r.Name.Contains(searchString))
                                            .OrderBy(r => r.RoomCode));

            pagination.Count = rooms.Count();

            rooms = await Task.Run(() => rooms.Skip((page - 1) * pagination.PageSize)
                                            .Take(pagination.PageSize));

            return View(nameof(RoomList), new RoomListViewModel
            {
                Rooms = rooms,
                Pagination = pagination,
                SearchString = searchString
            });
        }

        [Authorize]
        [HttpGet("{action}/{code}")]
        public async Task<ViewResult> RoomTag(string code)
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