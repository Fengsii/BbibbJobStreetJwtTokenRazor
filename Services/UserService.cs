using BbibbJobStreetJwtToken.Helpers;
using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using BbibbJobStreetJwtToken.Models;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using BbibbJobStreetJwtToken.Interfaces;


namespace BbibbJobStreetJwtToken.Services
{
    public class UserService : IUser
    {
        private readonly ApplicationContext _context;
        private readonly JwtHelper _jwtHelper;

        public UserService(ApplicationContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task<string?> LoginAsync(LoginDTO loginDTO)
        {
            try
            {
                // Mencari user berdasarkan username
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == loginDTO.Username);

                if (user == null)
                    return null; // User tidak ditemukan

                // Verifikasi password
                if (!VerifyPassword(loginDTO.Password, user.PasswordHash))
                    return null; // Password salah

                // Generate JWT Token
                var token = _jwtHelper.GenerateToken(user.Username, user.Email, user.Id);
                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoginAsync Error] {ex}");
                throw new Exception("Terjadi kesalahan saat login", ex);
            }
        }

        public async Task<bool> RegisterAsync(RegisterDTO registerDTO)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Cek apakah user sudah ada
                if (await UserExistsAsync(registerDTO.Username, registerDTO.Email))
                    return false;

                // Hash password
                var passwordHash = HashPassword(registerDTO.Password);

                // Buat user baru
                var user = new User
                {
                    Username = registerDTO.Username,
                    Email = registerDTO.Email,
                    PasswordHash = passwordHash,
                    Role = "User",
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"[RegisterAsync Error] {ex}");
                throw new Exception("Terjadi kesalahan saat registrasi", ex);
            }
        }

        public async Task<bool> UserExistsAsync(string username, string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Username == username || u.Email == email);
        }

        public async Task<UserDTO?> GetUserByIdAsync(int userId)
        {
            try
            {
                // Mengambil User entity dari database
                var userEntity = await _context.Users.FindAsync(userId);

                if (userEntity == null)
                    return null;

                // Konversi dari User entity ke UserModel (DTO)
                // PENTING: Tidak mengirim PasswordHash ke client!
                return new UserDTO
                {
                    Id = userEntity.Id,
                    Username = userEntity.Username,
                    Email = userEntity.Email,
                    Password = "",// Kosongkan untuk keamanan
                    CreatedAt = userEntity.CreatedAt
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUserByIdAsync Error] {ex}");
                throw new Exception("Gagal mengambil data user", ex);
            }
        }

      

        // Method untuk hash password menggunakan SHA256 (sederhana untuk contoh)
        // Untuk production, gunakan BCrypt atau Argon2

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password); 
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword); 
        }

       
    }
}
