using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwittorAPI.Models
{
    public partial class UserData
    {
        public int id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}