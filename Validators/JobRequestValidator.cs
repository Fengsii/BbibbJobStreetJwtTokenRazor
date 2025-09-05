using BbibbJobStreetJwtToken.Models;
using BbibbJobStreetJwtToken.Models.DB;
using BbibbJobStreetJwtToken.Models.DTO;
using FluentValidation;

namespace BbibbJobStreetJwtToken.Validators
{
    public class JobRequestValidator : AbstractValidator<LowonganPekerjaanAddUpdateDTO>
    {
        private readonly ApplicationContext _context;
        public JobRequestValidator(ApplicationContext context)
        {
            _context = context;

            RuleFor(x => x.Judul)
                .NotEmpty().WithMessage("Judul lowongan wajib diisi.")
                .MinimumLength(6).WithMessage("Judul lowongan 6 karakter.")
                .MaximumLength(150).WithMessage("Judul lowongan maksimal 150 karakter.");

            RuleFor(x => x.Alamat)
                .NotEmpty().WithMessage("Alamat wajib diisi.")
                .MinimumLength(4).WithMessage("Alamat minimal 4 karakter.")
                .MaximumLength(255).WithMessage("Alamat maksimal 255 karakter.");

            RuleFor(x => x.Deskripsi)
                .NotEmpty().WithMessage("Deskripsi pekerjaan wajib diisi.")
                .MinimumLength(12).WithMessage("Deskripsi minimal 12 karakter.");

            RuleFor(x => x.Posisi)
                .NotEmpty().WithMessage("Posisi pekerjaan wajib diisi.")
                .MaximumLength(100).WithMessage("Posisi maksimal 100 karakter.");

            RuleFor(x => x.TanggalDibuat)
                .NotEmpty().WithMessage("Tanggal dibuat wajib diisi.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Tanggal dibuat tidak boleh di masa depan.");
        }
    }
}
