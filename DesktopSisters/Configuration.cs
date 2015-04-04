using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using DesktopSisters.Utils;
using GoogleMaps.LocationServices;
using Newtonsoft.Json;

namespace DesktopSisters
{
    public class Configuration
    {
        private string _path;

        // ReSharper disable once UnusedMember.Local
        private Configuration(){}

        private Configuration(string path)
        {
            _path = path;
        }

        public string Coordinates { get; set; } = "";

        public bool UseNewArtStyle { get; set; } = true;

        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;

        #region Load & Save
        public static Configuration LoadConfig(string path)
        {
            if (!File.Exists(path))
            {
                
                return CreateDefaultConfig(path);
            }
            else
            {
                var config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(path));
                config._path = path;
                config.UpdateLatLong();
                return config;
            }
        }

        private static Configuration CreateDefaultConfig(string path)
        {
            var config = new Configuration(path);

            config.Save();

            return config;
        }

        public void Save()
        {
            File.WriteAllText(_path, JsonConvert.SerializeObject(this));
        }

        private Regex _googleLatLongRegex = new Regex(@"(?<lat>\d\d[.][\d]+)[^0-9] (?<latC>[NS]), (?<long>\d\d[.][\d]+)[^0-9] (?<longC>[EW])", RegexOptions.Compiled);
        public void UpdateLatLong()
        {
            var latLong = Coordinates;


            if (latLong != null)
            {
                Tuple<string, string> parsed = GetGoogleLatLong(latLong);

                if (parsed != null)
                {
                    Latitude = Util.ConvertDegree(parsed.Item1);
                    Longitude = Util.ConvertDegree(parsed.Item2);
                }
                else
                {
                    try
                    {
                        var locationService = new GoogleLocationService();
                        var point = locationService.GetLatLongFromAddress(latLong);
                        

                        if (point != null)
                        {
                            Latitude = point.Latitude;
                            Longitude = point.Longitude;

                            var region = locationService.GetRegionFromLatLong(Latitude, Longitude);

                            var infos = TimeZoneInfo.GetSystemTimeZones();
                            foreach (var info in infos)
                            {
                                Console.WriteLine(info.Id);
                            }
                            int bob = 1;
                        }
                    }
                    catch (Exception e)
                    {
                        return;
                    }  
                }

                Save();
            }
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
        #endregion
    }
}