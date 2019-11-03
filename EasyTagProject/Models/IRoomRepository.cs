using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public interface IRoomRepository
    {
        IQueryable<Room> Rooms { get; }
        /// <summary>
        /// Save a room
        /// </summary>
        /// <param name="room">
        /// Room to ve saved
        /// </param>
        void Save(Room room);
        Room Delete(int id);
    }
}
