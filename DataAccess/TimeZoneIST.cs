using System.Runtime.InteropServices;

namespace SpliterX_API.DataAccess
{
    public class TimeZoneIST
    {
        public static DateTime now()
        {
            string timeZoneId;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                timeZoneId = "India Standard Time"; 
            }
            else
            {
                timeZoneId = "Asia/Kolkata"; 
            }

            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
        }

    }
}
