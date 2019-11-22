using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models.ViewModels
{
    // ViewModel to generate Room View
    public class RoomViewModel
    {
        public Room Room { get; set; }
        public RoomPagination Pagination { get; set; }
    }
}
