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

            // Executed only if the database is empty
            if (!context.Rooms.Any())
            {
                context.Rooms.AddRange(
                    new Room {
                        Block = 'A',
                        Floor = 3,
                        RoomCode = "A3-13",
                        Number = 13,
                        Type = Type.Classroom,
                        Schedule = new Schedule()
                    },
                    new Room
                    {
                        Block = 'B',
                        Floor = 2,
                        RoomCode = "B2-17",
                        Number = 17,
                        Type = Type.Classroom,
                        Schedule = new Schedule()
                    },
                    new Room {
                        Block = 'C',
                        Floor = 2,
                        RoomCode = "C2-17",
                        Number = 17,
                        Type = Type.Classroom,
                        Schedule = new Schedule()
                    },
                    new Room
                    {
                        Block = 'A',
                        Floor = 3,
                        RoomCode = "A3-15",
                        Number = 15,
                        Type = Type.Classroom,
                        Schedule = new Schedule()
                    },
                    new Room
                    {
                        Block = 'A',
                        Floor = 3,
                        RoomCode = "A3-17",
                        Number = 17,
                        Type = Type.Classroom,
                        Schedule = new Schedule()
                    },
                    new Room
                    {
                        Block = 'D',
                        Floor = 2,
                        RoomCode = "D2-17",
                        Number = 17,
                        Type = Type.Classroom,
                        Schedule = new Schedule()
                    },
                    new Room
                    {
                        Block = 'A',
                        Floor = 2,
                        RoomCode = "A2-12",
                        Number = 12,
                        Type = Type.Classroom,
                        Schedule = new Schedule()
                    },
                    new Room
                    {
                        Block = 'A',
                        Floor = 1,
                        RoomCode = "A1-17",
                        Number = 17,
                        Type = Type.Classroom,
                        Schedule = new Schedule()
                    },
                    new Room
                    {
                        Block = 'C',
                        Floor = 3,
                        RoomCode = "C3-05",
                        Number = 5,
                        Type = Type.Classroom,
                        Schedule = new Schedule()
                    },
                    new Room
                    {
                        Block = 'C',
                        Floor = 3,
                        RoomCode = "C3-04",
                        Number = 4,
                        Type = Type.Classroom,
                        Schedule = new Schedule()
                    },
                    new Room
                    {
                        Block = 'E',
                        Floor = 2,
                        RoomCode = "E2-11",
                        Number = 11,
                        Type = Type.Classroom,
                        Schedule = new Schedule()
                    },
                    new Room
                    {
                        Block = 'B',
                        Floor = 1,
                        RoomCode = "B1-10",
                        Number = 10,
                        Type = Type.Classroom,
                        Schedule = new Schedule()
                    },
                    new Room
                    {
                        Block = 'D',
                        Floor = 3,
                        RoomCode = "D3-11",
                        Number = 11,
                        Type = Type.Classroom,
                        Schedule = new Schedule()
                    },
                    new Room
                    {
                        Block = 'E',
                        Floor = 2,
                        RoomCode = "E2-09",
                        Number = 9,
                        Type = Type.Classroom,
                        Schedule = new Schedule()
                    },
                    new Room
                    {
                        Block = 'E',
                        Floor = 1,
                        RoomCode = "E1-02",
                        Number = 2,
                        Type = Type.Classroom,
                        Schedule = new Schedule()
                    });
                context.SaveChanges();
            }
        }
    }
}
