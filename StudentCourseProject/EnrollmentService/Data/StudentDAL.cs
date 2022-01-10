using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnrollmentService.Dtos;
using EnrollmentService.Models;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentService.Data
{
    public class StudentDAL : IStudent
    {
        private readonly AppDbContext _dbContext;

        public StudentDAL(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Delete(int Id)
        {
            var student = await GetById(Id);
            try
            {
                _dbContext.Students.Remove(student);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Student>> GetAll()
        {
            return await _dbContext.Students.ToListAsync();
        }

        public async Task<Student> GetById(int Id)
        {
            var student = await _dbContext.Students.Where(s=>s.Id == Id).SingleOrDefaultAsync();
            if(student == null) throw new Exception("Data student tidak ditemukan");
            else return student;
        }

        public async Task<Student> Insert(Student Obj)
        {
            try
            {
                _dbContext.Add(Obj);
                await _dbContext.SaveChangesAsync();
                return Obj;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task<Student> Update(int Id, Student Obj)
        {
            var student = await GetById(Id);
            try
            {
                student.FirstName = Obj.FirstName;
                student.LastName = Obj.LastName;
                student.EnrollmentDate = Obj.EnrollmentDate;
                await _dbContext.SaveChangesAsync();
                return Obj;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}