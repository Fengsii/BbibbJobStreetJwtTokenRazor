using BbibbJobStreetJwtToken.Models.DTO;
using FluentValidation;

namespace BbibbJobStreetJwtToken.Validators
{
    public class ApplicationRequestValidator : AbstractValidator<LamaranAddUpdateDTO>
    {
        public ApplicationRequestValidator()
        {

            RuleFor(x => x.Nama)
                .NotEmpty().WithMessage("Nama wajib diisi.")
                .MinimumLength(4).WithMessage("Nama minimal 4 karakter.")
                .MaximumLength(100).WithMessage("Nama maksimal 100 karakter.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email wajib diisi.")
                .EmailAddress().WithMessage("Format email tidak valid.");

            RuleFor(x => x.NoHP)
                .NotEmpty().WithMessage("Nomor HP wajib diisi.")
                .Matches(@"^[0-9]+$").WithMessage("Nomor HP hanya boleh angka.")
                .MinimumLength(10).WithMessage("Nomor HP minimal 10 digit.")
                .MaximumLength(15).WithMessage("Nomor HP maksimal 15 digit.");

            RuleFor(x => x.GajiSaatIni)
                .GreaterThanOrEqualTo(0).WithMessage("Gaji saat ini tidak boleh negatif.");

            RuleFor(x => x.GajiDiharapkan)
                .GreaterThan(0).WithMessage("Gaji yang diharapkan wajib lebih dari 0.");

            
        }
    }
}
