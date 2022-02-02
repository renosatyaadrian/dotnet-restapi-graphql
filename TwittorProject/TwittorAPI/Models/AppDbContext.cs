using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace TwittorAPI.Models
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContext;

        public AppDbContext()
        {    
        }

        public AppDbContext(DbContextOptions<AppDbContext> options,IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContext = httpContextAccessor;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Twittor> Twittors { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Car> Cars { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>()
                .HasKey(b => b.Id)
                .HasName("PrimaryKey_CarId");

            modelBuilder.Entity<Car>()
                .Property(c => c.CreatedDate)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<Car>()
                .Property(p => p.DisplayName)
                .HasComputedColumnSql("[Model] + ' ' + [Type]");
        }
    }
}