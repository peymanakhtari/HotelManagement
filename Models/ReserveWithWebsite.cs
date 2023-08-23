using hotelsimorgh.Data.DbModels;

namespace hotelsimorgh.Models
{
    public class ReserveWithWebsite
    {
        public Reserve reserve { get; set; }
        public string website { get; set; }
    }
}
