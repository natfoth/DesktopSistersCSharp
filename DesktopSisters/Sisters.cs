using System;
using System.Threading;
using System.Windows.Forms;
using DesktopSistersCSharpForm;

namespace DesktopSisters
{
    public class Sisters
    {
        private readonly Form _window;

        
        public RenderController RenderController;
        public EventController EventController;

        private System.Timers.Timer _pulse;
        private System.Timers.Timer _newSceneUpdateTimer;

        public Sisters(Form window)
        {
            _window = window;


            RenderController = new RenderController(Configuration.Instance);

            var dateTimetest = DateTime.Parse("08:30:59");
            var test2 = dateTimetest.ToDouble();

            RenderController.AddSceneToQueue(DateTime.Now, "Wallpaper");

           // GenerateDayAndNightCycle();

            _pulse = new System.Timers.Timer();
            _pulse.Elapsed += Pulse;
            _pulse.Interval = 1000; // in miliseconds
            _pulse.Start();

            _newSceneUpdateTimer = new System.Timers.Timer();
            _newSceneUpdateTimer.Elapsed += AddScenePulse;
            _newSceneUpdateTimer.Interval = Configuration.Instance.UpdateInterval * 1000; // in miliseconds
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
        }

        public void UpdateConfig()
        {
            RenderController.UpdateConfig();
        }

        public void UpdateTimer()
        {
            _newSceneUpdateTimer.Interval = Configuration.Instance.UpdateInterval * 1000; // in miliseconds
            _newSceneUpdateTimer.Stop();
            _newSceneUpdateTimer.Start();
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