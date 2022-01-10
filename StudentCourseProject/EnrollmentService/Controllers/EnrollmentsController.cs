using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EnrollmentService.Data;
using EnrollmentService.Dtos;
using EnrollmentService.Models;
using EnrollmentService.SyncDataServices.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentService.Controllers
{
    // [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentDataClient _dataClient;
        private readonly IEnrollment _enrollment;
        private readonly IMapper _mapper;

        public EnrollmentsController(IEnrollment enrollment, IEnrollmentDataClient dataClient, IMapper mapper)
        {
            _dataClient = dataClient;
            _enrollment = enrollment;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IEnumerable<Enrollment>> GetAll()
        {
            return await _enrollment.GetAll();
        } 

        [HttpPost]
        public async Task<ActionResult> CreateEnrollment(CreateEnrollmentDto enrollmentDto)
        {
            try
            {
                await _dataClient.CreateEnrollmentFromPaymentService(enrollmentDto);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}