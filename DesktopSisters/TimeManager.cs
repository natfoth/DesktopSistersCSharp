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

        public DateTime _dateTime;

        public TimeManager(DateTime dateTime, double lat, double longi)
        {
            //_dateTime = DateTime.Parse("6:00 pm");
            _dateTime = dateTime;
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
            return SunPosition.CalculateSunPosition(_dateTime, _latitude, _longitude);
        }

//Math.Abs(((_dateTime - SunSet).TotalMinutes)) < 60

        public bool IsNightTime => (_dateTime - SunRise).TotalSeconds < 0 || (_dateTime - SunSet).TotalSeconds > 0;

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
            var julianDate = _dateTime.ToOADate() + 2415018.5;


            var date = _dateTime;

            var zone = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).Hours;


            var jd = Util.calcJD(date);
            double sunRise = Util.calcSunRiseUTC(jd, Latitude, Longitude);
            double sunSet = Util.calcSunSetUTC(jd, Latitude, Longitude);

            var sunriseString = Util.getTimeString(sunRise, zone, jd, false);
            var sunsetString = Util.getTimeString(sunSet, zone, jd, false);

            var sunRiseHour = sunriseString.Split(':').First();
            var sunRiseMinute = sunriseString.Split(':').Last();

            SunRise = new DateTime(_dateTime.Year, _dateTime.Month, _dateTime.Day, int.Parse(sunRiseHour), int.Parse(sunRiseMinute), 0);
            
            var sunSetMinute = sunsetString.Split(':').Last();
            var sunSetHour = sunsetString.Split(':').First();

            SunSet = new DateTime(_dateTime.Year, _dateTime.Month, _dateTime.Day, int.Parse(sunSetHour), int.Parse(sunSetMinute), 0);

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
