using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TwittorAPI.Models
{
    public class Twittor
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = ("varchar(250)"))]
        public string Twit { get; set; }
        [Required]
        public DateTime Created { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}