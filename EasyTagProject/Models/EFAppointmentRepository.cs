using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public class EFAppointmentRepository : IAppointmentRepository
    {
        ApplicationDbContext context;
        public EFAppointmentRepository(ApplicationDbContext ctx) => context = ctx;
        public IQueryable<Appointment> Appointments => context.Appointments;
    }
}
