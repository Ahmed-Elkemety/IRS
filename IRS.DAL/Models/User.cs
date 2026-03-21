using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IRS.DAL.Models;
using Microsoft.AspNetCore.Identity;


namespace IRS.DAL.Models
{
    public class User:IdentityUser
    {
        public Citizen Citizen { get; set; }
        public Authority Authority { get; set; }
    }
}
