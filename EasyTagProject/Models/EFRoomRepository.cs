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

            if (room.Id != 0)
            {
                Room roomEntry = context.Rooms.FirstOrDefault(r => r.Id == room.Id);

                if (roomEntry != null)
                {
                    roomEntry.Name = room.Name;
                    roomEntry.Items = roomEntry.Items;
                    roomEntry.Number = room.Number;
                    roomEntry.Type = room.Type;
                    roomEntry.Floor = room.Floor;
                    roomEntry.Block = room.Block;
                    roomEntry.RoomCode = room.RoomCode;
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
                context.Rooms.Remove(room);
                context.SaveChanges();
            }

            return room ?? default;
        }
    }
}
