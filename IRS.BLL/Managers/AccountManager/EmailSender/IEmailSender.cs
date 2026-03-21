using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.AccountManager.Email_Sender
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        // Before (string to, string subject, string htmlBody)
        Task SendOtpEmailAsync(string to, string otp);
    }
}