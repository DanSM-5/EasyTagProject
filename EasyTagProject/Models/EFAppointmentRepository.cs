using Microsoft.EntityFrameworkCore;
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
        private IQueryable<Room> Rooms => context.Rooms
            .Include(r => r.Schedule);
                //.ThenInclude(s => s.Appointments);

        public void Save(Appointment appointment)
        {
            if (appointment.Id != 0)
            {
                Appointment entry = context.Appointments.FirstOrDefault(r => r.Id == appointment.Id);

                if (entry != null)
                {
                    entry.Start = appointment.Start;
                    entry.End = appointment.End;
                    entry.Description = appointment.Description;
                    entry.Course = appointment.Course;
                    entry.UserName = appointment.UserName;
                }
            }
            else
            {
                Room room = Rooms.FirstOrDefault(r => r.Id == appointment.RoomId);

                if (room != null)
                {
                    room.Schedule.Appointments.Add(appointment);
                }
            }

            context.SaveChanges();
        }

        public Appointment Delete(int id)
        {
            Appointment app = context.Appointments.FirstOrDefault(a => a.Id == id);

            if (app != null)
            {
                context.Appointments.Remove(app);
                context.SaveChanges();
            }

            return app;
        }
    }
}
