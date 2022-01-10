using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnrollmentService.Dtos
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public float TotalHours { get; set; }
        public double Price { get; set; }
    }
}