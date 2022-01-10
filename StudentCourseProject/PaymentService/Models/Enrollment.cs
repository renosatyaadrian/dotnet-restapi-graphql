using System;
using System.Collections.Generic;

#nullable disable

namespace PaymentService.Models
{
    public partial class Enrollment
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int Studentid { get; set; }
        public int Grade { get; set; }

        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }
}
