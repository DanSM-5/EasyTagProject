using AppWithScheduler.Code;
using EasyTagProject.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyTagProject.Models.Scheduler.ScheduledTasks
{
    public class ResetIdentityDbWeeklyTask : IScheduledTask
    {
        private readonly ETIdentityDbContext _contex;

        public ResetIdentityDbWeeklyTask(ETIdentityDbContext ctx)
        {
            _contex = ctx;
        }

        public string Schedule => "5 0 * * 1";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Plan later
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Error at cleaning data");
            }
        }
    }
}
