using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRS.DAL.Models
{
    public class Citizen
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public string NationalId { get; set; }
        public byte[]? Image { get; set; }
        public bool IsDeleted { get; set; }

        // Relations
        public ICollection<Report>? Reports { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<AdminChat>? AdminChats { get; set; }
    }
}
