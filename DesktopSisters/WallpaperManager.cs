using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DesktopSisters.Utils;
using DesktopSistersCSharpForm;
using DesktopSistersCSharpForm.Utils;

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
        private ImageController _imageController;
        private EventController _eventController;

        public WallpaperManager(TimeManager timeManager, ImageController imageController, EventController eventController)
        {
            TimeManager = timeManager;
            _imageController = imageController;
            _eventController = eventController;
        }

        public TimeManager TimeManager;

        public int ResW;
        public int ResH;


        public Bitmap Wallpaper;


        public NightColors NightColors = new NightColors();
        public DayColors DayColors = new DayColors();

        public void Init()
        {
            Rectangle resolution = Screen.PrimaryScreen.Bounds;

            ResW = resolution.Width;
            ResH = resolution.Height;

            Wallpaper = new Bitmap(ResW, ResH);
        }

        

        public void UpdateConfig()
        {
            
        }

        

        public Bitmap RenderToBitmap()
        {
            Init();
            GenerateWallpaper();
            return Wallpaper;
        }

        public void GenerateWallpaper()
        {
            Benchmark.Start();

            if (TimeManager.IsNightTime)
            {
                GenerateNightBackground();
                GenerateNightSky();
                GenerateNightForground();
            }

            if (TimeManager.IsDayTime)
            {

            }

            Benchmark.End();
            var miliseconds = Benchmark.GetMiliseconds();
        }


        #region Night Generation
        public void GenerateNightBackground()
        {
            var innerCircle = 500;

            var moonX = TimeManager.MoonX;
            var moonY = TimeManager.MoonY;

            var nightColors = new NightColors();

            byte[] backgroundColor = new[] { nightColors.BackgroundColor.B, nightColors.BackgroundColor.G, nightColors.BackgroundColor.R, nightColors.BackgroundColor.A };
            byte[] horizonColor = new[] { nightColors.HorizonColor.B, nightColors.HorizonColor.G, nightColors.HorizonColor.R, nightColors.HorizonColor.A };
            byte[] bleedColor = new[] { nightColors.BleedColor.B, nightColors.BleedColor.G, nightColors.BleedColor.R, nightColors.BleedColor.A };
            byte[] nightColor = new[] { nightColors.NightColor.B, nightColors.NightColor.G, nightColors.NightColor.R, nightColors.NightColor.A };

            if (TimeManager.IsTwilight)
            {
                var item1 = TimeManager.GetSunPos().Item1;
                backgroundColor = ImgProcLib.BlendColor(Color.FromArgb(204, 102, 102).ToByteArray(), backgroundColor, (item1 / -6));
                horizonColor = ImgProcLib.BlendColor(Color.FromArgb(255, 200, 178).ToByteArray(), horizonColor, (item1 / -6));
            }


            var bleedWidth = ResW * 2 / 5;


            BitmapData bitmapData1 = Wallpaper.LockBits(new Rectangle(0, 0,
                                     Wallpaper.Width, Wallpaper.Height),
                                     ImageLockMode.ReadOnly,
                                     PixelFormat.Format32bppArgb);

            byte bitsPerPixel = 4;

            unsafe
            {
                byte* scan0 = (byte*)bitmapData1.Scan0.ToPointer();

                Parallel.For(0, bitmapData1.Height, y =>
                {
                    Parallel.For(0, bitmapData1.Width, x =>
                    {
                        var imagePointer1 = scan0 + y * bitmapData1.Stride + x * bitsPerPixel;

                        var Dist = Math.Sqrt((moonX - x) * (moonX - x) + (moonY - y) * (moonY - y));

                        var Base = ImgProcLib.BlendColor(backgroundColor, horizonColor, (double)y / (double)ResH);
                        var Night = ImgProcLib.BlendColor(bleedColor, nightColor, ((double)(Math.Max(x - ResW + bleedWidth, 0))) / (double)bleedWidth);
                        var BG = ImgProcLib.BlendColor(Base, Night, Night[2] / 255.0f); // blends the value of (red / 255)

                        var color = BG;
                        if (Dist < innerCircle)
                        {
                            var alphaToUse = Dist / innerCircle;
                            alphaToUse = (1.0 - alphaToUse) * 255.0;

                            color = ImgProcLib.BlendColor(BG, Color.FromArgb(255, 255, 255, 255).ToByteArray(), (alphaToUse / 255) * 0.2);
                        }

                        imagePointer1[0] = color[0];
                        imagePointer1[1] = color[1];
                        imagePointer1[2] = color[2];
                        imagePointer1[3] = 255;
                    });
                });

            }//end unsafe
            Wallpaper.UnlockBits(bitmapData1);
        }
        public void GenerateNightSky()
        {
            using (var g = Graphics.FromImage(Wallpaper))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                #region Stars
                g.DrawImage(_imageController.Stars, new Rectangle(0, 0, ((int)((double)ResW)), (ResH)));
                #endregion

                #region Clouds
                g.DrawImageUnscaled(_imageController.NightClouds[2], (int) (ResW - ((double)(1.0 - TimeManager.NightRatio) * (double)ResW * 0.5)), -20);
                g.DrawImageUnscaled(_imageController.NightClouds[1], (int)(ResW - ((double)(1.0 - TimeManager.NightRatio) * (double)ResW * 0.5)) - 220, -20);
                g.DrawImageUnscaled(_imageController.NightClouds[0], (int)(ResW - ((double)(1.0 - TimeManager.NightRatio) * (double)ResW * 0.5)) - 400, -20);
                #endregion

                #region Falling Star
                g.DrawImageUnscaled(_imageController.FallingStar, (int) ((double)TimeManager.NightRatio * (double)ResW) + 500, 200);
                #endregion

                #region Triangle
                g.DrawImageUnscaled(_imageController.Triangle, (int)(ResW - ((double)(1.0 - TimeManager.NightRatio) * (double)ResW * 0.5)) - 400, 50);
                #endregion

                #region Moon

                var moonX = TimeManager.MoonX - (double)_imageController.Moon.Width/2.0;
                var moonY = TimeManager.MoonY - (double)_imageController.Moon.Height / 2.0;

                g.DrawImageUnscaled(_imageController.Moon, (int) moonX, (int) moonY);
                #endregion

                g.Save();
            }
        }

        public void GenerateNightForground()
        {
            using (var canvas = Graphics.FromImage(Wallpaper))
            {
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;

                #region Landscape
                canvas.DrawImage(_imageController.LandscapeNight, new Rectangle(0, ResH - _imageController.LandscapeNight.Height, ((int)((double)ResW)), _imageController.LandscapeNight.Height));
                #endregion

                #region Luna
                canvas.DrawImage(_imageController.Luna, new Rectangle(20, ResH - _imageController.Luna.Height - 10, _imageController.Luna.Width, _imageController.Luna.Height));
                #endregion

              //  _eventController.RenderNightForgrounds(canvas, TimeManager);

                canvas.Save();
            }
        }
        #endregion


    }
}
