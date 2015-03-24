﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public sealed class NightColors
    {
        public Color HorizonColor { get; set; } = Color.FromArgb(77, 145, 157);
        public Color NightColor { get; set; } = Color.FromArgb(114, 49, 187);
        public Color BleedColor { get; set; } = Color.FromArgb(52, 50, 101);
        public Color ShineColor { get; set; } = Color.FromArgb(64, 255, 255, 255);
        public Color BackgroundColor { get; set; } = Color.FromArgb(80, 77, 157);
    }

    public sealed class DayColors
    {
        public Color StartColor { get; set; } = Color.FromArgb(255, 243, 102);
        public Color InnerColor { get; set; } = Color.FromArgb(255, 166, 124);
        public Color OuterColor { get; set; } = Color.FromArgb(255, 200, 178);
        public Color RayColor { get; set; } = Color.FromArgb(255, 255, 159);
    }

    public class WallpaperManager
    {
        private Configuration _config;

        public WallpaperManager(TimeManager timeManager, Configuration config)
        {
            TimeManager = timeManager;
            _config = config;
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

        public NightColors NightColors = new NightColors();
        public DayColors DayColors = new DayColors();

        public void Init()
        {

            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (directory == null)
                return;

            Canvas = Image.FromFile(Path.Combine(directory, "CanvasTexture.jpg"));

            Luna = LoadNightImage("Luna.png");
            Moon = LoadNightImage("Moon.png");
            LandscapeNight = LoadNightImage("LandscapeNight.png");
            Stars = LoadNightImage("Stars.png");
            FallingStar = LoadNightImage("FallingStar.png");
            NightClouds = new Image[3] { LoadNightImage("NightCloud1.png"), LoadNightImage("NightCloud2.png"), LoadNightImage("NightCloud3.png") };
            Triangle = LoadNightImage("Triangle.png");


            Celestia = LoadDayImage("Celestia.png");
            Sun = LoadDayImage("Sun.png");
            Landscape = LoadDayImage("Landscape.png");
            DayClouds = new Image[3] { LoadDayImage("DayCloud1.png"), LoadDayImage("DayCloud2.png"), LoadDayImage("DayCloud3.png") };

            Rectangle resolution = Screen.PrimaryScreen.Bounds;

            ResW = resolution.Width;
            ResH = resolution.Height;

            Wallpaper = new Bitmap(ResW, ResH);

            
            Update();
        }

        public Image LoadDayImage(string name)
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (directory == null)
                return null;

            var imageLocationName = string.Format("Day/Traditional/{0}", name);
            if (_config.UseNewArtStyle)
                imageLocationName = string.Format("Day/New/{0}", name);


            return Image.FromFile(Path.Combine(directory, imageLocationName));
        }

        public Image LoadNightImage(string name)
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (directory == null)
                return null;

            var imageLocationName = string.Format("Night/Traditional/{0}", name);
            if (_config.UseNewArtStyle)
                imageLocationName = string.Format("Night/New/{0}", name);


            return Image.FromFile(Path.Combine(directory, imageLocationName));
        }


        public void Update()
        {
            GenerateWallpaper();
            Save();
        }

        public void UpdateConfig(Configuration config)
        {
            _config = config;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, string pvParam, UInt32 fWinIni);
        private static UInt32 SPI_SETDESKWALLPAPER = 20;
        private static UInt32 SPIF_UPDATEINIFILE = 0x1;

        public void Save()
        {
            string tempPath = Path.GetTempPath();
            string filePath = Path.Combine(tempPath, "WallpaperNew.bmp");

            try
            {
                Wallpaper.Save(filePath);
            }
            catch
            {
            }
            
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


                    alpha = 255;

                    var newColor = Color.FromArgb(alpha, red, green, blue);

                    newColor = Merge(src, newColor, 0.65);

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
            var innerCircle = 500; //1000.0 * ResW * 1.0 / 1680.0;
            var outerCircle = 1000.0 * ResW * 1.0 / 1680.0;

            var moonX = TimeManager.MoonX;
            var moonY = TimeManager.MoonY;

            var nightColors = new NightColors();

            if (TimeManager.IsTwilight) {
                var item1 = TimeManager.GetSunPos().Item1;
                nightColors.BackgroundColor = BlendColor(Color.FromArgb(102, 102, 204), nightColors.BackgroundColor, (item1/-6));
                nightColors.HorizonColor = BlendColor(Color.FromArgb(255, 200, 178), nightColors.HorizonColor, (item1 / -6));
            }

            var moonshineRadius = Moon.Width/2;
            var bleedWidth = ResW*2/5;

            Benchmark.Start();
            var wallpaperLockBitmap = new LockBitmap(Wallpaper);
            wallpaperLockBitmap.LockBits();

            for (int y = 0; y < wallpaperLockBitmap.Height; y++)
            {
                for (int x = 0; x < wallpaperLockBitmap.Width; x++)
                {
                    var Dist = Math.Sqrt((moonX - x) * (moonX - x) + (moonY - y) * (moonY - y));

                    var Base = BlendColor(nightColors.BackgroundColor, nightColors.HorizonColor, (double) y/(double) ResH);
                    var Night = BlendColor(nightColors.BleedColor, nightColors.NightColor, ((double) (Math.Max(x - ResW + bleedWidth, 0)))/(double) bleedWidth);
                    var BG = BlendColor(Base, Night, Night.R/255.0f);

                    var color = BG;
                    if (Dist < innerCircle)
                    {
                        var alphaToUse = Dist / innerCircle;
                        alphaToUse = (1.0 - alphaToUse) * 255.0;
                        if (Dist < 50)
                        {
                            color = Color.Red;
                        }

                        //color = Merge(INNER_COLOR, testColor);

                        color = BlendColor(BG, Color.FromArgb(255, 255, 255, 255), (alphaToUse / 255) * 0.2);

                        int bob = 1;
                    }

                    wallpaperLockBitmap.SetPixel(x, y, color);
                }
            }

            wallpaperLockBitmap.UnlockBits();
            Benchmark.End();
            double miliseconds = Benchmark.GetMiliseconds();

        }

        /// <summary>Blends the specified colors together.</summary>
        /// <param name="color">Color to blend onto the background color.</param>
        /// <param name="backColor">Color to blend the other color onto.</param>
        /// <param name="amount">How much of <paramref name="color"/> to keep,
        /// “on top of” <paramref name="backColor"/>.</param>
        /// <returns>The blended colors.</returns>
        public static Color Blend(Color color, Color backColor, double amount) {
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            return Color.FromArgb(r, g, b);
        }

        public void GenerateNightSky()
        {
            using (var canvas = Graphics.FromImage(Wallpaper))
            {
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;

                #region Stars
                canvas.DrawImage(Stars, new Rectangle(0, 0, ((int)((double)ResW)), (ResH)));
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
            var innerCircle = 1000.0*ResW*1.0/1680.0;
            //INNER_CIRCLE = 1000;
            var outerCircle = 1000.0 * ResW * 1.0 / 1680.0;

            Benchmark.Start();
            var wallpaperLockBitmap = new LockBitmap(Wallpaper);
            wallpaperLockBitmap.LockBits();

            var SunX = TimeManager.SunX;
            var SunY = TimeManager.SunY;

            for (int y = 0; y < wallpaperLockBitmap.Height; y++)
            {
                for (int x = 0; x < wallpaperLockBitmap.Width; x++)
                {
                    var dist = Math.Sqrt((SunX - x)*(SunX - x) + (SunY - y)*(SunY - y));

                    Color color = Color.Red;

                    if (dist < innerCircle)
                    {
                        var alphaToUse = dist / innerCircle;
                        alphaToUse = (1.0 - alphaToUse) * 255.0;
                        if (dist < 50)
                        {
                            color = Color.Blue;
                        }

                        //alphaToUse = 1;
                        var testColor = Color.FromArgb((int) 255, DayColors.StartColor.R, DayColors.StartColor.G, DayColors.StartColor.B);

                        //color = Merge(INNER_COLOR, testColor);

                        color = BlendColor(DayColors.InnerColor, testColor, alphaToUse / 255);

                        int bob = 1;
                    }
                    else
                    {
                        var range = Math.Min((dist - innerCircle)*255.0/(innerCircle + outerCircle), 1.0);

                        color = BlendColor(DayColors.InnerColor, DayColors.OuterColor, 0);

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

        public Color Merge(Color dest, Color src, double opacity = 1.0)
        {

            int srcAlpha = (int) (src.A * opacity);

            if (srcAlpha <= 0)
                return dest;
            else if (srcAlpha == 255)
            {
                return src;
            }


            double newalpha = srcAlpha + (((255 - srcAlpha) * dest.A) / 255.0);// / 255.0;
            int alpha255 = (int) ((dest.A * (255 - srcAlpha))/255.0);//  /255.0;

            newalpha = 1.0 / newalpha;

            var red =  ((src.R * srcAlpha + (dest.R * alpha255)) * newalpha);
            var green = ((src.G * srcAlpha + (dest.G * alpha255)) * newalpha);
            var blue = ((src.B * srcAlpha + (dest.B * alpha255)) * newalpha);

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
