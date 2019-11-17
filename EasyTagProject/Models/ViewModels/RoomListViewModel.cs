using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models.ViewModels
{
    public class RoomListViewModel
    {
        public IEnumerable<Room> Rooms { get; set; }
        public RoomListPagination Pagination { get; set; }
        public string SearchString { get; set; }
    }
}
