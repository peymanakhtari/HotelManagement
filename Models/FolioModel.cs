using hotelsimorgh.Data.DbModels;

namespace hotelsimorgh.Models
{
    public class FolioModel
    {
        public Folio Folio { get; set; }
        public IEnumerable<passenger> passengers { get; set; }
    }
}
