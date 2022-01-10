using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EnrollmentService.Dtos;
using EnrollmentService.Models;

namespace EnrollmentService.Profiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseDto>()
                .ForMember(des=> des.TotalHours, opt=>opt.MapFrom(src=>src.Credits*1.5));
                
            CreateMap<CreateCourseDto, Course>();
        }
    }
}