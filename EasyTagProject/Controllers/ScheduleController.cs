using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyTagProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace EasyTagProject.Controllers
{
    public class ScheduleController : Controller
    {
        IRoomRepository roomRepository;

        public ScheduleController(IRoomRepository rRepo) => roomRepository = rRepo;

        public ViewResult AddAppointment(int roomId, string range)
        {
            return View();
        }
    }
}