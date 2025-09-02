using BbibbJobStreetJwtToken.Models.DTO;
using X.PagedList;

namespace BbibbJobStreetJwtToken.Interfaces
{
    public interface IUser
    {
        Task<string?> LoginAsync(LoginUserDTO loginDTO);
        Task<bool> RegisterAsync(RegisterUserDTO registerDTO);
        Task<bool> UserExistsAsync(string username, string email);
        Task<UserUpdateDTO?> GetUserByIdAsync(int userId);

        public IPagedList<UserUpdateDTO> GetListUsers(int page, int pageSize, string searchTerm = "");
        public List<UserUpdateDTO> GetListUsers();
        public bool UpdateUser(UserUpdateDTO dto);
        public bool DeleteUser(int id);
        public UserProfileViewDTO GetCurrentUser();
        public bool UpdateUserProfile(UserProfileUpdateDTO dto);
        Task<bool> SendOtpAsync(string emailOrUsername);
        Task<bool> ResetPasswordWithOtpAsync(VerifyOtpAndResetPasswordDTO dto);
    }
}
