using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRS.BLL.Dtos.AccountDto.User
{
    public class ConfirmEmailDto
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
