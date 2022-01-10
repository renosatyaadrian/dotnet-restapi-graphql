using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EnrollmentService.Dtos;
using EnrollmentService.Models;

namespace EnrollmentService.Profiles
{
    public class StudentsProfile : Profile
    {
        public StudentsProfile()
        {
            CreateMap<Student, StudentDto>().ForMember(dest=>dest.FullName, opt=>opt.MapFrom(s => s.FirstName + " " + s.LastName));

            CreateMap<CreateStudentDto, Student>();
        }
    }
}