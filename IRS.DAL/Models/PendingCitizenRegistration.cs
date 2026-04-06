using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRS.DAL.Models
{
    public class PendingCitizenRegistration
    {
        public int Id { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }

        public string FullName { get; set; }
        public string NationalId { get; set; }
        public byte[]? Image { get; set; }


        // Audit
        public DateTime CreatedAt { get; set; }
    }

}
