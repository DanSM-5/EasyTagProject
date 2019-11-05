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


        public async Task SaveAsync(Room room)
        {
            context.AttachRange(room.Schedule.Appointments.Select(a => a));

            if (room.Id != 0)
            {
                Room roomEntry = await context.Rooms.FirstOrDefaultAsync(r => r.Id == room.Id);

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
                await context.Rooms.AddAsync(room);
            }

            await context.SaveChangesAsync();
        }

        public async Task<Room> DeleteAsync(int id)
        {
            Room room = await context.Rooms.FirstOrDefaultAsync(r => r.Id == id);

            if (room != null)
            {
                context.Rooms.Remove(room);
                await context.SaveChangesAsync();
            }

            return room ?? default;
        }
    }
}
