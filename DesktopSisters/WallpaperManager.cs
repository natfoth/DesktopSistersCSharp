using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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

            
            Update();
        }

        public void Update()
        {
            GenerateWallpaper();
            Save();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, String pvParam, UInt32 fWinIni);
        private static UInt32 SPI_SETDESKWALLPAPER = 20;
        private static UInt32 SPIF_UPDATEINIFILE = 0x1;

        public void Save()
        {
            string tempPath = Path.GetTempPath();
            string filePath = Path.Combine(tempPath, "WallpaperNew.bmp");

            Wallpaper.Save(filePath);

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filePath, SPIF_UPDATEINIFILE);
        }

        public void GenerateWallpaper()
        {
            if (TimeManager.IsNightTime)
            {
                GenerateNightBackground();
                GenerateNightSky();
                GenerateNightForground();
            }

            if (TimeManager.IsDayTime)
            {
                GenerateDayBackground();
                GenerateDaySky();
                GenerateDayForground();
            }

            GenerateCanvasEffect();
        }

        #region Canvas Generation
        public void GenerateCanvasEffect()
        {
            var canvasResized = new Bitmap(ResW, ResH);

            using (var canvas = Graphics.FromImage(canvasResized)) // resize the canvas to fit the wallpaper size
            {
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                canvas.DrawImage(Canvas, new Rectangle(0, 0, Wallpaper.Width, Wallpaper.Height));
                canvas.Save();
            }

            Benchmark.Start();
            var wallpaperLockBitmap = new LockBitmap(Wallpaper);
            wallpaperLockBitmap.LockBits();

            for (int y = 0; y < wallpaperLockBitmap.Height; y++)
            {
                for (int x = 0; x < wallpaperLockBitmap.Width; x++)
                {

                    var blend = canvasResized.GetPixel(x, y);
                    var src = wallpaperLockBitmap.GetPixel(x, y);

                    int red = src.R;
                    int green = src.G;
                    int blue = src.B;
                    int alpha = src.A;

                    if (src.R < 128)
                        red = Limit255((2 * (double)src.R * (double)blend.R) / 255);
                    else
                        red = Limit255(255 - (2 * (255 - blend.R) * (255 - src.R)) / 255);

                    if (src.G < 128)
                        green = Limit255((2 * (double)src.G * (double)blend.G) / 255);
                    else
                        green = Limit255(255 - (2 * (255 - blend.G) * (255 - src.G)) / 255);

                    if (src.B < 128)
                        blue = Limit255((2 * (double)src.B * (double)blend.B) / 255);
                    else
                        blue = Limit255(255 - (2 * (255 - blend.B) * (255 - src.B)) / 255);


                    alpha = Limit255(((double)src.A * (double)blend.A) / 255.0f);

                    var newColor = Color.FromArgb(alpha, red, green, blue);

                   // var dest = src;
                   // dest = Merge(dest, newColor);


                    wallpaperLockBitmap.SetPixel(x, y, newColor);
                }
            }

            wallpaperLockBitmap.UnlockBits();
            Benchmark.End();
            double miliseconds = Benchmark.GetMiliseconds();
        }
        #endregion

        #region Night Generation
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
                    var Base = BlendColor(BG_COLOR, HORIZON_COLOR, (double) y/(double) ResH);
                    var Night = BlendColor(BLEED_COLOR, NIGHT_COLOR, ((double) (Math.Max(x - ResW + BleedWidth, 0)))/(double) BleedWidth);
                    var BG = BlendColor(Base, Night, Night.R/255.0f);

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

                #region Stars
                canvas.DrawImage(Stars, new Rectangle(10, 50, ((int)((double)ResW)), (ResH)));
                #endregion

                #region Clouds
                canvas.DrawImageUnscaled(NightClouds[2], (int) (ResW - ((double)(1.0 - TimeManager.NightRatio) * (double)ResW * 0.5)), -20);
                canvas.DrawImageUnscaled(NightClouds[1], (int)(ResW - ((double)(1.0 - TimeManager.NightRatio) * (double)ResW * 0.5)) - 220, -20);
                canvas.DrawImageUnscaled(NightClouds[0], (int)(ResW - ((double)(1.0 - TimeManager.NightRatio) * (double)ResW * 0.5)) - 400, -20);
                #endregion

                #region Falling Star
                canvas.DrawImageUnscaled(FallingStar, (int) ((double)TimeManager.NightRatio * (double)ResW) + 500, 200);
                #endregion

                #region Triangle
                canvas.DrawImageUnscaled(Triangle, (int)(ResW - ((double)(1.0 - TimeManager.NightRatio) * (double)ResW * 0.5)) - 400, 50);
                #endregion

                #region Moon

                var moonX = TimeManager.MoonX - (double) Moon.Width/2.0;
                var moonY = TimeManager.MoonY - (double)Moon.Height / 2.0;

                canvas.DrawImageUnscaled(Moon, (int) moonX, (int) moonY);
                #endregion

                canvas.Save();
            }
        }

        public void GenerateNightForground()
        {
            using (var canvas = Graphics.FromImage(Wallpaper))
            {
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;

                #region Landscape
                canvas.DrawImage(LandscapeNight, new Rectangle(0, ResH - LandscapeNight.Height, ((int)((double)ResW)), LandscapeNight.Height));
                #endregion

                #region Luna
                canvas.DrawImage(Luna, new Rectangle(20, ResH - Luna.Height - 10, Luna.Width, Luna.Height));
                #endregion

                canvas.Save();
            }
        }
        #endregion

        public void GenerateDayBackground()
        {
            var INNER_CIRCLE = 1000.0*ResW*1.0/1680.0;
            //INNER_CIRCLE = 1000;
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
                        var alphaToUse = Dist / INNER_CIRCLE;
                        alphaToUse = (1.0 - alphaToUse) * 255.0;
                        if (Dist < 50)
                        {
                            color = Color.Blue;
                        }

                        //alphaToUse = 1;
                        var testColor = Color.FromArgb((int) 255, START_COLOR.R, START_COLOR.G, START_COLOR.B);

                        //color = Merge(INNER_COLOR, testColor);

                        color = BlendColor(INNER_COLOR, testColor, alphaToUse / 255);

                        int bob = 1;
                    }
                    else
                    {
                        var range = Math.Min((Dist - INNER_CIRCLE)*255.0/(INNER_CIRCLE + OUTER_CIRCLE), 1.0);

                        color = BlendColor(INNER_COLOR, OUTER_COLOR, 0);

                        int bob = 1;
                    }

                    wallpaperLockBitmap.SetPixel(x, y, color);
                }
            }

            wallpaperLockBitmap.UnlockBits();
            Benchmark.End();
            double miliseconds = Benchmark.GetMiliseconds();
        }

        public void GenerateDaySky()
        {
            using (var canvas = Graphics.FromImage(Wallpaper))
            {
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;

                #region Clouds
                canvas.DrawImageUnscaled(DayClouds[2], (int)(ResW - ((double)(1.0 - TimeManager.DayRatio) * (double)ResW * 0.5)), -20);
                canvas.DrawImageUnscaled(DayClouds[1], (int)(ResW - ((double)(1.0 - TimeManager.DayRatio) * (double)ResW * 0.5)) - 220, -20);
                canvas.DrawImageUnscaled(DayClouds[0], (int)(ResW - ((double)(1.0 - TimeManager.DayRatio) * (double)ResW * 0.5)) - 400, -20);
                #endregion

                #region Sun

                var sunX = TimeManager.SunX - (double)Sun.Width / 2.0;
                var sunY = TimeManager.SunY - (double)Sun.Height / 2.0;

                canvas.DrawImageUnscaled(Sun, (int)sunX, (int)sunY);
                #endregion

                canvas.Save();
            }
        }

        public void GenerateDayForground()
        {
            using (var canvas = Graphics.FromImage(Wallpaper))
            {
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;

                #region Landscape
                canvas.DrawImage(Landscape, new Rectangle(0, ResH - Landscape.Height, ((int)((double)ResW)), Landscape.Height));
                #endregion

                #region Celestia
                canvas.DrawImage(Celestia, new Rectangle(20, ResH - Celestia.Height - 10, Celestia.Width, Celestia.Height));
                #endregion

                canvas.Save();
            }
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

        public Color Merge(Color dest, Color src)
        {

            int srcAlpha = src.A;

            if (srcAlpha <= 0)
                return dest;
            else if (srcAlpha == 255)
            {
                return src;
            }


            double newalpha = srcAlpha + (((255 - srcAlpha) * dest.A) / 255.0);// / 255.0;
            int alpha255 = (int) ((dest.A * (255 - srcAlpha))/255.0);//  /255.0;


            var red =  (byte)((src.R * srcAlpha + (dest.R * alpha255)) * newalpha);
            var green = (byte)((src.G * srcAlpha + (dest.G * alpha255)) * newalpha);
            var blue = (byte)((src.B * srcAlpha + (dest.B * alpha255)) * newalpha);

            return Color.FromArgb(255, (int)red, (int)green, (int)blue);

        }

        public Color From0BGR(uint bgrColor)
        {
            // Get the color bytes
            var bytes = BitConverter.GetBytes(bgrColor);

            // Return the color from the byte array
            return Color.FromArgb(bytes[2], bytes[1], bytes[0]);
        }

        public int DIVIDE_BY_255(int input)
        {
            return (input + (((input + 128) >> 8) + 128)) >> 8;
        }

    }

   /* public static class DesktopSisterExtensions
    {
        public static void Merge(this Color color, Color src)
        {
            int destAlpha = color.A;

            if (destAlpha <= 0)
                return;
            else if (destAlpha == 255)
            {
                return;
            }
            double newalpha = destAlpha + DIVIDE_BY_255((255 - destAlpha) * src.A); // / 255.0;
            int alpha255 = DIVIDE_BY_255(src.A * (255 - destAlpha)); //  /255.0;

            var newAlpha = newalpha;
            newalpha = 1.0 / newalpha;

            var red = ((color.R * destAlpha + (color.R * alpha255)) * newalpha);
            var green = ((color.G * destAlpha + (color.G * alpha255)) * newalpha);
            var blue = ((color.B * destAlpha + (color.B * alpha255)) * newalpha);

            color = Color.FromArgb((int)newAlpha, (int)red, (int)green, (int)blue);
        }

        public static int DIVIDE_BY_255(int input)
        {
            return (input + (((input + 128) >> 8) + 128)) >> 8;
        }
    }*/
}
