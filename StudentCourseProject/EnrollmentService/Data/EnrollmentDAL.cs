using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnrollmentService.Models;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentService.Data
{
    public class EnrollmentDAL : IEnrollment
    {
        private readonly AppDbContext _db;

        public EnrollmentDAL(AppDbContext db)
        {
            _db = db;
        }
        public Task Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Enrollment>> GetAll()
        {
            return await _db.Enrollments.ToListAsync(); 
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