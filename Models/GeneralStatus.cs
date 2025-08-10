namespace BbibbJobStreetJwtToken.Models
{
    public class GeneralStatus
    {
        public enum GeneralStatusData
        {
            Active,// semua bisa lihat
            Unactive,//admin aja yang lihat
            Delete//cuman ad didata base
        }
    }
}
