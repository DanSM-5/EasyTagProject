using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public class RoomPagination
    {
        public DateTime Yesterday => CurrentDate.AddDays(-1);
        [BindProperty(SupportsGet = true)]
        public DateTime CurrentDate { get; set; } = DateTime.Today;
        public DateTime Tomorrow => CurrentDate.AddDays(1);
    }
}
