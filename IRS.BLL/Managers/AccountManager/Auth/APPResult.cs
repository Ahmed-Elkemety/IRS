using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IRS.BLL.Managers.AccountManager.Auth
{
    public class APPResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public object? Data { get; set; } 

        public string? Token { get; set; }

        public List<string> Errors { get; set; } = new();
    }
}
