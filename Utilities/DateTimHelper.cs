using MD.PersianDateTime;

namespace hotelsimorgh.Utilities
{
    public static class DateTimHelper
    {
        public static string PersianDayOfWeek(this PersianDateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "یک‌شنبه";
                case DayOfWeek.Monday:
                    return "دوشنبه";
                case DayOfWeek.Tuesday:
                   return "سه‌شنبه";
                case DayOfWeek.Wednesday:
                  return  "چهارشنبه";
                case DayOfWeek.Thursday:
                  return  "پنچ‌شنبه";
                case DayOfWeek.Friday:
                  return  "جمعه";
                case DayOfWeek.Saturday:
                  return  "شنبه";
                default:
                    return "";
            }
        }

    }
   
}
