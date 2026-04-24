using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.DAL.Database;
using Microsoft.EntityFrameworkCore;
namespace IRS.DAL.Repository.NotificationRepo
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IRS_Context _context;

        public NotificationRepository(IRS_Context context)
        {
            _context = context;
        }

        public async Task<List<Models.Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.Citizen.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(List<Models.Notification> notifications)
        {
            foreach (var n in notifications)
            {
                n.IsRead = true;
            }
        }

        public async Task DeleteNotificationsAsync(List<int> ids, string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => ids.Contains(n.Id) && n.Citizen.UserId == userId)
                .ToListAsync();

            _context.Notifications.RemoveRange(notifications);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task AddAsync(Models.Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }
    }
}
