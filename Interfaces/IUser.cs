using BbibbJobStreetJwtToken.Models.DTO;

namespace BbibbJobStreetJwtToken.Interfaces
{
    public interface IUser
    {
        Task<string?> LoginAsync(LoginDTO loginDTO);
        Task<bool> RegisterAsync(RegisterDTO registerDTO);
        Task<bool> UserExistsAsync(string username, string email);
        Task<UserDTO?> GetUserByIdAsync(int userId);
    }
}
