using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models
{
    public class EFScheduleRepository: IScheduleRepository
    {
        private ApplicationDbContext context;

        public EFScheduleRepository(ApplicationDbContext ctx) => context = ctx;

        public IQueryable<Schedule> Schedules => context.Schedules
            .Include(s => s.Appointments.Select(a => a));
    }
}
