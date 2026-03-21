using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRS.DAL.Models
{
    public class Authority
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public int DeptId { get; set; }
        public Department Department { get; set; }

        // Relations
        public ICollection<Report> Reports { get; set; }
    }
}
