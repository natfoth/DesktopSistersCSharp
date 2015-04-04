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
        private Configuration _config;
        private ImageController _imageController;
        private EventController _eventController;

        public WallpaperManager(TimeManager timeManager, ImageController imageController, EventController eventController, Configuration config)
        {
            TimeManager = timeManager;
            _imageController = imageController;
            _eventController = eventController;
            _config = config;
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

        

        public void UpdateConfig(Configuration config)
        {
            _config = config;
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
                GenerateDayBackground();
                GenerateDaySky();
                GenerateDayForground();
            }

             GenerateCanvasEffect();

            Benchmark.End();
            var miliseconds = Benchmark.GetMiliseconds();
        }

        #region Canvas Generation
        public void GenerateCanvasEffect()
        {
            BitmapData bitmapData1 = Wallpaper.LockBits(new Rectangle(0, 0,
                                     Wallpaper.Width, Wallpaper.Height),
                                     ImageLockMode.ReadWrite,
                                     PixelFormat.Format32bppArgb);

            BitmapData bitmapDataCanvas = _imageController.Canvas.LockBits(new Rectangle(0, 0,
                                     _imageController.Canvas.Width, _imageController.Canvas.Height),
                                     ImageLockMode.ReadWrite,
                                     PixelFormat.Format32bppArgb);

            byte bitsPerPixel = 4;

            unsafe
            {
                byte* scan0 = (byte*)bitmapData1.Scan0.ToPointer();
                byte* scan1 = (byte*)bitmapDataCanvas.Scan0.ToPointer();

                Parallel.For(0, bitmapData1.Height, y =>
                {
                    Parallel.For(0, bitmapData1.Width, x =>
                    {
                        var imagePointer1 = scan0 + y * bitmapData1.Stride + x * bitsPerPixel;
                        var imagePointerCanvas = scan1 + y * bitmapDataCanvas.Stride + x * bitsPerPixel;

                        byte[] newColor = new byte[4];

                        if (imagePointer1[0] < 128)
                            newColor[0] = (byte)((2 * imagePointer1[0] * imagePointerCanvas[0]) / 255);
                        else
                            newColor[0] = (byte)(255 - (2 * (255 - imagePointerCanvas[0]) * (255 - imagePointer1[0])) / 255);

                        if (imagePointer1[1] < 128)
                            newColor[1] = (byte)((2 * imagePointer1[1] * imagePointerCanvas[1]) / 255);
                        else
                            newColor[1] = (byte)(255 - (2 * (255 - imagePointerCanvas[1]) * (255 - imagePointer1[1])) / 255);

                        if (imagePointer1[2] < 128)
                            newColor[2] = (byte)((2 * imagePointer1[2] * imagePointerCanvas[2]) / 255);
                        else
                            newColor[2] = (byte)(255 - (2 * (255 - imagePointerCanvas[2]) * (255 - imagePointer1[2])) / 255);

                        newColor[3] = 255;

                        var color = ImgProcLib.Merge(new[] { imagePointer1[0], imagePointer1[1], imagePointer1[2], imagePointer1[3] }, newColor, 0.65);

                        imagePointer1[0] = color[0];
                        imagePointer1[1] = color[1];
                        imagePointer1[2] = color[2];
                        imagePointer1[3] = 255;
                    });
                });

            }//end unsafe
            Wallpaper.UnlockBits(bitmapData1);
            _imageController.Canvas.UnlockBits(bitmapDataCanvas);
        }
        #endregion

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
            using (var canvas = Graphics.FromImage(Wallpaper))
            {
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;

                #region Stars
                canvas.DrawImage(_imageController.Stars, new Rectangle(0, 0, ((int)((double)ResW)), (ResH)));
                #endregion

                #region Clouds
                canvas.DrawImageUnscaled(_imageController.NightClouds[2], (int) (ResW - ((double)(1.0 - TimeManager.NightRatio) * (double)ResW * 0.5)), -20);
                canvas.DrawImageUnscaled(_imageController.NightClouds[1], (int)(ResW - ((double)(1.0 - TimeManager.NightRatio) * (double)ResW * 0.5)) - 220, -20);
                canvas.DrawImageUnscaled(_imageController.NightClouds[0], (int)(ResW - ((double)(1.0 - TimeManager.NightRatio) * (double)ResW * 0.5)) - 400, -20);
                #endregion

                #region Falling Star
                canvas.DrawImageUnscaled(_imageController.FallingStar, (int) ((double)TimeManager.NightRatio * (double)ResW) + 500, 200);
                #endregion

                #region Triangle
                canvas.DrawImageUnscaled(_imageController.Triangle, (int)(ResW - ((double)(1.0 - TimeManager.NightRatio) * (double)ResW * 0.5)) - 400, 50);
                #endregion

                #region Moon

                var moonX = TimeManager.MoonX - (double)_imageController.Moon.Width/2.0;
                var moonY = TimeManager.MoonY - (double)_imageController.Moon.Height / 2.0;

                canvas.DrawImageUnscaled(_imageController.Moon, (int) moonX, (int) moonY);
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
                canvas.DrawImage(_imageController.LandscapeNight, new Rectangle(0, ResH - _imageController.LandscapeNight.Height, ((int)((double)ResW)), _imageController.LandscapeNight.Height));
                #endregion

                #region Luna
                canvas.DrawImage(_imageController.Luna, new Rectangle(20, ResH - _imageController.Luna.Height - 10, _imageController.Luna.Width, _imageController.Luna.Height));
                #endregion

                _eventController.RenderNightForgrounds(canvas, TimeManager);

                canvas.Save();
            }
        }
        #endregion

        public void GenerateDayBackground()
        {
            var innerCircle = 1000.0 * ResW * 1.0 / 1680.0;
            //INNER_CIRCLE = 1000;
            var outerCircle = 1000.0 * ResW * 1.0 / 1680.0;
            var SunX = TimeManager.SunX;
            var SunY = TimeManager.SunY;

            byte[] inner_color = new[] { DayColors.InnerColor.B, DayColors.InnerColor.G, DayColors.InnerColor.R, DayColors.InnerColor.A };
            byte[] start_color = new[] { DayColors.StartColor.B, DayColors.StartColor.G, DayColors.StartColor.R, DayColors.StartColor.A };


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

                        var dist = Math.Sqrt((SunX - x) * (SunX - x) + (SunY - y) * (SunY - y));

                        if (dist < innerCircle)
                        {
                            var alphaToUse = dist / innerCircle;
                            alphaToUse = (1.0 - alphaToUse);

                            // BlendColor(inner_color, start_color, 1.0);
                            var color = ImgProcLib.BlendColor(inner_color, start_color, alphaToUse);

                            imagePointer1[0] = color[0];
                            imagePointer1[1] = color[1];
                            imagePointer1[2] = color[2];
                            imagePointer1[3] = 255;
                        }
                        else
                        {
                            imagePointer1[0] = inner_color[0];
                            imagePointer1[1] = inner_color[1];
                            imagePointer1[2] = inner_color[2];
                            imagePointer1[3] = 255;
                        }
                    });
                });

            }//end unsafe
            Wallpaper.UnlockBits(bitmapData1);
        }

        public void GenerateDayBackgroundEffect()
        {
            var innerCircle = 1000.0 * ResW * 1.0 / 1680.0;
            //INNER_CIRCLE = 1000;
            var outerCircle = 1000.0 * ResW * 1.0 / 1680.0;
            var SunX = TimeManager.SunX;
            var SunY = TimeManager.SunY;


            BitmapData bitmapData1 = Wallpaper.LockBits(new Rectangle(0, 0,
                                     Wallpaper.Width, Wallpaper.Height),
                                     ImageLockMode.ReadOnly,
                                     PixelFormat.Format32bppArgb);
            int a = 0;
            byte[] inner_color = new[] { DayColors.InnerColor.B, DayColors.InnerColor.G, DayColors.InnerColor.R, DayColors.InnerColor.A };
            byte[] start_color = new[] { DayColors.StartColor.B, DayColors.StartColor.G, DayColors.StartColor.R, DayColors.StartColor.A };
            unsafe
            {
                byte* imagePointer1 = (byte*)bitmapData1.Scan0;

                for (int y = 0; y < bitmapData1.Height; y++)
                {
                    for (int x = 0; x < bitmapData1.Width; x++)
                    {
                        var dist = Math.Sqrt((SunX - x) * (SunX - x) + (SunY - y) * (SunY - y));

                        if (dist < innerCircle)
                        {
                            var alphaToUse = dist / innerCircle;
                            alphaToUse = (1.0 - alphaToUse);

                            // BlendColor(inner_color, start_color, 1.0);
                            var color = ImgProcLib.BlendColor(inner_color, start_color, alphaToUse);

                            imagePointer1[0] = color[0];
                            imagePointer1[1] = color[1];
                            imagePointer1[2] = color[2];
                            imagePointer1[3] = 255;
                        }
                        else
                        {
                            imagePointer1[0] = inner_color[0];
                            imagePointer1[1] = inner_color[1];
                            imagePointer1[2] = inner_color[2];
                            imagePointer1[3] = 255;
                        }

                        //4 bytes per pixel
                        imagePointer1 += 4;
                    }//end for j
                     //4 bytes per pixel
                    imagePointer1 += bitmapData1.Stride -
                                    (bitmapData1.Width * 4);
                }//end for i
            }//end unsafe
            Wallpaper.UnlockBits(bitmapData1);
        }

        public void GenerateDaySky()
        {
            using (var canvas = Graphics.FromImage(Wallpaper))
            {
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;

                #region Clouds
                canvas.DrawImageUnscaled(_imageController.DayClouds[2], (int)(ResW - ((double)(1.0 - TimeManager.DayRatio) * (double)ResW * 0.5)), -20);
                canvas.DrawImageUnscaled(_imageController.DayClouds[1], (int)(ResW - ((double)(1.0 - TimeManager.DayRatio) * (double)ResW * 0.5)) - 220, -20);
                canvas.DrawImageUnscaled(_imageController.DayClouds[0], (int)(ResW - ((double)(1.0 - TimeManager.DayRatio) * (double)ResW * 0.5)) - 400, -20);
                #endregion

                #region Sun

                

                var sunHeight = Math.Min(_imageController.Sun.Height, ResH / 3.4);
                var ratio = (double)sunHeight / _imageController.Sun.Height;
                var sunWidth = (int)(_imageController.Sun.Width * ratio);

                var sunX = TimeManager.SunX - (double)sunWidth / 2.0;
                var sunY = TimeManager.SunY - (double)sunHeight / 2.0;

                canvas.DrawImage(_imageController.Sun, new Rectangle((int) sunX, (int) sunY, sunWidth, (int) sunHeight));
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
                canvas.DrawImage(_imageController.Landscape, new Rectangle(0, ResH - _imageController.Landscape.Height, ((int)((double)ResW)), _imageController.Landscape.Height));
                #endregion

                var celestiaHeight = Math.Min(_imageController.Celestia.Height, 450);
                var ratio = (double)celestiaHeight / _imageController.Celestia.Height;
                var celestWidth = (int)(_imageController.Celestia.Width * ratio);

                #region Celestia
                canvas.DrawImage(_imageController.Celestia, new Rectangle(20, ResH - celestiaHeight - 10, celestWidth, celestiaHeight));
                #endregion

                _eventController.RenderDayForgrounds(canvas, TimeManager);

                canvas.Save();
            }
        }

        

    }
}
