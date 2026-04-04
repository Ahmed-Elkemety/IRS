using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRS.DAL.Models
{
    public class pendingAuthorityRegistrations
    {
        public int Id { get; set; }

        // بيانات الـ User (مؤقتة)
        public string Email { get; set; }
        public string Password { get; set; }

        // بيانات Authority
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int DeptId { get; set; }


        // Audit
        public DateTime CreatedAt { get; set; }
    }
}
