using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnrollmentService.Dtos
{
    public class CreateCourseDto
    {
        [Required(ErrorMessage = "Title tidak boleh kosong")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Credit tidak boleh kosong")]
        public int Credits { get; set; }

        [Required(ErrorMessage = "Price tidak boleh kosong")]
        public double Price { get; set; }
    }
}