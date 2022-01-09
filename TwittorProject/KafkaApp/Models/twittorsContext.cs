using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace KafkaApp.Models
{
    public partial class twittorsContext : DbContext
    {
        public twittorsContext()
        {
        }

        public twittorsContext(DbContextOptions<twittorsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Twittor> Twittors { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                   .AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();
            optionsBuilder.UseSqlServer(config["Settings:Database"]);

            //            if (!optionsBuilder.IsConfigured)
            //            {
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
            //                optionsBuilder.UseSqlServer("Server=localhost;database=ShoppingCart;uid=student;pwd=pass123");
            //            }
            // if (!optionsBuilder.IsConfigured)
//             {
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                 optionsBuilder.UseSqlServer("Server=localhost,1433;Database=twittors;MultipleActiveResultSets=true;User=sa;Password=Swordman89!@#;");
//             }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasIndex(e => e.TwittorId, "IX_Comments_TwittorId");

                entity.Property(e => e.CommentDesc).IsRequired();

                entity.HasOne(d => d.Twittor)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.TwittorId);
            });

            modelBuilder.Entity<Twittor>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_Twittors_UserId");

                entity.Property(e => e.Twit).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Twittors)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasDefaultValueSql("(N'')");

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Username).IsRequired();
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_UserRoles_RoleId");

                entity.HasIndex(e => e.UserId, "IX_UserRoles_UserId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
