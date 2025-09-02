using BbibbJobStreetJwtToken.Helpers;
using BbibbJobStreetJwtToken.Interfaces;
using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using X.PagedList;
using static BbibbJobStreetJwtToken.Models.GeneralStatus;


namespace BbibbJobStreetJwtToken.Services
{
    public class UserService : IUser
    {
        private readonly ApplicationContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly IEnkripsiPassword _enkripsiPassword;
        private readonly ILogger<UserService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IImageHelper _imageHelper;
        private readonly IEmailHelper _emailHelper;

        public UserService(ApplicationContext context, JwtHelper jwtHelper, IEnkripsiPassword enkripsiPassword, ILogger<UserService> logger, IHttpContextAccessor httpContextAccessor, IImageHelper imageHelper, IEmailHelper emailHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _enkripsiPassword = enkripsiPassword;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _imageHelper = imageHelper;
            _emailHelper = emailHelper;
        }







        public async Task<bool> SendOtpAsync(string emailOrUsername)
        {
            try
            {
                emailOrUsername = emailOrUsername.Trim().ToLower();
                Console.WriteLine($"[SEND OTP] EmailOrUsername masuk: '{emailOrUsername}'");

                var user = await _context.Users
                    .FirstOrDefaultAsync(x =>
                        x.Email.ToLower() == emailOrUsername || x.Username.ToLower() == emailOrUsername);


                if (user == null)
                {
                    Console.WriteLine("[SEND OTP] User tidak ditemukan di DB.");
                    return false;
                }
                else
                {
                    Console.WriteLine($"[SEND OTP] User ditemukan: {user.Username}, {user.Email}");
                }



                // Hapus OTP lama jika sudah expired
                if (user.OtpExpiredAt != null && user.OtpExpiredAt < DateTime.Now)
                {
                    user.OtpCode = null;
                    user.OtpExpiredAt = null;
                }

                // Generate OTP baru
                var otp = new Random().Next(100000, 999999).ToString();
                user.OtpCode = otp;
                user.OtpExpiredAt = DateTime.Now.AddMinutes(10);

                await _context.SaveChangesAsync();

                // Kirim OTP ke email user
                var sent = await _emailHelper.SendOtpEmailAsync(user.Email, otp);
                return sent;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Terjadi kesalahan saat menyimpan OTP ke database.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Terjadi kesalahan umum saat mengirim OTP.");
                return false;
            }
        }

        public async Task<bool> ResetPasswordWithOtpAsync(VerifyOtpAndResetPasswordDTO dto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u =>
                    (u.Email.ToLower() == dto.EmailOrUsername.ToLower() || u.Username.ToLower() == dto.EmailOrUsername.ToLower())
                    && u.Status != GeneralStatusData.Delete && u.Status != GeneralStatusData.Unactive);

                if (user == null)
                    return false;

                if (user.OtpCode != dto.OtpCode)
                    return false;

                if (user.OtpExpiredAt == null || user.OtpExpiredAt < DateTime.Now)
                    return false;

                user.PasswordHash = _enkripsiPassword.HashPassword(dto.NewPassword);

                // Hapus OTP setelah berhasil reset password
                user.OtpCode = null;
                user.OtpExpiredAt = null;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Terjadi kesalahan saat menyimpan password baru ke database.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Terjadi kesalahan umum saat mereset password.");
                return false;
            }
        }


        public async Task<string?> LoginAsync(LoginUserDTO loginDTO)
        {
            try
            {
                var user = await _context.Users.Where(u => u.Status != GeneralStatusData.Unactive && u.Status != GeneralStatusData.Delete)
                    .FirstOrDefaultAsync(u => u.Username == loginDTO.Username);

                if (user == null)
                    return null; // User tidak ditemukan

                // Verifikasi password
                if (!_enkripsiPassword.VerifyPassword(loginDTO.Password, user.PasswordHash))
                    return null; // Password salah

                var token = _jwtHelper.GenerateToken(user.Username, user.Email, user.Id, user.Role);
                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoginAsync Error] {ex}");
                throw new Exception("Terjadi kesalahan saat login", ex);
            }
        }

        public async Task<bool> RegisterAsync(RegisterUserDTO registerDTO)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Cek apakah user sudah ada
                if (await UserExistsAsync(registerDTO.Username, registerDTO.Email))
                    return false;

                // Hash password
                var passwordHash = _enkripsiPassword.HashPassword(registerDTO.Password);

                var user = new User
                {
                    Username = registerDTO.Username,
                    Email = registerDTO.Email,
                    PasswordHash = passwordHash,
                    ProfileImage = "",
                    CoverImage = "",
                    Role = "User",
                    Posisi = registerDTO.Posisi,
                    CreatedAt = DateTime.Now,
                    Status = GeneralStatus.GeneralStatusData.Active
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

        public async Task<UserUpdateDTO?> GetUserByIdAsync(int userId)
        {
            try
            {
                // Mengambil User entity dari database
                var userEntity = await _context.Users.FindAsync(userId);

                if (userEntity == null)
                    return null;

                // Konversi dari User entity ke UserModel (DTO)
                // PENTING: Tidak mengirim PasswordHash ke client!
                return new UserUpdateDTO
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




        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        // Method baru untuk mendapatkan profil user yang sedang login
        public UserProfileViewDTO GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            var data = _context.Users
                .Where(x => x.Status != GeneralStatus.GeneralStatusData.Delete && x.Id == userId)
                .Select(x => new UserProfileViewDTO
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email,
                    Role = x.Role,
                    Posisi = x.Posisi,
                    ProfileImage = "/upload/" + Path.GetFileName(x.ProfileImage),
                    CoverImage = "/upload/" + Path.GetFileName(x.CoverImage),
                    CreatedAt = x.CreatedAt,
                    Status = x.Status.ToString()
                })
                .FirstOrDefault();

            return data ?? new UserProfileViewDTO();
        }

        // Method baru untuk mengupdate profil user yang sedang login
        public bool UpdateUserProfile(UserProfileUpdateDTO dto)
        {
            var userId = GetCurrentUserId();
            var data = _context.Users
                .FirstOrDefault(x => x.Id == dto.Id && x.Id == userId);

            if (data == null)
            {
                return false;
            }

           
            // Update password hanya kalau user isi password baru
            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                data.PasswordHash = _enkripsiPassword.HashPassword(dto.NewPassword);
            }

            // Update profile image jika ada file baru
            if (dto.ProfileImage != null && dto.ProfileImage.Length > 0)
            {
                data.ProfileImage = _imageHelper.Save(dto.ProfileImage, "upload");
            }

            // Update cover image jika ada file baru
            if (dto.CoverImage != null && dto.CoverImage.Length > 0)
            {
                data.CoverImage = _imageHelper.Save(dto.CoverImage, "upload");
            }

            data.Username = dto.Username;
            data.Email = dto.Email;
            data.Posisi = dto.Posisi;
            data.LastUpdatedAt = DateTime.Now;

            _context.Users.Update(data);
            _context.SaveChanges();
            return true;
        }



        ///====================== Untuk Admin ==========================\\\
        public IPagedList<UserUpdateDTO> GetListUsers(int page, int pageSize, string searchTerm = "")
        {
            var query = _context.Users
                .Where(x => x.Status != GeneralStatusData.Delete && x.Id != 1)
                .Select(x => new UserUpdateDTO
                {
                    Id = x.Id,
                    ProfileImage = "/upload/" + Path.GetFileName(x.ProfileImage),
                    Username = x.Username,
                    Email = x.Email,
                    Posisi = x.Posisi,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt
                });

            // Tambahkan pencarian
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x =>
                    x.Username.Contains(searchTerm) ||
                    x.Posisi.Contains(searchTerm));
            }

            return query.OrderBy(x => x.Username)
                       .ToPagedList(page, pageSize);
        }

        public List<UserUpdateDTO> GetListUsers()
        {
            return _context.Users
                .Where(x => x.Status != GeneralStatusData.Delete)
                .Select(x => new UserUpdateDTO
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email,
                    Posisi = x.Posisi,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt
                }).ToList();
        }

        public bool UpdateUser(UserUpdateDTO dto)
        {
            var data = _context.Users
                .FirstOrDefault(x => x.Id == dto.Id);

            if (data == null)
            {
                return false;
            }

           
            data.Status = dto.Status;

            // Update password hanya jika diisi
            if (!string.IsNullOrEmpty(dto.Password))
            {
                data.PasswordHash = _enkripsiPassword.HashPassword(dto.Password);
            }

            _context.Users.Update(data);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteUser(int id)
        {
            var data = _context.Users
                .FirstOrDefault(x => x.Id == id);

            if (data == null)
            {
                return false;
            }

            data.Status = GeneralStatusData.Delete;
            _context.SaveChanges();
            return true;
        }



    }
}
