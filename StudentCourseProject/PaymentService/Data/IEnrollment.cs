using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentService.Models;

namespace PaymentService.Data
{
    public interface IEnrollment
    {
        Task CreateEnrollemnt(Enrollment enrollment);
    }
}