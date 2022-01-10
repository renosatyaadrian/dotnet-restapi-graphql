using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnrollmentService.Models;
using Microsoft.EntityFrameworkCore;

namespace EnrollmentService.Data
{
    public class CourseDAL : ICourse
    {
        private readonly AppDbContext _db;

        public CourseDAL(AppDbContext db)
        {
            _db = db;
        }
        public async Task Delete(int Id)
        {
            var course = await GetById(Id);
            try
            {
                 _db.Courses.Remove(course);
                 await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Course>> GetAll()
        {
            return await _db.Courses.ToListAsync();
        }

        public async Task<Course> GetById(int Id)
        {
            var course = await _db.Courses.Where(c=>c.Id == Id).SingleOrDefaultAsync();
            if(course == null) throw new Exception("Course tidak ditemukan");
            return course;
        }

        public async Task<Course> Insert(Course Obj)
        {
            try
            {
                  _db.Courses.Add(Obj);
                 await _db.SaveChangesAsync();
                 return Obj;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task<Course> Update(int Id, Course Obj)
        {
            var course = await GetById(Id);
            try
            {
                 course.Title = Obj.Title;
                 course.Credits = Obj.Credits;
                 course.Price = Obj.Price;
                 await _db.SaveChangesAsync();
                 return course;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}