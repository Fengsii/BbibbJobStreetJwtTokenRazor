using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DTO;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BbibbJobStreetJwtToken.Validators
{
    public class UpdateUserRequestValidator : AbstractValidator<UserProfileUpdateDTO>
    {
        private readonly ApplicationContext _context;
        public UpdateUserRequestValidator(ApplicationContext context)
        {

            _context = context;

            //Validasi Username
            RuleFor(x => x.Username)
                .MinimumLength(3).WithMessage("Username minimal 3 karakter.")
                .MaximumLength(50).WithMessage("Username maksimal 50 karakter.")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username hanya boleh berisi huruf, angka, dan garis bawah (_).")
                .MustAsync(async (user, username, cancellation) =>
                {
                    return !await _context.Users.AnyAsync(u => u.Username == username && u.Id != user.Id, cancellation);
                }).WithMessage("Username sudah digunakan.");

            //Validasi Email
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Format email tidak valid.")
                .MaximumLength(100).WithMessage("Email maksimal 100 karakter.")
                .MustAsync(async (user, email, cancellation) =>
                {
                    return !await _context.Users.AnyAsync(u => u.Email == email && u.Id != user.Id, cancellation);
                }).WithMessage("Email sudah terdaftar.");

            RuleFor(x => x.NewPassword)
                .MinimumLength(6).WithMessage("Password minimal 6 karakter.")
                .MaximumLength(100).WithMessage("Password maksimal 100 karakter.")
                .When(x => !string.IsNullOrEmpty(x.NewPassword));

            RuleFor(x => x.Posisi)
                .MinimumLength(6).WithMessage("Posisi minimal 6 karakter.")
                .MaximumLength(100).WithMessage("Posisi maksimal 100 karakter.")
                .When(x => !string.IsNullOrEmpty(x.Posisi));
        }
    }
}
