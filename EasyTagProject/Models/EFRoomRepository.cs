using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public class EFRoomRepository : IRoomRepository
    {
        private ApplicationDbContext context;
        public EFRoomRepository(ApplicationDbContext ctx) => context = ctx;
        public IQueryable<Room> Rooms => context.Rooms
            .Include(r => r.Schedule)
                .ThenInclude(s => s.Appointments)
            .Include(r => r.Items);


        public void Save(Room room)
        {
            context.AttachRange(room.Schedule.Appointments.Select(a => a));

            if (context.Rooms.Any(r => r.Id == room.Id))
            {
                Room roomEntry = context.Rooms.FirstOrDefault(r => r.Id == room.Id);

                if (roomEntry != null)
                {
                    roomEntry.Name = room.Name;
                    roomEntry.Items = roomEntry.Items;
                    roomEntry.Number = room.Number;
                    roomEntry.Schedule = room.Schedule;
                    roomEntry.Type = room.Type;
                    roomEntry.Floor = room.Floor;
                    roomEntry.Block = room.Block;
                    //roomEntry.RightRoom = room.RightRoom;
                    //roomEntry.LeftRoom = room.LeftRoom;
                }
            }
            else
            {
                context.Rooms.Add(room);
            }

            context.SaveChanges();
        }

        public Room Delete(int id)
        {
            Room room = context.Rooms.FirstOrDefault(r => r.Id == id);

            if (room != null)
            {
                context.Rooms.Add(room);
                context.SaveChanges();
            }

            return room ?? default;
        }
    }
}
