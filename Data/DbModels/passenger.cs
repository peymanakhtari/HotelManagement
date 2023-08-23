namespace hotelsimorgh.Data.DbModels
{
    public class passenger
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Family { get; set; }
        public string FatherName { get; set; }
        public string NationalCode { get; set; }
        public int FolioId { get; set; }
        public Folio Folio { get; set; }

    }
}
