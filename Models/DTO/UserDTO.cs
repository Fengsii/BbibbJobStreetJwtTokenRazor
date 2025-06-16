namespace BbibbJobStreetJwtToken.Models.DTO
{
    public class UserDTO
    {
        //=========== INI UNTUK RQUESTNYA(MENAMPILKAN/TAMP) =========\\
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
