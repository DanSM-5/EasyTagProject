using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models.Notifications
{
    public interface INotificationConnection
    {
        IQueryable<Notification> Notifications { get; }
        Task Create(Notification notification);
        Task<Notification> Delete(int id);



    }
}
