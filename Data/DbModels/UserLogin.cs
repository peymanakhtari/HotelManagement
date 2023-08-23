namespace hotelsimorgh.Data.DbModels
{
    public class UserLogin
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime datetime { get; set; }
        public User User { get; set; }
    }
}
