using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IRS.BLL.Managers.AccountManager.Auth
{
    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public string? UserId { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }

        public List<string> Errors { get; set; } = new();
    }
}
