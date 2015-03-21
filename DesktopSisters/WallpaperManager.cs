using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            if (TimeManager.IsNightTime)
            {
                GenerateNightBackground();
                GenerateNightSky();
            }

            if (TimeManager.IsDayTime)
            {
                GenerateDayBackground();
            }
        }

        public void GenerateNightBackground()
        {
            var MoonX = TimeManager.MoonX;
            var MoonY = TimeManager.MoonY;

            //80, 77, 157
            //77, 145, 157
            //114, 49, 187
            //52, 50, 101

            var BG_COLOR = Color.FromArgb(80, 77, 157);
            var HORIZON_COLOR = Color.FromArgb(77, 145, 157);
            var NIGHT_COLOR = Color.FromArgb(114, 49, 187);
            var BLEED_COLOR = Color.FromArgb(52, 50, 101);
            var SHINE_COLOR = Color.FromArgb(64, 255, 255, 255);

            /* var BG_COLOR = 0xFF504D9d;
             var HORIZON_COLOR = 0xFF4D919D;
             var NIGHT_COLOR = 0xAA7231BB;
             var BLEED_COLOR = 0x00343265;*/

            var MOONSHINE_RADIUS = Moon.Width/2;
            var BleedWidth = ResW*2/5;


            Benchmark.Start();
            var wallpaperLockBitmap = new LockBitmap(Wallpaper);
            wallpaperLockBitmap.LockBits();

            var SunX = TimeManager.SunX;
            var SunY = TimeManager.SunY;

            for (int y = 0; y < wallpaperLockBitmap.Height; y++)
            {
                for (int x = 0; x < wallpaperLockBitmap.Width; x++)
                {
                    if (x > 2000)
                    {
                        int bob = 1;
                    }

                    var Base = BlendColor(BG_COLOR, HORIZON_COLOR, (double)y /(double)ResH);
                    var Night = BlendColor(BLEED_COLOR, NIGHT_COLOR, ((double)(Math.Max(x - ResW + BleedWidth, 0)))/ (double)BleedWidth);
                    var BG = BlendColor(Base, Night, Night.R / 255.0f);


                    wallpaperLockBitmap.SetPixel(x, y, BG);
                }
            }

            wallpaperLockBitmap.UnlockBits();
            Benchmark.End();
            double miliseconds = Benchmark.GetMiliseconds();

        }

        public void GenerateNightSky()
        {
            using (var canvas = Graphics.FromImage(Wallpaper))
            {
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                var moonX = TimeManager.MoonX - (double) Moon.Width/2.0;
                var moonY = TimeManager.MoonY - (double)Moon.Height / 2.0;

                canvas.DrawImageUnscaled(Moon, (int) moonX, (int) moonY);
                canvas.Save();
            }
        }

        public void GenerateDayBackground()
        {
            var INNER_CIRCLE = 1000.0*ResW*1.0/1680.0;
            var OUTER_CIRCLE = 1000.0 * ResW * 1.0 / 1680.0;

            var START_COLOR = Color.FromArgb(255, 251, 205);
            var INNER_COLOR = Color.FromArgb(255, 221, 205);
            var OUTER_COLOR = Color.FromArgb(255, 200, 178);
            var RAY_COLOR = Color.FromArgb(255, 255, 210);

            


            Benchmark.Start();
            var wallpaperLockBitmap = new LockBitmap(Wallpaper);
            wallpaperLockBitmap.LockBits();

            var SunX = TimeManager.SunX;
            var SunY = TimeManager.SunY;

            for (int y = 0; y < wallpaperLockBitmap.Height; y++)
            {
                for (int x = 0; x < wallpaperLockBitmap.Width; x++)
                {
                    var Dist = Math.Sqrt((SunX - x)*(SunX - x) + (SunY - y)*(SunY - y));

                    Color color = Color.Red;

                    if (Dist < INNER_CIRCLE)
                    {
                        if (Dist < 1300)
                        {
                            int b123ob = 1;
                        }

                        var range = Dist / INNER_CIRCLE;

                        color = BlendColor(START_COLOR, INNER_COLOR, range);

                        int bob = 1;
                    }
                    else
                    {
                        var range = Math.Min((Dist - INNER_CIRCLE)*255.0/(INNER_CIRCLE + OUTER_CIRCLE), 1.0);

                        color = BlendColor(INNER_COLOR, OUTER_COLOR, range);

                        int bob = 1;
                    }

                    wallpaperLockBitmap.SetPixel(x, y, color);
                }
            }

            wallpaperLockBitmap.UnlockBits();
            Benchmark.End();
            double miliseconds = Benchmark.GetMiliseconds();
        }

        public int Limit255(double val1)
        {
            if (val1 < 0)
                return 0;
            if (val1 > 255)
                return 255;

            return (int)val1;
        }


        public Color BlendColor(Color firstColor, Color secondColor, double ratio)
        {
            double revRatio = 1.0f - ratio;

            var red = Limit255(firstColor.R * revRatio + secondColor.R * ratio);
            var green = Limit255(firstColor.G * revRatio + secondColor.G * ratio);
            var blue = Limit255(firstColor.B * revRatio + secondColor.B * ratio);
            var alpha = Limit255(firstColor.A * revRatio + secondColor.A * ratio);

            return Color.FromArgb(alpha, red, green, blue);
        }

        public Color From0BGR(uint bgrColor)
        {
            // Get the color bytes
            var bytes = BitConverter.GetBytes(bgrColor);

            // Return the color from the byte array
            return Color.FromArgb(bytes[2], bytes[1], bytes[0]);
        }
    }
}
