using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TwittorAPI.Models
{
    public class Car
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }
        [Required]
        public string Type { get; set; }
        public string DisplayName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}