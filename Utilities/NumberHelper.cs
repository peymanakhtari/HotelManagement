namespace hotelsimorgh.Utilities
{
    public static class NumberHelper
    {
        public static string toPersianNumber(this int num)
        {
            switch (num)
            {
                case 1:
                    return "یک";
                case 2:
                    return "دو";
                case 3:
                    return "سه‌";
                case 4:
                    return "چهار";
                case 5:
                    return "پنج";
                default:
                    return "num";
            }
        }
                    
    }
}
