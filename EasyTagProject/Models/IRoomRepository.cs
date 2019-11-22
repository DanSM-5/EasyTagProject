using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public interface IRoomRepository
    {
        /// <summary>
        /// Returns the collection of rooms in database. Include schedueles and appointments.
        /// </summary>
        IQueryable<Room> Rooms { get; }
        /// <summary>
        /// Save a room in database
        /// </summary>
        /// <param name="room">
        /// Room to ve saved
        /// </param>
        Task SaveAsync(Room room);
        /// <summary>
        /// Delete a room by the specified id
        /// </summary>
        /// <param name="id">
        /// Id of the room to be deleted
        /// </param>
        /// <returns></returns>
        Task<Room> DeleteAsync(int id);
    }
}
