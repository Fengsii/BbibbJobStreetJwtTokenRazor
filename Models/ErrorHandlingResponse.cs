namespace BbibbJobStreetJwtToken.Models
{
    public class ErrorHandlingResponse
    {
        public string StatusCode { get; set; }
        public string Statusdesc { get; set; }
        public object Data { get; set; }
    }
}
