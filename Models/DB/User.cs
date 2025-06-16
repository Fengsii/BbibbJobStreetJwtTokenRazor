namespace BbibbJobStreetJwtToken.Models.DB
{
    public class User
    {
        //=========== INI SEBAGAI DATABASE NYA =========\\
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
