using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DTO;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BbibbJobStreetJwtToken.Validators
{
    public class RegisterCompanyRequestValidator : AbstractValidator<RegisterPerusahaanDTO>
    {
        private readonly ApplicationContext _context;
        public RegisterCompanyRequestValidator(ApplicationContext context)
        {
            _context = context;

            // Validasi Nama Perusahaan
            RuleFor(x => x.NamaPerusahaan)
                .NotEmpty().WithMessage("Nama perusahaan wajib diisi.")
                .MinimumLength(3).WithMessage("Nama perusahaan minimal 3 karakter.")
                .MaximumLength(100).WithMessage("Nama perusahaan maksimal 100 karakter.")
                .MustAsync(async (perusahaan, nama, cancellation) =>
                {
                    return !await _context.Perusahaans.AnyAsync(p => p.NamaPerusahaan == nama && p.Id != perusahaan.PerusahaanId, cancellation);
                }).WithMessage("Nama perusahaan sudah terdaftar.");

            // Validasi Email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email wajib diisi.")
                .EmailAddress().WithMessage("Format email tidak valid.")
                .MaximumLength(100).WithMessage("Email maksimal 100 karakter.")
                .MustAsync(async (perusahaan, email, cancellation) =>
                {
                    return !await _context.Perusahaans.AnyAsync(p => p.Email == email && p.Id != perusahaan.PerusahaanId, cancellation);
                }).WithMessage("Email sudah terdaftar.");

            // Validasi Password (hanya untuk perusahaan baru)
            RuleFor(x => x.Password)
                .MinimumLength(6).When(x => x.PerusahaanId == 0).WithMessage("Password minimal 6 karakter.")
                .MaximumLength(255).WithMessage("Password terlalu panjang (maksimal 255 karakter).");

            // Validasi Telepon
            RuleFor(x => x.Telepon)
                .NotEmpty().WithMessage("Nomor telepon wajib diisi.")
                .Matches(@"^[0-9+]{10,15}$").WithMessage("Nomor telepon tidak valid (hanya angka, panjang 10-15 digit).");

            // Validasi Alamat
            RuleFor(x => x.Alamat)
                .NotEmpty().WithMessage("Alamat wajib diisi.")
                .MinimumLength(10).WithMessage("Alamat minimal 10 karakter.")
                .MaximumLength(255).WithMessage("Alamat maksimal 255 karakter.");

            // Validasi Kota
            RuleFor(x => x.Kota)
                .NotEmpty().WithMessage("Kota wajib diisi.")
                .MaximumLength(100).WithMessage("Kota maksimal 100 karakter.")
                .MinimumLength(4).WithMessage("Bidang Usaha minimal 4 karakter."); ;

            // Validasi Provinsi
            RuleFor(x => x.Provinsi)
                .NotEmpty().WithMessage("Provinsi wajib diisi.")
                .MaximumLength(100).WithMessage("Provinsi maksimal 100 karakter.");

            // Validasi Bidang Usaha
            RuleFor(x => x.BidangUsaha)
                .NotEmpty().WithMessage("Bidang usaha wajib diisi.")
                .MaximumLength(100).WithMessage("Bidang usaha maksimal 100 karakter.")
                .MinimumLength(6).WithMessage("Bidang Usaha minimal 6 karakter.");

            // Validasi Tanggal Berdiri
            RuleFor(x => x.TanggalBerdiri)
                .NotEmpty().WithMessage("Tanggal berdiri wajib diisi.")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Tanggal berdiri tidak boleh di masa depan.");

            // ✅ Validasi LogoPath (opsional)
            RuleFor(x => x.LogoPath)
                .Must(file =>
                {
                    if (file == null) return true; // opsional
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    return allowedExtensions.Contains(extension);
                }).WithMessage("Logo hanya boleh dalam format JPG atau PNG.");
        }



    }
}
