using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DesktopSisters.Utils;

namespace DesktopSisters
{
    public class TimeManager
    {
        public double DayRatio;
        public double NightRatio;

        public DateTime SunRise;
        public DateTime SunSet;

        public double SunX;
        public double SunY;
        public double MoonX;
        public double MoonY;


        public void Update()
        {
            //47.9972° N, 11.3406° E
            var lat = Util.ConvertDegree("33°35'N");
            var longi = Util.ConvertDegree("86°50'W");
            //var lat = Util.ConvertDegree("47°9972'N");
            //var longi = Util.ConvertDegree("11°3406'E");

            Rectangle resolution = Screen.PrimaryScreen.Bounds;

            SetSunCycleRatio(lat, longi);
            CalculateSunPosition();
        }

        public bool IsTwilight => Math.Abs(((DateTime.Now - SunSet).TotalMinutes)) < 60;

        public bool IsNightTime => (DateTime.Now - SunSet).TotalSeconds > 0;

        public bool IsDayTime => !IsNightTime;

        public void CalculateSunPosition()
        {
            var resolution = Screen.PrimaryScreen.Bounds;
            var resW = resolution.Width;
            var resH = resolution.Height;
            const int minHeight = 300;

            SunX = resW * DayRatio;
            if (NightRatio < 0.5)
                SunY = (resH - (resH * NightRatio)) - minHeight;
            else
                SunY = (resH - (resH * (1.0 - NightRatio))) - minHeight;

            MoonX = resW*NightRatio;
            if(NightRatio < 0.5)
                MoonY = (resH - (resH* NightRatio)) - minHeight;
            else
                MoonY = (resH - (resH * (1.0- NightRatio))) - minHeight;
        }

        public void SetSunCycleRatio(double Latitude, double Longitude)
        {
            var julianDate = DateTime.Now.ToOADate() + 2415018.5;
            var julianDay = (int)julianDate;


            var date = DateTime.Now;
            var isSunrise = false;
            var isSunset = false;


            var thisTime = DateTime.Now;
            var isDaylight = TimeZoneInfo.Local.IsDaylightSavingTime(thisTime);

            var zone = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).Hours;


            var jd = Util.calcJD(date);
            double sunRise = Util.calcSunRiseUTC(jd, Latitude, Longitude);
            double sunSet = Util.calcSunSetUTC(jd, Latitude, Longitude);

            var sunriseString = Util.getTimeString(sunRise, zone, jd, false);
            var sunsetString = Util.getTimeString(sunSet, zone, jd, false);

            var sunRiseHour = sunriseString.Split(':').First();
            var sunRiseMinute = sunriseString.Split(':').Last();

            SunRise = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(sunRiseHour), int.Parse(sunRiseMinute), 0);
            
            var sunSetMinute = sunsetString.Split(':').Last();
            var sunSetHour = sunsetString.Split(':').First();

            SunSet = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(sunSetHour), int.Parse(sunSetMinute), 0);

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

            var test = IsDayTime;
        }
    }
}
