using System;
using System.Threading;

namespace DesktopSisters
{
    public class Sisters
    {
        private readonly Configuration _config;

        public TimeManager TimeManager;
        public WallpaperManager WallpaperManager;


        private System.Windows.Forms.Timer _pulse;

        public Sisters(Configuration config)
        {
            _config = config;
            TimeManager = new TimeManager(_config.Coordinates);
            TimeManager.Update();

            WallpaperManager = new WallpaperManager(TimeManager);
            WallpaperManager.Init();
            WallpaperManager.Save();


            _pulse = new System.Windows.Forms.Timer();
            _pulse.Tick += new EventHandler(Pulse);
            _pulse.Interval = 30 * 1000; // in miliseconds
            _pulse.Start();
        }

        private void Start()
        {
            TimeManager.Update();
        }

        private void Pulse(object sender, EventArgs e)
        {
            _pulse.Stop();
            _pulse.Start();

            Update();
        }

        private void Update()
        {
            TimeManager.Update();
            WallpaperManager.Update();
        }
        
    }
}