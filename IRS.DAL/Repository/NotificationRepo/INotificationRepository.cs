using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.DAL.Models;

namespace IRS.DAL.Repository.NotificationRepo
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetUserNotificationsAsync(string userId);
        Task MarkAsReadAsync(List<Notification> notifications);
        Task DeleteNotificationsAsync(List<int> ids, string userId);
        Task SaveChangesAsync();

        Task AddAsync(Notification notification);

    }
}
