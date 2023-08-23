using MD.PersianDateTime;
using System.Globalization;

namespace hotelsimorgh.Utilities
{
    public static class DateConvertor
    {
       public static PersianDateTime toPersianDateTime(this string reserveDate)
        {
            int year = Convert.ToInt32(reserveDate.Split('/')[0]);
            int month = Convert.ToInt32(reserveDate.Split('/')[1]);
            int day = Convert.ToInt32(reserveDate.Split('/')[2]);
            return new PersianDateTime(year, month, day);
        }
        public static string ToStringPersianDateTime2(this PersianDateTime persianDateTime)
        {
            var month = persianDateTime.Month.ToString();
            var day = persianDateTime.Day.ToString();
            return $"{month}/{day}";
        }
       
        public static string ToStringPersianDateTime(this PersianDateTime persianDateTime)
        {
            var year=persianDateTime.Year.ToString();
            var month=persianDateTime.Month.ToString();    
            var day=persianDateTime.Day.ToString();
            return $"{year}/{month}/{day}";
        }
        public static PersianDateTime ToShamsi(this DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            var year=pc.GetYear(date);
            var month=pc.GetMonth(date);
            var day=pc.GetDayOfMonth(date);

            return new PersianDateTime(year, month,day);
        }
        public static DateTime ToMiladi(this PersianDateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            return new DateTime(date.Year, date.Month, date.Day, pc);
        }
        public static DateTime getFromDate(this string dateRange)
        {
            DateTime dateTime;
            if (dateRange==null)
            {
                dateTime ="1402/05/17".toPersianDateTime().ToMiladi();
            }
            else
            {
                dateTime = dateRange.Split("-")[0].Trim().toPersianDateTime().ToMiladi();
            }
            return dateTime;
        }
        public static DateTime getFromDate(this string dateRange,int Days)
        {
            DateTime dateTime;
            if (dateRange == null)
            {
                dateTime ="1402/05/17".toPersianDateTime().ToMiladi().AddDays(Days);
            }
            else
            {
                dateTime = dateRange.Split("-")[0].Trim().toPersianDateTime().ToMiladi();
            }
            return dateTime;
        }
        public static DateTime getUntilDate(this string dateRange,int Days)
        {
            DateTime dateTime;
            if (dateRange == null)
            {
                dateTime = "1402/05/17".toPersianDateTime().ToMiladi().AddDays(Days);
            }
            else
            {
                dateTime = dateRange.Split("-")[1].Trim().toPersianDateTime().ToMiladi();
            }
            return dateTime;
        }
    }
}
