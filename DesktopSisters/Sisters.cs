using System;

namespace DesktopSisters
{
    public class Sisters
    {
        private readonly Configuration _config;
        public int Height;
        public int Width;
        public int Depth { get; private set; }
        public byte[] Pixels { get; set; }

        public TimeManager TimeManager;
        public WallpaperManager WallpaperManager;

        public double DayRatio => TimeManager.DayRatio;


        public Sisters(Configuration config)
        {
            _config = config;
            TimeManager = new TimeManager(_config.Coordinates);
            TimeManager.Update();

            WallpaperManager = new WallpaperManager(TimeManager);
            WallpaperManager.Init();
            WallpaperManager.Save();
        }

        public void Start()
        {
            TimeManager.Update();
        }
        
    }
}