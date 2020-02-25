using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    /*
     * Model class for pagination of Room page
     */
    public class RoomPagination
    {
        public DateTime Yesterday => CurrentDate.AddDays(-1);
        public DateTime CurrentDate { get; set; } = DateTime.Today;
        public DateTime Tomorrow => CurrentDate.AddDays(1);
        public string RoomCode { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public bool AllowPrevious { get; set; } = true;
        public bool AllowCurrent { get; set; } = true;
        public bool AllowNext { get; set; } = true;
    }
}
