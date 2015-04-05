using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using DesktopSisters;
using DesktopSisters.Utils;
using GoogleMaps.LocationServices;

namespace DesktopSistersCSharpForm
{
    public class Scene
    {
        public string Filename { get; set; }
        //private WallpaperManager _wallpaperManager;
        private EventController _eventController;

        public Bitmap Wallpaper;
        public int ResW;
        public int ResH;

        private TimeManager _timeManager;

        public Scene(string fileName, DateTime timeToRender, ImageController imageController, EventController eventController)
        {
            Filename = fileName;

            Rectangle resolution = Screen.PrimaryScreen.Bounds;

            ResW = resolution.Width;
            ResH = resolution.Height;

            Wallpaper = new Bitmap(ResW, ResH);

            _timeManager = new TimeManager(timeToRender, Configuration.Instance.Latitude, Configuration.Instance.Longitude);
            _timeManager.Update();

            Wallpaper = new Bitmap(ResW, ResH);

            _eventController = eventController;

            // _wallpaperManager = new WallpaperManager(timeManager, _imageController, eventController, config);
        }

        public void GenerateScene()
        {
            _eventController.RenderEvents(Wallpaper, _timeManager);
        }

        public Bitmap RenderToBitmap()
        {
            GenerateScene();
            return Wallpaper;
        }

        public Bitmap RenderedScene => RenderToBitmap();

        public void Dispose()
        {
            Wallpaper.Dispose();
        }
    }

    public class RenderController
    {
        
        private bool _rendering;
        private ImageController _imageController;
        private EventController _eventController;

        public List<Scene> Scenes = new List<Scene>(); 

        public RenderController(Configuration config)
        {
            _eventController = new EventController(config);

            Configuration.Instance = config;
            _imageController = new ImageController();

            UpdateConfig(); 
        }

        public void AddSceneToQueue(DateTime dateTime, string filename)
        {
            var newScene = new Scene(filename, dateTime, _imageController, _eventController);
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
            string filePath = Path.Combine(tempPath, String.Format("{0}", sceneToRender.Filename));


            renderedScene.Save(filePath);

            if (sceneToRender.Filename == "Wallpaper.bmp")
            {
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filePath, SPIF_UPDATEINIFILE);
            }

            renderedScene.Dispose();

            _rendering = false;

        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, string pvParam, UInt32 fWinIni);
        private static UInt32 SPI_SETDESKWALLPAPER = 20;
        private static UInt32 SPIF_UPDATEINIFILE = 0x1;

        #region Config
        public void UpdateConfig()
        {
            _imageController.Update();
        }

        
        #endregion

    }
}
