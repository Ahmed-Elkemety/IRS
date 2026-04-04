using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRS.BLL.Dtos.AccountDto.User
{
    public class RegisterAuthorityDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public int DeptId { get; set; }
    }
}
