using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnrollmentService.Dtos
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}