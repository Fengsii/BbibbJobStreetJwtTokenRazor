namespace BbibbJobStreetJwtToken.Models.DTO
{
    public class UserProfileViewDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Posisi { get; set; }
        public string ProfileImage { get; set; }
        public string CoverImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
