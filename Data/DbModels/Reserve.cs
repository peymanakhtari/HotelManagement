namespace hotelsimorgh.Data.DbModels
{
    public class Reserve
    {
        public int Id { get; set; }
        public DateTime Date   { get; set; }
        public DateTime SubmitDate   { get; set; }
        public int Price { get; set; }
        public bool HafDay { get; set; }
        public int? WebsiteId { get; set; }
        public int FolioId { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public Folio Folio { get; set; }    
        public Room Room { get; set; }
        public User User { get; set; }
    }
}
