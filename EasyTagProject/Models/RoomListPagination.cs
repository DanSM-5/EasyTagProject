using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public class RoomListPagination
    {
        //[BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 3;

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count,PageSize));

        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;
        public bool ShowFirst => CurrentPage != 1;
        public bool ShowLast => CurrentPage != TotalPages;
    }
}
