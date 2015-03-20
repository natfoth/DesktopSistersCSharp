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
using NAA;

namespace DesktopSisters
{
    public class Sisters
    {
        public int Height;
        public int Width;
        public int Depth { get; private set; }
        public byte[] Pixels { get; set; }

        public int ResW;
        public int ResH;
        public double DayRatio;
        public double NightRatio;
        public int SunX;
        public int SunY;
        public int MoonX;
        public int MoonY;

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

        public Sisters()
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (directory == null)
                return;

            var CanvasTexture = Image.FromFile(Path.Combine(directory, "CanvasTexture.jpg"));

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

            GenerateDayBackground();

            Save();



            int bob = 1;
        }

        public void Start()
        {
            var lat = ConvertDegree("33°35'N");
            var longi = ConvertDegree("86°50'W");

            int bob = 1;
            SetSunCycleRatio(lat, longi);


        }

        public void GenerateDayBackground()
        {
            Benchmark.Start();
            var lockBitmap = new LockBitmap(Wallpaper);
            lockBitmap.LockBits();


            for (int y = 0; y < lockBitmap.Height; y++)
            {
                for (int x = 0; x < lockBitmap.Width; x++)
                {
                    lockBitmap.SetPixel(x, y, Color.Red);
                }
            }

            lockBitmap.UnlockBits();
            Benchmark.End();
            double miliseconds = Benchmark.GetMiliseconds();
        }

        public void Save()
        {
            string tempPath = Path.GetTempPath();

            Wallpaper.Save(Path.Combine(tempPath, "Wallpaper.png"));

            int bob = 1;
        }



        //Behind Functions

        public double sineOut(double current, double start, double end)
        {
            return end * Math.Sin(current / (end - start) * (Math.PI / 2)) + start;
        }

        public void SetSunCycleRatio(double Latitude, double Longitude)
        {
            var julianDate = DateTime.Now.ToOADate() + 2415018.5;
            var julianDay = (int) julianDate;


            var date = DateTime.Now;
            var isSunrise = false;
            var isSunset = false;
            var sunrise = DateTime.Now;
            var sunset = DateTime.Now;


            var thisTime = DateTime.Now;
            var isDaylight = TimeZoneInfo.Local.IsDaylightSavingTime(thisTime);

            var zone = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).Hours;


            var jd = Util.calcJD(date);
            double sunRise = Util.calcSunRiseUTC(jd, Latitude, Longitude);
            double sunSet = Util.calcSunSetUTC(jd, Latitude, Longitude);

            var sunriseString = Util.getTimeString(sunRise, zone, jd, false);
            var sunsetString = Util.getTimeString(sunSet, zone, jd, false);

            double currentTimeDec = Double.Parse(String.Format("{0}.{1}", date.Hour, date.Minute));
            double sunRiseTimeDec = Double.Parse(sunriseString.Replace(":", "."));
            double sunSetTimeDec = Double.Parse(sunsetString.Replace(":", "."));

            double startingSunRise = sunRiseTimeDec - 12; // 12 hours is the length of a cycle


            DayRatio = (currentTimeDec - sunRiseTimeDec)/(sunSetTimeDec - sunRiseTimeDec);
            NightRatio = (currentTimeDec - startingSunRise) / (sunRiseTimeDec - startingSunRise);

            if (currentTimeDec > sunSetTimeDec && currentTimeDec > 15)
            {
                var amountToSub = currentTimeDec - sunSetTimeDec;
                currentTimeDec = 0 - amountToSub;
                NightRatio = (currentTimeDec - startingSunRise) / (sunRiseTimeDec - startingSunRise);
            }
        }



        public double ConvertDegree(string toConvert)
        {
            //"77 2 00.000W"; Sample Input from textBox1
            var input = toConvert;
            double sd = 0.0;
            double min = 0.0;
            double sec = 0.0;
            double deg = 0.0;
            string direction = input.Substring((input.Length - 1), 1);
            string sign = "";

            if ((direction.ToUpper() == "S") || (direction.ToUpper() == "W"))
            {
                sign = "-";
            }

            int degIndex = input.IndexOf('°');
            if (degIndex > 0)
            {
                var text = input.Substring(0, degIndex);
                deg = Convert.ToDouble(text);
            }

            int minIndex = input.IndexOf('\'');
            if (minIndex > 0)
            {
                var text = input.Substring(degIndex+1, minIndex - (degIndex + 1));
                min = Convert.ToDouble(text);
            }

            min = min / ((double)60);
            sec = sec / ((double)3600);
            sd = deg + min + sec;

            if (!(string.IsNullOrEmpty(sign)))
            {
                sd = sd * (-1);
            }

            sd = Math.Round(sd, 6);
            string sdnew = Convert.ToString(sd);
            string sdnew1 = "";

            sdnew1 = string.Format("{0:0.000000}", sd);
            return sd;
            //EXPECTED OUTPUT -77.03333
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

    public class Benchmark
    {
        private static DateTime startDate = DateTime.MinValue;
        private static DateTime endDate = DateTime.MinValue;

        public static TimeSpan Span { get { return endDate.Subtract(startDate); } }

        public static void Start() { startDate = DateTime.Now; }

        public static void End() { endDate = DateTime.Now; }

        public static double GetMiliseconds()
        {
            if (endDate == DateTime.MinValue) return 0.0;
            else return Span.TotalMilliseconds;
        }
    }

    public class LockBitmap
    {
        Bitmap source = null;
        IntPtr Iptr = IntPtr.Zero;
        BitmapData bitmapData = null;

        public byte[] Pixels { get; set; }
        public int Depth { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public LockBitmap(Bitmap source)
        {
            this.source = source;
        }

        /// <summary>
        /// Lock bitmap data
        /// </summary>
        public void LockBits()
        {
            try
            {
                // Get width and height of bitmap
                Width = source.Width;
                Height = source.Height;

                // get total locked pixels count
                int PixelCount = Width * Height;

                // Create rectangle to lock
                Rectangle rect = new Rectangle(0, 0, Width, Height);

                // get source bitmap pixel format size
                Depth = System.Drawing.Bitmap.GetPixelFormatSize(source.PixelFormat);

                // Check if bpp (Bits Per Pixel) is 8, 24, or 32
                if (Depth != 8 && Depth != 24 && Depth != 32)
                {
                    throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
                }

                // Lock bitmap and return bitmap data
                bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite,
                                             source.PixelFormat);

                // create byte array to copy pixel values
                int step = Depth / 8;
                Pixels = new byte[PixelCount * step];
                Iptr = bitmapData.Scan0;

                // Copy data from pointer to array
                Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Unlock bitmap data
        /// </summary>
        public void UnlockBits()
        {
            try
            {
                // Copy data from byte array to pointer
                Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);

                // Unlock bitmap data
                source.UnlockBits(bitmapData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get the color of the specified pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color GetPixel(int x, int y)
        {
            Color clr = Color.Empty;

            // Get color components count
            int cCount = Depth / 8;

            // Get current index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            if (i > Pixels.Length - cCount)
                throw new IndexOutOfRangeException();

            if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                byte a = Pixels[i + 3]; // a
                clr = Color.FromArgb(a, r, g, b);
            }
            if (Depth == 24) // For 24 bpp get Red, Green and Blue
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                clr = Color.FromArgb(r, g, b);
            }
            if (Depth == 8)
            // For 8 bpp get color value (Red, Green and Blue values are the same)
            {
                byte c = Pixels[i];
                clr = Color.FromArgb(c, c, c);
            }
            return clr;
        }

        /// <summary>
        /// Set the color of the specified pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(int x, int y, Color color)
        {
            // Get color components count
            int cCount = Depth / 8;

            // Get current index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
                Pixels[i + 3] = color.A;
            }
            if (Depth == 24) // For 24 bpp set Red, Green and Blue
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
            }
            if (Depth == 8)
            // For 8 bpp set color value (Red, Green and Blue values are the same)
            {
                Pixels[i] = color.B;
            }
        }
    }
}
