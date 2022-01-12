using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TwittorAPI.Models;

namespace TwittorAPI.Data
{
    public class PrepDb
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

            if(!context.Users.Any())
            {
                Console.WriteLine("--> Seeding data -->");
                context.Users.AddRange(
                    new User(){ Username = "renosatyaadrian", Password = "Renoreno123", FullName = "Reno Satya", Email = "renosatyaadrian@gmail.com", IsLocked = false, Created = DateTime.Now },
                    new User(){ Username = "rezaaditya", Password = "Renoreno123", FullName = "Reza Aditya", Email = "rezaaditya@gmail.com", IsLocked = false, Created = DateTime.Now }
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> Already have data -->");
            }
        }
    }
}