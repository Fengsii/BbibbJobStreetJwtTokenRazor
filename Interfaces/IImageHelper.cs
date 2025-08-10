namespace BbibbJobStreetJwtToken.Interfaces
{
    public interface IImageHelper
    {
        public string Save(IFormFile file, string folder = "uploads");
        public bool Delete(string relativePath);
    }
}
