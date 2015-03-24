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

        private Regex _googleLatLongRegex = new Regex(@"(?<lat>\d\d[.][\d]+)[^0-9] (?<latC>[NS]), (?<long>\d\d[.][\d]+)[^0-9] (?<longC>[EW])", RegexOptions.Compiled);


        public TimeManager(Configuration config)
        {
            _dateTime = DateTime.Parse("6:00 pm");
            _lastRealTime = DateTime.Now;
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

        private DateTime _dateTime;
        private DateTime _lastRealTime;

        public void Update()
        {
            var timeSpan = DateTime.Now.Subtract(_lastRealTime);
            _dateTime = _dateTime.AddMinutes(5);
            _lastRealTime = DateTime.Now;

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
        }

        public void UpdateConfig(Configuration config)
        {
            UpdateLatLong(config.Coordinates);
        }
    }
}
