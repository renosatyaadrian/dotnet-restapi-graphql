using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnrollmentService.Dtos
{
    public class CreateEnrollmentDto
    {
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public int Grade { get; set; }
    }
}