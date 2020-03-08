using AppWithScheduler.Code;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyTagProject.Models.Scheduler.ScheduledTasks
{
    public class ResetEasyTagDbWeeklyTask : IScheduledTask
    {
        private readonly ApplicationDbContext _contex;

        public ResetEasyTagDbWeeklyTask(ApplicationDbContext ctx)
        {
            _contex = ctx;
        }

        public string Schedule => "0 0 * * 1";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                _contex.Appointments.RemoveRange(_contex.Appointments);
                _contex.Schedules.RemoveRange(_contex.Schedules);
                _contex.Rooms.RemoveRange(_contex.Rooms);

                await _contex.Rooms.AddRangeAsync(SeedData.GetRooms());
                await _contex.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Error at cleaning data");
            }
        }
    }
}
