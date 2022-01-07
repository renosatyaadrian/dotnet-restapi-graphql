using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TwittorAPI.Models
{
    public class UserRole
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}