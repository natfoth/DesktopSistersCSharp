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
using DesktopSisters;

namespace DesktopSistersCSharpForm
{
    public class ImageController
    {
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


        public ImageController()
        {
            Rectangle resolution = Screen.PrimaryScreen.Bounds;

            var ResW = resolution.Width;
            var ResH = resolution.Height;

            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (directory == null)
                return;

            var canvasImage = new Bitmap(Image.FromFile(Path.Combine(directory, "CanvasTexture.jpg")));

            Canvas = new Bitmap(ResW, ResH);

            using (var canvas = Graphics.FromImage(Canvas)) // resize the canvas to fit the wallpaper size
            {
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                canvas.DrawImage(canvasImage, new Rectangle(0, 0, ResW, ResH));
                canvas.Save();
            }

            LoadImages();
        }

        public void LoadImages()
        {
            /*Luna = LoadNightImage("Luna.png");
            Moon = LoadNightImage("Moon.png");
            LandscapeNight = LoadNightImage("LandscapeNight.png");
            Stars = LoadNightImage("Stars.png");
            FallingStar = LoadNightImage("FallingStar.png");
            NightClouds = new Image[3] { LoadNightImage("NightCloud1.png"), LoadNightImage("NightCloud2.png"), LoadNightImage("NightCloud3.png") };
            Triangle = LoadNightImage("Triangle.png");


            Celestia = LoadDayImage("Celestia.png");
            Sun = LoadDayImage("Sun.png");
            Landscape = LoadDayImage("Landscape.png");
            DayClouds = new Image[3] { LoadDayImage("DayCloud1.png"), LoadDayImage("DayCloud2.png"), LoadDayImage("DayCloud3.png") };*/
        }

        public void Update()
        {
            LoadImages();
        }

        public static Image LoadDayImage(string name)
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (directory == null)
                return null;

            var imageLocationName = string.Format("Day/Traditional/{0}", name);
            if (Configuration.Instance.UseNewArtStyle)
                imageLocationName = string.Format("Day/New/{0}", name);


            return Image.FromFile(Path.Combine(directory, imageLocationName));
        }

        

        public static Image LoadNightImage(string name)
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (directory == null)
                return null;

            var imageLocationName = string.Format("Night/Traditional/{0}", name);
            if (Configuration.Instance.UseNewArtStyle)
                imageLocationName = string.Format("Night/New/{0}", name);


            return Image.FromFile(Path.Combine(directory, imageLocationName));
        }

        public static Image LoadEventImage(string name)
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (directory == null)
                return null;

            var imageLocationName = string.Format("Events/Images/{0}", name);


            return Image.FromFile(Path.Combine(directory, imageLocationName));
        }

    }
}
