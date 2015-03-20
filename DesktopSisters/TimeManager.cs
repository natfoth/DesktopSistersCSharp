using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSisters.Utils;

namespace DesktopSisters
{
    public class TimeManager
    {
        public double DayRatio;
        public double NightRatio;

        public void Update()
        {
            var lat = Util.ConvertDegree("33°35'N");
            var longi = Util.ConvertDegree("86°50'W");

            int bob = 1;
            SetSunCycleRatio(lat, longi);
        }

        public void SetSunCycleRatio(double Latitude, double Longitude)
        {
            var julianDate = DateTime.Now.ToOADate() + 2415018.5;
            var julianDay = (int)julianDate;


            var date = DateTime.Now;
            var isSunrise = false;
            var isSunset = false;
            var sunrise = DateTime.Now;
            var sunset = DateTime.Now;


            var thisTime = DateTime.Now;
            var isDaylight = TimeZoneInfo.Local.IsDaylightSavingTime(thisTime);

            var zone = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).Hours;


            var jd = Util.calcJD(date);
            double sunRise = Util.calcSunRiseUTC(jd, Latitude, Longitude);
            double sunSet = Util.calcSunSetUTC(jd, Latitude, Longitude);

            var sunriseString = Util.getTimeString(sunRise, zone, jd, false);
            var sunsetString = Util.getTimeString(sunSet, zone, jd, false);

            double currentTimeDec = Double.Parse(String.Format("{0}.{1}", date.Hour, date.Minute));
            double sunRiseTimeDec = Double.Parse(sunriseString.Replace(":", "."));
            double sunSetTimeDec = Double.Parse(sunsetString.Replace(":", "."));

            double startingSunRise = sunRiseTimeDec - 12; // 12 hours is the length of a cycle


            DayRatio = (currentTimeDec - sunRiseTimeDec) / (sunSetTimeDec - sunRiseTimeDec);
            NightRatio = (currentTimeDec - startingSunRise) / (sunRiseTimeDec - startingSunRise);

            if (currentTimeDec > sunSetTimeDec && currentTimeDec > 15)
            {
                var amountToSub = currentTimeDec - sunSetTimeDec;
                currentTimeDec = 0 - amountToSub;
                NightRatio = (currentTimeDec - startingSunRise) / (sunRiseTimeDec - startingSunRise);
            }
        }
    }
}
