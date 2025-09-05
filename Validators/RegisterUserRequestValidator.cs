using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DTO;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BbibbJobStreetJwtToken.Validators
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserDTO>
    {
        private readonly ApplicationContext _context;

        public RegisterUserRequestValidator(ApplicationContext context)
        {
            _context = context;

            //Validasi Username
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username wajib diisi.")
                .MinimumLength(3).WithMessage("Username minimal 3 karakter.")
                .MaximumLength(50).WithMessage("Username maksimal 50 karakter.")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username hanya boleh berisi huruf, angka, dan garis bawah (_).")
                .MustAsync(async (user, username, cancellation) =>
                {
                    return !await _context.Users.AnyAsync(u => u.Username == username && u.Id != user.Id, cancellation);
                }).WithMessage("Username sudah digunakan.");

            //Validasi Email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email wajib diisi.")
                .EmailAddress().WithMessage("Format email tidak valid.")
                .MaximumLength(100).WithMessage("Email maksimal 100 karakter.")
                .MustAsync(async (user, email, cancellation) =>
                {
                    return !await _context.Users.AnyAsync(u => u.Email == email && u.Id != user.Id, cancellation);
                }).WithMessage("Email sudah terdaftar.");

            //Validasi Password (hanya untuk user baru)
            RuleFor(x => x.Password)
                .NotEmpty().When(x => x.Id == 0).WithMessage("Password wajib diisi.")
                .MinimumLength(6).When(x => x.Id == 0).WithMessage("Password minimal 6 karakter.")
                .MaximumLength(255).WithMessage("Password terlalu panjang (maksimal 255 karakter).");

            //Validasi Posisi
            RuleFor(x => x.Posisi)
                .MinimumLength(6).WithMessage("Posisi minimal 6 karakter.")
                .MaximumLength(100).WithMessage("Posisi maksimal 100 karakter.");
        }



    }

}

