namespace BbibbJobStreetJwtToken.Interfaces
{
    public interface IEnkripsiPassword
    {
        public string HashPassword(string password);
        public bool VerifyPassword(string password, string hashedPassword);
    }
}
