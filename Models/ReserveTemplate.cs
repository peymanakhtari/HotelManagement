using hotelsimorgh.Data.DbModels;
using MD.PersianDateTime;

namespace hotelsimorgh.Models
{
    public class ReserveTemplateRow
    {
        public DateTime date { get; set; }
        public ReserveWithWebsite reserve { get; set; }
        public ReserveWithWebsite HafDay { get; set; }
        public int statusRes { get; set; }
        public int statusHres { get; set; }
        public Room room { get; set; }

    }
}
