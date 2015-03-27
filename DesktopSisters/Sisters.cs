using System;
using System.Threading;
using System.Windows.Forms;
using DesktopSistersCSharpForm;

namespace DesktopSisters
{
    public class Sisters
    {
        private Configuration _config;
        private readonly Form _window;

        
        public RenderController RenderController;

        private System.Timers.Timer _pulse;
        private System.Timers.Timer _newSceneUpdateTimer;

        public Sisters(Configuration config, Form window)
        {
            _config = config;
            _window = window;

            RenderController = new RenderController(config);

            RenderController.AddSceneToQueue(DateTime.Now, "Wallpaper");

            GenerateDayAndNightCycle();

            _pulse = new System.Timers.Timer();
            _pulse.Elapsed += Pulse;
            _pulse.Interval = 1000; // in miliseconds
            _pulse.Start();

            _newSceneUpdateTimer = new System.Timers.Timer();
            _newSceneUpdateTimer.Elapsed += AddScenePulse;
            _newSceneUpdateTimer.Interval = 10 * 1000; // in miliseconds
            _newSceneUpdateTimer.Start();
        }

        private void Start()
        {
            
        }

        private void Pulse(object sender, EventArgs e)
        {
            _pulse.Stop();
            _pulse.Start();

            RenderController.Pulse();
        }

        private void AddScenePulse(object sender, EventArgs e)
        {
            _newSceneUpdateTimer.Stop();
            _newSceneUpdateTimer.Start();

            RenderController.AddSceneToQueue(DateTime.Now, "Wallpaper.bmp");
        }

        public void UpdateScene()
        {
            RenderController.AddSceneToQueue(DateTime.Now, "Wallpaper.bmp");
            //TimeManager.UpdateScene();
            // WallpaperManager.UpdateScene();
        }

        public void UpdateConfig(Configuration config)
        {
            _config = config;

            RenderController.UpdateConfig(config);
           // WallpaperManager.UpdateConfig(config);
           // TimeManager.UpdateConfig(config);
        }

        public void GenerateDayAndNightCycle()
        {
            var time = DateTime.Parse("7:00 am");
            const int updateTime = 5; // in minutes

            for (int i = 0; i < 288; i++) // 
            {
                RenderController.AddSceneToQueue(time, String.Format("DesktopSisters\\{0:00}_{1:00}.png", time.Hour, time.Minute));
                time = time.AddMinutes(updateTime);
            }

            
        }

        
    }
}