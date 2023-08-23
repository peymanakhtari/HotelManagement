namespace hotelsimorgh.Data.DbModels
{
    public class DeletedReserve
    {
        public int Id { get; set; }
        public DateTime datetime { get; set; }
        public int UserId { get; set; }
        public DateTime ReserveDate { get; set; }
        public int folioId { get; set; }
        public string folioName { get; set; }
        public string Mobile { get; set; }
        public int? websiteId { get; set; }
        public int RoomNumber { get; set; }
        public int Price { get; set; }
        public bool HafDay { get; set; }
        public User User { get; set; }
    }
}
