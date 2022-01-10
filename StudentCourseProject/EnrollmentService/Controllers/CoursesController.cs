using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EnrollmentService.Data;
using EnrollmentService.Dtos;
using EnrollmentService.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourse _course;
        private readonly IMapper _mapper;

        public CoursesController(ICourse course, IMapper mapper)
        {
            _course = course;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> Get()
        {
            var courses = await _course.GetAll();
            var dtos = _mapper.Map<IEnumerable<CourseDto>>(courses);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> Get(int id)
        {
            var course = await _course.GetById(id);
            var dtos = _mapper.Map<CourseDto>(course);
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<ActionResult<CourseDto>> Post([FromBody] CreateCourseDto courseDto)
        {
            try
            {
                var dto = _mapper.Map<Course>(courseDto);
                var result = await _course.Insert(dto);
                return Ok(_mapper.Map<CourseDto>(result));  
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CourseDto>> Put(int id, [FromBody] CreateCourseDto courseDto)
        {
            try
            {
                var dto = _mapper.Map<Course>(courseDto);
                var result = await _course.Update(id, dto);
                var resultDto = _mapper.Map<CourseDto>(result);
                return Ok(resultDto);  
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _course.Delete(id);
                return Ok($"Delete data course id: {id}, berhasil");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}