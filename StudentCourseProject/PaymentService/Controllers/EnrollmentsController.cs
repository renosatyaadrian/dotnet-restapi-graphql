using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Data;
using PaymentService.Models;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollment _enrollment;

        public EnrollmentsController(IEnrollment enrollment)
        {
            _enrollment = enrollment;
        }

        [HttpPost]
        public async Task<ActionResult> CreateEnrollment(Enrollment enrollment)
        {
            try
            {
                 await _enrollment.CreateEnrollemnt(enrollment);
                 return Ok($"Data enrollment StudentId: {enrollment.Studentid} dan CourseId: {enrollment.CourseId} berhasil ditambahkan");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}