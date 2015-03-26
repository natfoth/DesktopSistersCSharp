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

        public Bitmap Canvas;

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
            Rectangle resolution = Screen.PrimaryScreen.Bounds;

            ResW = resolution.Width;
            ResH = resolution.Height;

            Wallpaper = new Bitmap(ResW, ResH);

            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (directory == null)
                return;

            var canvasImage = new Bitmap(Image.FromFile(Path.Combine(directory, "CanvasTexture.jpg")));

            Canvas = new Bitmap(ResW, ResH);

            using (var canvas = Graphics.FromImage(Canvas)) // resize the canvas to fit the wallpaper size
            {
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                canvas.DrawImage(canvasImage, new Rectangle(0, 0, Wallpaper.Width, Wallpaper.Height));
                canvas.Save();
            }

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

            string filePathTemp = Path.Combine(tempPath, String.Format("WallpaperNew_{0:00}_{1:00}.jpg", TimeManager._dateTime.Hour, TimeManager._dateTime.Minute));
            if (TimeManager._dateTime.Hour < 7)
                filePathTemp = Path.Combine(tempPath, String.Format("WallpaperNew_3{0:00}_{1:00}.jpg", TimeManager._dateTime.Hour, TimeManager._dateTime.Minute)); // this is just to make it so it goes in order

            try
            {
                Wallpaper.Save(filePath);

               // Wallpaper.Save(filePathTemp); save out the day cycle
            }
            catch
            {
            }
            
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filePath, SPIF_UPDATEINIFILE);
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
                GenerateDayBackgroundEffect();
                GenerateDaySky();
                GenerateDayForground();
            }

            GenerateCanvasEffect();

            Benchmark.End();
            var miliseconds = Benchmark.GetMiliseconds();

            //System.IO.File.WriteAllText(@"C:\Users\Nathaniel\Documents\WriteLines.txt", miliseconds.ToString());

            int bob = 1;
        }

        #region Canvas Generation
        public void GenerateCanvasEffect()
        {
            var innerCircle = 1000.0 * ResW * 1.0 / 1680.0;
            //INNER_CIRCLE = 1000;
            var outerCircle = 1000.0 * ResW * 1.0 / 1680.0;
            var SunX = TimeManager.SunX;
            var SunY = TimeManager.SunY;


            BitmapData bitmapData1 = Wallpaper.LockBits(new Rectangle(0, 0,
                                     Wallpaper.Width, Wallpaper.Height),
                                     ImageLockMode.ReadWrite,
                                     PixelFormat.Format32bppArgb);

            BitmapData bitmapDataCanvas = Canvas.LockBits(new Rectangle(0, 0,
                                     Canvas.Width, Canvas.Height),
                                     ImageLockMode.ReadWrite,
                                     PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* imagePointer1 = (byte*)bitmapData1.Scan0;
                byte* imagePointerCanvas = (byte*)bitmapDataCanvas.Scan0;

                for (int y = 0; y < bitmapData1.Height; y++)
                {
                    for (int x = 0; x < bitmapData1.Width; x++)
                    {
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

                        var color = Merge(new[] { imagePointer1[0], imagePointer1[1], imagePointer1[2], imagePointer1[3] }, newColor, 0.65);

                        imagePointer1[0] = color[0];
                        imagePointer1[1] = color[1];
                        imagePointer1[2] = color[2];
                        imagePointer1[3] = 255;

                        //4 bytes per pixel
                        imagePointer1 += 4;
                        imagePointerCanvas += 4;
                    }//end for j
                     //4 bytes per pixel
                    imagePointer1 += bitmapData1.Stride -
                                    (bitmapData1.Width * 4);
                    imagePointerCanvas += bitmapData1.Stride -
                                    (bitmapData1.Width * 4);
                }//end for i
            }//end unsafe
            Wallpaper.UnlockBits(bitmapData1);
            Canvas.UnlockBits(bitmapDataCanvas);
        }
        #endregion

        #region Night Generation
        public void GenerateNightBackground()
        {
           /* var innerCircle = 500; //1000.0 * ResW * 1.0 / 1680.0;
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
            double miliseconds = Benchmark.GetMiliseconds();*/

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
                            var color = BlendColor(inner_color, start_color, alphaToUse);

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

        public byte[] BlendColor(byte[] firstColor, byte[] secondColor, double ratio)
        {
            double revRatio = 1.0f - ratio;

            var blue = (byte) (firstColor[0] * revRatio + secondColor[0] * ratio);
            var green = (byte) (firstColor[1] * revRatio + secondColor[1] * ratio);
            var red = (byte) (firstColor[2] * revRatio + secondColor[2] * ratio);
            var alpha = (byte) (firstColor[3] * revRatio + secondColor[3] * ratio);

            return new []{blue, green, red, (byte)255};
        }

        public byte[] Merge(byte[] dest, byte[] src, double opacity = 1.0)
        {
            int srcAlpha = (int)(src[3] * opacity);

            if (srcAlpha <= 0)
                return dest;
            else if (srcAlpha == 255)
            {
                return src;
            }

            double newalpha = srcAlpha + (((255 - srcAlpha) * dest[3]) / 255.0);// / 255.0;
            int alpha255 = (int)((dest[3] * (255 - srcAlpha)) / 255.0);//  /255.0;

            newalpha = 1.0 / newalpha;

            var blue = (byte)((src[0] * srcAlpha + (dest[0] * alpha255)) * newalpha);
            var green = (byte)((src[1] * srcAlpha + (dest[1] * alpha255)) * newalpha);
            var red = (byte)((src[2] * srcAlpha + (dest[2] * alpha255)) * newalpha);

            return new[] { blue, green, red, (byte)255 };
        }

    }
}
