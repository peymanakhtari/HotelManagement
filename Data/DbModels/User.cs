namespace hotelsimorgh.Data.DbModels
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Bloked { get; set; }
        public ICollection<Reserve> Reserves { get; set; } = new List<Reserve>();
        public ICollection<DeletedReserve> DeletedReserves { get; set; } = new List<DeletedReserve>();
        public ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
    }
}
