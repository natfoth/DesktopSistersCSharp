using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DesktopSisters.Utils;

namespace DesktopSisters
{
    public class WallpaperManager
    {
        public WallpaperManager(TimeManager timeManager)
        {
            TimeManager = timeManager;
        }

        public TimeManager TimeManager;

        public int ResW;
        public int ResH;

        public Image Canvas;

        public Image Luna;
        public Image Moon;
        public Image LandscapeNight;
        public Image Stars;
        public Image FallingStar;
        public Image[] NightClouds;
        public Image Triangle;

        public Image Celestia;
        public Image Sun;
        public Image Landscape;
        public Image[] DayClouds;

        public Bitmap Wallpaper;

        public void Init()
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (directory == null)
                return;

            Canvas = Image.FromFile(Path.Combine(directory, "CanvasTexture.jpg"));

            Luna = Image.FromFile(Path.Combine(directory, "Night/Luna.png"));
            Moon = Image.FromFile(Path.Combine(directory, "Night/Moon.png"));
            LandscapeNight = Image.FromFile(Path.Combine(directory, "Night/LandscapeNight.png"));
            Stars = Image.FromFile(Path.Combine(directory, "Night/Stars.png"));
            FallingStar = Image.FromFile(Path.Combine(directory, "Night/FallingStar.png"));
            NightClouds = new Image[3] { Image.FromFile(Path.Combine(directory, "Night/NightCloud1.png")), Image.FromFile(Path.Combine(directory, "Night/NightCloud2.png")), Image.FromFile(Path.Combine(directory, "Night/NightCloud3.png")) };
            Triangle = Image.FromFile(Path.Combine(directory, "Night/Triangle.png"));


            Celestia = Image.FromFile(Path.Combine(directory, "Day/Celestia.png"));
            Sun = Image.FromFile(Path.Combine(directory, "Day/Sun.png"));
            Landscape = Image.FromFile(Path.Combine(directory, "Day/Landscape.png"));
            DayClouds = new Image[3] { Image.FromFile(Path.Combine(directory, "Day/DayCloud1.png")), Image.FromFile(Path.Combine(directory, "Day/DayCloud2.png")), Image.FromFile(Path.Combine(directory, "Day/DayCloud3.png")) };

            Rectangle resolution = Screen.PrimaryScreen.Bounds;

            ResW = resolution.Width;
            ResH = resolution.Height;

            Wallpaper = new Bitmap(ResW, ResH);

            
            GenerateWallpaper();
        }

        public void Update()
        {
            GenerateWallpaper();
        }

        public void Save()
        {
            string tempPath = Path.GetTempPath();

            Wallpaper.Save(Path.Combine(tempPath, "Wallpaper.png"));
        }

        public void GenerateWallpaper()
        {
            
        }

        public void GenerateDayBackground()
        {
            Benchmark.Start();
            var wallpaperLockBitmap = new LockBitmap(Wallpaper);
            wallpaperLockBitmap.LockBits();


            for (int y = 0; y < wallpaperLockBitmap.Height; y++)
            {
                for (int x = 0; x < wallpaperLockBitmap.Width; x++)
                {
                    wallpaperLockBitmap.SetPixel(x, y, Color.Red);
                }
            }

            wallpaperLockBitmap.UnlockBits();
            Benchmark.End();
            double miliseconds = Benchmark.GetMiliseconds();
        }


    }
}
