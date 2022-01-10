using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnrollmentService.Dtos
{
    public class CreateStudentDto
    {
        [Required(ErrorMessage = "First name cannot be empty")]
        [MaxLength(255, ErrorMessage = "First name cannot excess 255 character")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name cannot be empty")]
        [MaxLength(255, ErrorMessage = "Last name cannot excess 255 character")]
        public string LastName { get; set; }
        
        [Required]
        public DateTime EnrollmentDate { get; set; }
    }
}