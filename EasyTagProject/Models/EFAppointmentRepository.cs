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

        public async Task SaveAsync(Appointment appointment)
        {
            if (appointment.Id != 0)
            {
                Appointment entry = await context.Appointments.FirstOrDefaultAsync(r => r.Id == appointment.Id);

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
                Room room = await Rooms.FirstOrDefaultAsync(r => r.Id == appointment.RoomId);

                if (room != null)
                {
                    room.Schedule.Appointments.Add(appointment);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task<Appointment> DeleteAsync(int id)
        {
            Appointment app = await context.Appointments.FirstOrDefaultAsync(a => a.Id == id);

            if (app != null)
            {
                context.Appointments.Remove(app);
                await context.SaveChangesAsync();
            }

            return app;
        }
    }
}
