using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DesktopSisters;
using DesktopSisters.Utils;

namespace DesktopSistersCSharpForm
{
    public class RenderScene
    {
        public String Filename { get; set; }
        private TimeManager TimeManager;
        private WallpaperManager WallpaperManager;
        

        public RenderScene(string fileName, DateTime timeToRender, double lat, double longi, Configuration config)
        {
            Filename = fileName;

            TimeManager = new TimeManager(timeToRender, lat, longi);
            TimeManager.Update();

            WallpaperManager = new WallpaperManager(TimeManager, config);
            WallpaperManager.Init();
        }

        public Bitmap RenderedScene => WallpaperManager.RenderToBitmap();
    }

    public class RenderController
    {
        

        private Configuration _config;

        private double _latitude;
        private double _longitude;
        private bool _rendering;

        public List<RenderScene> Scenes = new List<RenderScene>(); 

        public RenderController(Configuration config)
        {
            _config = config;

            UpdateConfig(_config); 
        }

        public void AddSceneToQueue(DateTime dateTime, string filename)
        {
            var newScene = new RenderScene(filename, dateTime, _latitude, _longitude, _config);
            Scenes.Add(newScene);
        }

        public void Pulse()
        {
            if (Scenes.Count == 0 || _rendering)
                return;

            _rendering = true;
            var sceneToRender = Scenes[0]; // grab the first scene
            Scenes.RemoveAt(0); // pop it off the stack

            var renderedScene = sceneToRender.RenderedScene;

            string tempPath = Path.GetTempPath();
            string filePath = Path.Combine(tempPath, String.Format("{0}.bmp", sceneToRender.Filename));

            renderedScene.Save(filePath);

            if (sceneToRender.Filename == "Wallpaper")
            {
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filePath, SPIF_UPDATEINIFILE);
            }
            _rendering = false;

        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, string pvParam, UInt32 fWinIni);
        private static UInt32 SPI_SETDESKWALLPAPER = 20;
        private static UInt32 SPIF_UPDATEINIFILE = 0x1;

        #region Config
        public void UpdateConfig(Configuration config)
        {
            UpdateLatLong(config.Coordinates);
        }

        private Regex _googleLatLongRegex = new Regex(@"(?<lat>\d\d[.][\d]+)[^0-9] (?<latC>[NS]), (?<long>\d\d[.][\d]+)[^0-9] (?<longC>[EW])", RegexOptions.Compiled);
        public void UpdateLatLong(string latLong)
        {
            if (latLong != null)
            {
                Tuple<string, string> parsed = GetGoogleLatLong(latLong);

                if (parsed != null)
                {
                    _latitude = Util.ConvertDegree(parsed.Item1);
                    _longitude = Util.ConvertDegree(parsed.Item2);
                }
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
