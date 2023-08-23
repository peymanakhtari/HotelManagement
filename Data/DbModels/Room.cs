namespace hotelsimorgh.Data.DbModels
{
    public class Room
    {
        public int Id { get; set; }
        public int bed { get; set; }
        public int Capacity { get; set; }
        public int Sequence { get; set; }
        public int Number { get; set; }
        public ICollection<Reserve> Reserves { get; set; } = new List<Reserve>();
    }
}
