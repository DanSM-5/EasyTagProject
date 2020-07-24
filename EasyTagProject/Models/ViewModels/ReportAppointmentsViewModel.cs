using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models.ViewModels
{
    public class ReportAppointmentsViewModel
    {
        public IEnumerable<Appointment> Appointments { get; set; }
        public BtnNewAppViewModel BtnNew { get; set; }
        public string Date { get; set; }
    }
}
