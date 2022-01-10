using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnrollmentService.Data;
using EnrollmentService.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmensController : ControllerBase
    {
        private readonly IEnrollment _enrollment;

        public EnrollmensController(IEnrollment enrollment)
        {
            _enrollment = enrollment;
        }
        [HttpGet]
        public async Task<IEnumerable<Enrollment>> GetAll()
        {
            return await _enrollment.GetAll();
        } 

    }
}