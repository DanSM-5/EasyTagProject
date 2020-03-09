using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Models.Notifications
{
    public class NotificationConnection : INotificationConnection
    {
        ApplicationDbContext context;
        public NotificationConnection(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Notification> Notifications => context.Notifications;

        public async Task Create(Notification notification)
        {
            notification.TimeCreated = DateTime.Now;
            await context.Notifications.AddAsync(notification);
            await context.SaveChangesAsync();
        }

        public async Task<Notification> Delete(int id)
        {
            Notification notification = await context.Notifications.FirstOrDefaultAsync(n => n.Id == id);

            if (notification != null)
            {
                context.Notifications.Remove(notification);
                await context.SaveChangesAsync();
            }

            return notification;
        }

        public async Task UpdateTimeCreated(int id)
        {
            Notification notification = await context.Notifications.FirstOrDefaultAsync(n => n.Id == id);

            if (notification != null)
            {
                notification.TimeCreated = DateTime.Now;
                await context.SaveChangesAsync();
            }
        }
    }
}
