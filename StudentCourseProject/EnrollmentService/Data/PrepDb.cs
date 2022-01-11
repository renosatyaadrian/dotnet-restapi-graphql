using System;
using System.Linq;
using EnrollmentService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnrollmentService.Data
{
    public static class PrepDb
    {
        public static void PrePopulation(IApplicationBuilder app, bool isProd)
        {
            using(var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
            };
        }

        private static void SeedData(AppDbContext context, bool isProd)
        {
            if(isProd)
            {  
                Console.Write("--> Menjalankan migrasi");
                try
                {
                     context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Gagal Menjalankan Migrasi dengan error: {ex.Message}");
                }
            }

            if(!context.Students.Any())
            {
                Console.WriteLine("--> Seeding data Student -->");
                context.Students.AddRange(
                    new Student(){ FirstName = "Reno" , LastName = "Adrian", EnrollmentDate = DateTime.Now },
                    new Student(){ FirstName = "Ilham" , LastName = "Udin", EnrollmentDate = DateTime.Now },
                    new Student(){ FirstName = "Meri" , LastName = "Krismas", EnrollmentDate = DateTime.Now }
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> Already have Students data -->");
            }

            if(!context.Courses.Any())
            {
                Console.WriteLine("--> Seeding data Student -->");
                context.Courses.AddRange(
                    new Course(){ Title = "ASP.NET Core Fundamental", Credits = 2, Price = 2000.00 },
                    new Course(){ Title = "ASP.NET Core Menengah", Credits = 3, Price = 4000.00 },
                    new Course(){ Title = "ASP.NET Core Lanjut", Credits = 4, Price = 6000.00 },
                    new Course(){ Title = "Docker Fundamental", Credits = 2, Price = 2000.00 },
                    new Course(){ Title = "Kubernetes Fundamental", Credits = 2, Price = 3000.00 }
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> Already have Courses data -->");
            }
            
            if(!context.Enrollments.Any())
            {
                Console.WriteLine("--> Seeding data Student -->");
                context.Enrollments.AddRange(
                    new Enrollment(){ Studentid = 1, CourseId = 1, Grade = 1 },
                    new Enrollment(){ Studentid = 2, CourseId = 2, Grade = 1 },
                    new Enrollment(){ Studentid = 3, CourseId = 3, Grade = 1 }
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> Already have Enrollments data -->");
            }
        }
    }
}