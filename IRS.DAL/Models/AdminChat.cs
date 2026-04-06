using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRS.DAL.Models
{
    public class AdminChat
    {
        public int Id { get; set; }

        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CitizenId { get; set; }
        public Citizen Citizen { get; set; }
    }
}
