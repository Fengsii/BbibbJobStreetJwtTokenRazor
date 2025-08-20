using System.ComponentModel.DataAnnotations;

namespace BbibbJobStreetJwtToken.Models.DTO
{
    public class RegisterUserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Posisi { get; set; }
        public string? ProfileImage { get; set; }
        public string? CoverImage { get; set; }

    }
}
