using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopSisters
{
    public class Sisters
    {
        public int Height;
        public int Width;
        public int Depth { get; private set; }
        public byte[] Pixels { get; set; }

        public TimeManager TimeManager;
        public WallpaperManager WallpaperManager;

        public double DayRatio => TimeManager.DayRatio;


        public Sisters()
        {
            TimeManager = new TimeManager();
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

    class Program
    {
        private static void Main(string[] args)
        {
            var app = new Sisters();
            app.Start();
        }
    }
}
