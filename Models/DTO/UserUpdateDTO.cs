namespace BbibbJobStreetJwtToken.Models.DTO
{
    public class UserUpdateDTO
    {
        //=========== INI UNTUK RQUESTNYA(MENAMPILKAN/TAMP) =========\\
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProfileImage { get; set; }
        public string CoverImage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
