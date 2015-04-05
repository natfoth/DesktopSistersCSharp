using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using DesktopSisters.Utils;
using DesktopSistersCSharpForm;

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

        private double _latitude;
        private double _longitude;

        public DateTime DateTime;

        public TimeManager(DateTime dateTime, double lat, double longi)
        {
            //_dateTime = DateTime.Parse("6:00 pm");
            DateTime = dateTime;
            _latitude = lat;
            _longitude = longi;
        }

        public void Update()
        {
            //var timeSpan = DateTime.Now.Subtract(_lastRealTime);
            // _dateTime = _dateTime.AddMinutes(5);

            SetSunCycleRatio(_latitude, _longitude);
            CalculateSunPosition();

        }

        public bool IsTwilight
        {
            get
            {
                var sunPos = GetSunPos();
                return (sunPos.Item1 > -6 && sunPos.Item1 < 0);
            }
        }

        public bool IsPrePostTwilight {
            get {
                var sunPos = GetSunPos();
                return (sunPos.Item1 < 6 && sunPos.Item1 > 0);
            }
        }

        public Tuple<double, double> GetSunPos()
        {
            return SunPosition.CalculateSunPosition(DateTime, _latitude, _longitude);
        }

//Math.Abs(((_dateTime - SunSet).TotalMinutes)) < 60

        public bool IsNightTime => (DateTime - SunRise).TotalSeconds < 0 || (DateTime - SunSet).TotalSeconds > 0;

        public bool IsDayTime => !IsNightTime;

        public void CalculateSunPosition()
        {
            var resolution = Screen.PrimaryScreen.Bounds;
            var resW = resolution.Width;
            var resH = resolution.Height;
            const int minHeight = 300;

            SunX = resW * DayRatio;
            if (DayRatio < 0.5)
                SunY = (resH - (resH * DayRatio)) - minHeight;
            else
                SunY = (resH - (resH * (1.0 - DayRatio))) - minHeight;

            MoonX = resW*NightRatio;
            if(NightRatio < 0.5)
                MoonY = (resH - (resH* NightRatio)) - minHeight;
            else
                MoonY = (resH - (resH * (1.0- NightRatio))) - minHeight;
        }

        public void SetSunCycleRatio(double Latitude, double Longitude)
        {
            var date = DateTime;

            var zone = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).Hours;


            var jd = Util.calcJD(date);
            double sunRise = Util.calcSunRiseUTC(jd, Latitude, Longitude);
            double sunSet = Util.calcSunSetUTC(jd, Latitude, Longitude);

            var sunriseString = Util.getTimeString(sunRise, zone, jd, false);
            var sunsetString = Util.getTimeString(sunSet, zone, jd, false);

            if (sunriseString == "error" || sunsetString == "error")
            {
                DayRatio = 0.0;
                NightRatio = 0.0;
                return;
            }

            var sunRiseHour = sunriseString.Split(':').First();
            var sunRiseMinute = sunriseString.Split(':').Last();

            SunRise = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, int.Parse(sunRiseHour), int.Parse(sunRiseMinute), 0);
            
            var sunSetMinute = sunsetString.Split(':').Last();
            var sunSetHour = sunsetString.Split(':').First();

            SunSet = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, int.Parse(sunSetHour), int.Parse(sunSetMinute), 0);

            var currentTimeDec = date.ToDouble();
            var sunRiseTimeDec = SunRise.ToDouble();
            var sunSetTimeDec = SunSet.ToDouble();

            double startingSunRise = sunRiseTimeDec - 12; // 12 hours is the length of a cycle


            DayRatio = (currentTimeDec - sunRiseTimeDec) / (sunSetTimeDec - sunRiseTimeDec);
            NightRatio = (currentTimeDec - startingSunRise) / (sunRiseTimeDec - startingSunRise);

            if (currentTimeDec > sunSetTimeDec && currentTimeDec > 15)
            {
                var amountToSub = currentTimeDec - sunSetTimeDec;

                NightRatio = amountToSub / 12;
            }
        }
    }

}
