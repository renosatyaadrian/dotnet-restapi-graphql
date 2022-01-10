using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnrollmentService.Models;

namespace EnrollmentService.Data
{
    public class EnrollmentDAL : IEnrollment
    {
        public Task Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Enrollment>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Enrollment> GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<Enrollment> Insert(Enrollment Obj)
        {
            throw new NotImplementedException();
        }

        public Task<Enrollment> Update(int Id, Enrollment Obj)
        {
            throw new NotImplementedException();
        }
    }
}