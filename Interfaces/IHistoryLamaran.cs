using BbibbJobStreetJwtToken.Models.DTO;

namespace BbibbJobStreetJwtToken.Interfaces
{
    public interface IHistoryLamaran
    {
        public List<HistoryLamaranDTO> GetListHistoryLowongan();
        public bool DeletelamaranTersimpan(int id);
    }
}
