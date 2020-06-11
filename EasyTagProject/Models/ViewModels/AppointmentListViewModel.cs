using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models.ViewModels
{
    // ViewModel to generate AppointmentList View
    public class AppointmentListViewModel
    {
        public DateTime StartRange { get; set; } = DateTime.Parse("08:30:00");
        public DateTime EndRange { get; set; } = DateTime.Parse("22:30:00");
        public DateTime Date { get; set; }
        public Room Room { get; set; }
        public Appointment OverlappedAppointment { get; set; }
        public RoomPagination RoomPagination { get; set; }
        public string ResponsiveTable { get; set; } = "";
        public BtnNewAppViewModel BtnNew { get; set; }
    }
}
