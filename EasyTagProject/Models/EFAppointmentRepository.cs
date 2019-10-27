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

        public void Save(Appointment appointment)
        {
            if (context.Appointments.Any(r => r.Id == appointment.Id))
            {
                Appointment entry = context.Appointments.FirstOrDefault(r => r.Id == appointment.Id);

                if (entry != null)
                {
                    entry.Start = appointment.Start;
                    entry.End = appointment.End;
                    entry.Description = entry.Description;
                    entry.Course = entry.Course;
                    entry.UserName = entry.UserName;
                }
            }
            else
            {
                context.Appointments.Add(appointment);
            }

            context.SaveChanges();
        }
    }
}
