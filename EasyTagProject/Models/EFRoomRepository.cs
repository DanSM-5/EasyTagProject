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
        public IQueryable<Room> Rooms => context.Rooms;
    }
}
