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
        private readonly AppDbContext context;

        public PrepDb(AppDbContext context)
        {
            this.context = context;
        }
        public async Task SeedData(bool isProd)
        {
            if(isProd)
            {  
                Console.Write("--> Menjalankan migrasi");
                try
                {
                    await context.Database.MigrateAsync();
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

                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("--> Already have data -->");
            }
        }
    }
}