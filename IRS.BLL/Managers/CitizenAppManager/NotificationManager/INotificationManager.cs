using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.BLL.Dtos.CitizenDto.NotificationDto;

namespace IRS.BLL.Managers.CitizenAppManager.NotificationManager
{
    public interface INotificationManager
    {
        Task<List<NotificationDto>> GetUserNotificationsAsync(string userId);
        Task<bool> DeleteNotificationsAsync(DeleteNotificationsDto dto, string userId);
    }
}
