using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public interface IScheduleRepository
    {
        IQueryable<Schedule> Schedules{get;}
    }
}
