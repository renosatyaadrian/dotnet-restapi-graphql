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
    public class StudentsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStudent _student;

        public StudentsController(IStudent student, IMapper mapper)
        {
            _mapper = mapper;
            _student = student;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetAllStudents()
        {
            var students = await _student.GetAll();
            var dtos = _mapper.Map<IEnumerable<StudentDto>>(students);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudentById(int id)
        {
            var student = await _student.GetById(id);
            var dtos = _mapper.Map<StudentDto>(student);
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<ActionResult<StudentDto>> CreateStudent([FromBody] CreateStudentDto studentCreateDto)
        {
            try
            {
                var student = _mapper.Map<Student>(studentCreateDto);
                var result = await _student.Insert(student);
                var studentDtos = _mapper.Map<StudentDto>(result);
                return Ok(studentDtos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StudentDto>> UpdateStudent(int id, [FromBody] CreateStudentDto studentUpdateDto)
        {
            try
            {
                var student = _mapper.Map<Student>(studentUpdateDto);
                var result = await _student.Update(id, student);
                var studentDtos = _mapper.Map<StudentDto>(result);
                return Ok(studentDtos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
           try
           {
                await _student.Delete(id);
                return Ok($"Data student id : {id}, berhasil dihapus");
           }
           catch (Exception ex)
           {
               return BadRequest($"Error: {ex.Message}");
           } 
        }
    }
}