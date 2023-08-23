namespace hotelsimorgh.Models
{
    public class ReserveRow
    {
        public DateTime Date { get; set; }
        public List<RoomModel> rooms { get; set; }
    }
}
