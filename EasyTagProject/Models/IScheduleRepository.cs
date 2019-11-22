using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public interface IScheduleRepository
    {
        /// <summary>
        /// Returns the collection of schedules in database. Includes appoinments.
        /// </summary>
        IQueryable<Schedule> Schedules{get;}
    }
}
