using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRS.DAL.Models
{
    public class Otp
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Email { get; set; }
        public DateTime Expiry { get; set; }

        public bool IsUsed { get; set; }

        public int FailedAttempts { get; set; } = 0;

        public DateTime CreatedAt { get; set; }
    }
}
