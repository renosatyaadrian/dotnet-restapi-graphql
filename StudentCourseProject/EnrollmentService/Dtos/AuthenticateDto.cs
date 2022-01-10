using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnrollmentService.Dtos
{
    public class AuthenticateDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}