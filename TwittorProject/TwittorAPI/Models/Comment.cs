using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TwittorAPI.Models
{
    public class Comment
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string CommentDesc { get; set; }
        public int TwittorId { get; set; }
        public Twittor Twittor { get; set; }

    }
}