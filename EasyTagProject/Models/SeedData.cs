using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices.GetRequiredService<ApplicationDbContext>();

            context.Database.Migrate();

            if (!context.Rooms.Any())
            {
                Room room = new Room
                {
                    Block = 'A',
                    Floor = 3,
                    Name = "A3-13",
                    Number = 13,
                    Type = Type.Classroom,
                    Schedule = new Schedule
                    {
                        Appointments = new List<Appointment>
                        {
                            new Appointment
                            {
                                Course = "COMP334",
                                UserName = "Paulo",
                                Start = DateTime.ParseExact("2019-10-09 18:30 PM", "yyyy-MM-dd HH:mm tt",null),
                                End = DateTime.ParseExact("2019-10-09 21:30 PM", "yyyy-MM-dd HH:mm tt",null)
                            }
                        }
                    }
                };

                room.Schedule.Room = room;

                context.AttachRange(room.Schedule.Appointments.Select(a => a));

                context.Rooms.Add(room);

                context.SaveChanges();
            }
        }
    }
}
