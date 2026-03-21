using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRS.BLL.Dtos.AccountDto.Forget_password
{
    public class VerifyOtpDto
    {
        public string Email { get; set; }
        public string Code { get; set; } 
    }
}
