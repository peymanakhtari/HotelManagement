using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotelsimorgh.Data.DbModels
{
    public class Folio
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string FatherName { get; set; }
        public string Mobile { get; set; }
        public string NationalCode { get; set; }
        public string Address { get; set; }
        public string Pelak1 { get; set; }
        public string Pelak2 { get; set; }
        public string Pelak3 { get; set; }
        public string Pelak4 { get; set; }
        public string CarModel { get; set; }
        public bool PreReserve { get; set; }
        public bool Bedehkar { get; set; }
        public string Description { get; set; }
        public int? receptionistId { get; set; }
        public string Email { get; set; }
        public ICollection<passenger> passengers { get; set; } = new List<passenger>();
        public ICollection<Reserve> Reserves { get; set; } = new List<Reserve>();
    }
}
