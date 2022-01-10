using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnrollmentService.Models
{
    public class Enrollment
    {
        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        public int CourseId { get; set; }
        
        [Required]
        public int Studentid { get; set; }
        
        [Required]
        public int Grade { get; set; }
        
        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }

    public enum Grade
    {
        A,B,C,D,F
    }

}