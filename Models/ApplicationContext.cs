using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace BbibbJobStreetJwtToken.Models
{
    public class ApplicationContext : DbContext
    {
       
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            
        }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Tambahkan admin default
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = HashPassword("admin123"), // Buat method HashPassword
                    Role = "Admin",
                    CreatedAt = DateTime.Now
                }
            );

            base.OnModelCreating(modelBuilder);
        }

        private string HashPassword(string password)
        {
            // Implementasi hashing password (gunakan BCrypt atau metode secure lainnya)
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

    }
}