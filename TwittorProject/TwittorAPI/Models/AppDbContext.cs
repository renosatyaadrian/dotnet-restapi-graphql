using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TwittorAPI.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {    
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Twittor> Twittors { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
    }
}