using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models.DB;
using Microsoft.EntityFrameworkCore;
using static BbibbJobStreetJwtToken.Models.GeneralStatus;

namespace BbibbJobStreetJwtToken.Models
{
    public class ApplicationContext : DbContext
    {
        public readonly IEnkripsiPassword _enkripsiPassword;
        public ApplicationContext(DbContextOptions<ApplicationContext> options, IEnkripsiPassword enkripsiPassword) : base(options)
        {
            _enkripsiPassword = enkripsiPassword;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ProfileUserDetails> ProfileUsers { get; set; }
        public virtual DbSet<KategoriPekerjaan> KategoriPekerjaans { get; set; }
        public virtual DbSet<LowonganPekerjaan> LowonganPekerjaans { get; set; }
        public virtual DbSet<Lamaran> Lamarans { get; set; }
        public virtual DbSet<LowonganTersimpan> LowonganTersimpans { get; set; }
        public virtual DbSet<Perusahaan> Perusahaans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Tambahkan admin default
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = _enkripsiPassword.HashPassword("admin123"), // Buat method HashPassword
                    CoverImage = "",
                    ProfileImage = "",
                    Role = "Admin",
                    Posisi = "Administrator",
                    CreatedAt = DateTime.Now,
                    Status = GeneralStatusData.Active
                }
            );

            // Relasi User dengan ProfileUserDetails (One-to-One)
            modelBuilder.Entity<User>()
                .HasOne(u => u.ProfileUserDetails)
                .WithOne(p => p.User)
                .HasForeignKey<ProfileUserDetails>(p => p.UserId);

            // Relasi User dengan Lamaran (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Lamarans)
                .WithOne(l => l.user)
                .HasForeignKey(l => l.UserId);

            // Relasi User dengan LowonganTersimpan (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.lowonganTersimpans)
                .WithOne(lt => lt.User)
                .HasForeignKey(lt => lt.PenggunaId);

            // Relasi KategoriPekerjaan dengan LowonganPekerjaan (One-to-Many)
            modelBuilder.Entity<KategoriPekerjaan>()
                .HasMany(k => k.Lowongans)
                .WithOne(l => l.Kategori)
                .HasForeignKey(l => l.KategoriId);

            // Relasi Perusahaan dengan LowonganPekerjaan (One-to-Many)
            modelBuilder.Entity<Perusahaan>()
                .HasMany(p => p.Lowongans)
                .WithOne(l => l.Perusahaan)
                .HasForeignKey(l => l.PerusahaanId);

            // Relasi LowonganPekerjaan dengan Lamaran (One-to-Many)
            modelBuilder.Entity<LowonganPekerjaan>()
                .HasMany(l => l.Lamarans)
                .WithOne(la => la.Lowongan)
                .HasForeignKey(la => la.LowonganId);

            // Relasi LowonganPekerjaan dengan LowonganTersimpan (One-to-Many)
            modelBuilder.Entity<LowonganPekerjaan>()
                .HasMany(l => l.LowonganTersimpans)
                .WithOne(lt => lt.Lowongan)
                .HasForeignKey(lt => lt.LowonganId);


            base.OnModelCreating(modelBuilder);
        }

      
    }
}