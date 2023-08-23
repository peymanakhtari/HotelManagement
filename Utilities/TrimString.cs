using hotelsimorgh.Data.DbModels;

namespace hotelsimorgh.Utilities
{
    public static class TrimString
    {
        public static Folio TrimFolioValues(this Folio folio)
        {
            folio.Name = (folio.Name == null) ? null : folio.Name.Trim();
            folio.FatherName = (folio.FatherName == null) ? null : folio.FatherName.Trim();
            folio.Family= (folio.Family == null) ? null : folio.Family.Trim();
            folio.Address =(folio.Address==null)? null: folio.Address.Trim();
            folio.Pelak3=(folio.Pelak3==null) ? null: folio.Pelak3.Trim();
            folio.CarModel = (folio.CarModel == null) ? null : folio.CarModel.Trim();
            return folio;

        }
        public static List<passenger> TrimPassengersValues(this List<passenger> passengers)
        {
            foreach (var item in passengers)
            {
                item.Name = item.Name.Trim();
                item.Family = item.Family.Trim();
                item.FatherName = item.FatherName.Trim();
            }
            return passengers;
        }
    }
}
