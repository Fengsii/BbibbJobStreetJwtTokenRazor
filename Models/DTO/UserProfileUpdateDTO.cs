namespace BbibbJobStreetJwtToken.Models.DTO
{
    public class UserProfileUpdateDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Posisi { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public IFormFile? CoverImage { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
