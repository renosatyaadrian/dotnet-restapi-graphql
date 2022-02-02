using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwittorAPI.Models
{
    public class User
    {
        [Key]
        [Required]
        [Column(Order = 0)]
        public int Id { get; set; }
        [Required]
        [MaxLength(32)]
        [Column(Order = 1)]
        public string Username { get; set; }
        [Required]
        [MaxLength(100)]
        [Comment("Fullname Users")]
        [Column(Order = 4)]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        [Column(Order = 3)]
        public string Email { get; set; }
        [Required]
        [Column(Order = 2)]
        public string Password { get; set; }
        public bool IsLocked { get; set; }
        [Required]
        public DateTime Created { get; set; }
        public virtual ICollection<Twittor> Twittors { get; set; }
    }
}