using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PaymentService.Dtos;
using PaymentService.Models;

namespace PaymentService.Profiles
{
    public class EnrollmentProfile : Profile
    {
        public EnrollmentProfile()
        {
            CreateMap<CreateEnrollmentDto, Enrollment>();
        }
    }
}