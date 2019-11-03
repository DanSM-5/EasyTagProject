using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public interface IAppointmentRepository
    {
        IQueryable<Appointment> Appointments { get; }
        void Save(Appointment appointment);
        Appointment Delete(int id);
    }
}
