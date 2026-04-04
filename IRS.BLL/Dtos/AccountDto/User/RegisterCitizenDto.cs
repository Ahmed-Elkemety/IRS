using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace IRS.BLL.Dtos.AccountDto.User
{
    public class RegisterCitizenDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string NationalId { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public byte[]? Image { get; set; }
    }
}
