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
            // Gets the context object for EasyTagDb and runs the migration scripts
            ApplicationDbContext context = app.ApplicationServices.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            // Rxecuted only if the database is empty
            if (!context.Rooms.Any())
            {
                //First room declaration
                Room room = new Room
                {
                    Block = 'A',
                    Floor = 3,
                    RoomCode = "A3-13",
                    Number = 13,
                    Type = Type.Classroom,
                    Schedule = new Schedule
                    {
                        Appointments = new List<Appointment>()
                    }
                };

                Appointment appointment1 = new Appointment
                {
                    Course = "COMP334",
                    UserName = "Paulo",
                    Start = DateTime.ParseExact("2019-11-05 12:30 PM", "yyyy-MM-dd HH:mm tt", null),
                    End = DateTime.ParseExact("2019-11-05 14:30 PM", "yyyy-MM-dd HH:mm tt", null),
                    Description = "Scheduled class of Programming 4"
                };

                Appointment appointment2 = new Appointment
                {
                    Course = "COMP229",
                    UserName = "Andre",
                    Start = DateTime.ParseExact("2019-11-05 18:30 PM", "yyyy-MM-dd HH:mm tt", null),
                    End = DateTime.ParseExact("2019-11-05 21:30 PM", "yyyy-MM-dd HH:mm tt", null),
                    Description = "Scheduled class of Advance Web Development"
                };

                room.Schedule.Room = room;
                room.Schedule.Appointments.Add(appointment1);
                room.Schedule.Appointments.Add(appointment2);

                //Second room declaration
                Room secondRoom = new Room {
                    Block = 'B',
                    Floor = 2,
                    RoomCode = "A2-17",
                    Number = 17,
                    Type = Type.Classroom,
                    Schedule = new Schedule()
                };
                secondRoom.Schedule.Room = secondRoom;

                context.AttachRange(room.Schedule.Appointments.Select(a => a));
                context.Rooms.AddRange(room, secondRoom);
                context.SaveChanges();
            }
        }
    }
}
