namespace BbibbJobStreetJwtToken.Interfaces
{
    public interface IEmailHelper
    {
        Task<bool> SendOtpEmailAsync(string toEmail, string otpCode);
        Task<bool> SendStatusLamaranEmailAsync(string toEmail, string subject, string body);
    }
}
