using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        private double _latitude;
        private double _longitude;

        private Regex _googleLatLongRegex = new Regex(@"(?<lat>\d\d[.][\d]+)[^0-9] (?<latC>[NS]), (?<long>\d\d[.][\d]+)[^0-9] (?<longC>[EW])", RegexOptions.Compiled);

        public TimeManager(Configuration config)
        {
            UpdateConfig(config);
        }

        public void UpdateLatLong(string latLong)
        {
            if (latLong != null)
            {
                Tuple<string, string> parsed = GetGoogleLatLong(latLong);

                if (parsed != null)
                {
                    _latitude = Util.ConvertDegree(parsed.Item1);
                    _longitude = Util.ConvertDegree(parsed.Item2);
                    Console.WriteLine(IsTwilight);
                }
            }
        }

        public void Update()
        {

            Rectangle resolution = Screen.PrimaryScreen.Bounds;

            SetSunCycleRatio(_latitude, _longitude);
            CalculateSunPosition();
        }



        private Tuple<string, string> GetGoogleLatLong(string latLong)
        {
            var match = _googleLatLongRegex.Match(latLong);

            if (!match.Success)
                return null;
            var latitude = match.Groups["lat"].Value.Replace(".", "°") + "'" + match.Groups["latC"].Value;
            var longitude = match.Groups["long"].Value.Replace(".", "°") + "'" + match.Groups["longC"].Value;

            return new Tuple<string, string>(latitude, longitude);
        }


        public bool IsTwilight
        {
            get
            {
                var sunPos = GetSunPos();
                return (sunPos.Item1 > -6 && sunPos.Item1 < 0);
            }
        }

        public Tuple<double, double> GetSunPos()
        {
            return SunPosition.CalculateSunPosition(DateTime.Now, _latitude, _longitude);
        }

//Math.Abs(((DateTime.Now - SunSet).TotalMinutes)) < 60

        public bool IsNightTime => (DateTime.Now - SunRise).TotalSeconds < 0 || (DateTime.Now - SunSet).TotalSeconds > 0;

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

            double currentTimeDec = Double.Parse(String.Format("{0}.{1:00}", date.Hour, date.Minute));
            double sunRiseTimeDec = Double.Parse(sunriseString.Replace(":", "."));
            double sunSetTimeDec = Double.Parse(sunsetString.Replace(":", "."));

            double startingSunRise = sunRiseTimeDec - 12; // 12 hours is the length of a cycle


            DayRatio = (currentTimeDec - sunRiseTimeDec) / (sunSetTimeDec - sunRiseTimeDec);
            NightRatio = (currentTimeDec - startingSunRise) / (sunRiseTimeDec - startingSunRise);

            if (currentTimeDec > sunSetTimeDec && currentTimeDec > 15)
            {
                var amountToSub = currentTimeDec - sunSetTimeDec;

                NightRatio = amountToSub / 12;
            }

            var test = IsDayTime;
        }

        public void UpdateConfig(Configuration config)
        {
            UpdateLatLong(config.Coordinates);
        }
    }
}
