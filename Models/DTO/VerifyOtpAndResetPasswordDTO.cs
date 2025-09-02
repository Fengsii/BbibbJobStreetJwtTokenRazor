namespace BbibbJobStreetJwtToken.Models.DTO
{
    public class VerifyOtpAndResetPasswordDTO
    {
        public string EmailOrUsername { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
