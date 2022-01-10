using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Data;
using PaymentService.Dtos;
using PaymentService.Models;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollment _enrollment;
        private readonly IMapper _mapper;

        public EnrollmentsController(IEnrollment enrollment, IMapper mapper)
        {
            _enrollment = enrollment;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> CreateEnrollment(CreateEnrollmentDto enrollment)
        {
            try
            {
                 var enroll = _mapper.Map<Enrollment>(enrollment);
                 await _enrollment.CreateEnrollemnt(enroll);
                 return Ok($"Data enrollment StudentId: {enrollment.StudentId} dan CourseId: {enrollment.CourseId} berhasil ditambahkan");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}