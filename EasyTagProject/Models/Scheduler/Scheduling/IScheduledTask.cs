using System.Threading;
using System.Threading.Tasks;

namespace AppWithScheduler.Code
{
    public interface IScheduledTask
    {
        string Schedule { get; }
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}