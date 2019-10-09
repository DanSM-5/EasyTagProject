using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    interface IAppointmentRepository
    {
        IQueryable<Appointment> Appointments { get; }
    }
}
