using System;
using System.Threading;
using System.Windows.Forms;

namespace DesktopSisters
{
    public class Sisters
    {
        private Configuration _config;
        private readonly Form _window;

        public TimeManager TimeManager;
        public WallpaperManager WallpaperManager;


        private System.Timers.Timer _pulse;

        public Sisters(Configuration config, Form window)
        {
            _config = config;
            _window = window;

            TimeManager = new TimeManager(_config);
            TimeManager.Update();

            WallpaperManager = new WallpaperManager(TimeManager, _config);
            WallpaperManager.Init();


            _pulse = new System.Timers.Timer();
            _pulse.Elapsed += Pulse;
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

        public void Update()
        {
            TimeManager.Update();
            WallpaperManager.Update();
        }

        public void UpdateConfig(Configuration config)
        {
            _config = config;
            WallpaperManager.UpdateConfig(config);
            TimeManager.UpdateConfig(config);
        }
        
    }
}