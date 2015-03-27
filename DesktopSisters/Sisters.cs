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

            RenderController.AddSceneToQueue(DateTime.Now, "Wallpaper");

            /*RenderController.AddSceneToQueue(DateTime.Parse("6:00 pm"), "Wallpaper");
            RenderController.AddSceneToQueue(DateTime.Parse("1:00 pm"), "Wallpaper1");
            RenderController.AddSceneToQueue(DateTime.Parse("4:00 pm"), "Wallpaper2");
            RenderController.AddSceneToQueue(DateTime.Parse("9:00 pm"), "Wallpaper3");*/
        }

        public void UpdateScene()
        {
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
        
    }
}