using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using DesktopSisters;
using DesktopSistersCSharpForm.Utils;

namespace DesktopSistersCSharpForm.Events.Base
{
    class BaseCanvas : Event
    {
        public override void SetTimes(TimeManager timeManager)
        {
            StartTime = timeManager.DateTime;
            EndTime = StartTime + Length();
        }

        public override double Chance()
        {
            return 100;
        }

        public override List<EventTags> Tags => new List<EventTags> {EventTags.Filter};

        public override bool BaseEvent => true;

        public override TimeSpan Length()
        {
            return TimeSpan.FromMinutes(Sisters.UpdateTime);
        }

        public override int ZIndex()
        {
            return 31;
        }

        private Bitmap _canvas = null;
        public override void Draw(Bitmap frame, TimeManager timeManager)
        {
            if (_canvas == null)
            {
                var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                if (directory == null)
                    return;

                _canvas = new Bitmap(ResW, ResH);

                var canvasImage = new Bitmap(Image.FromFile(Path.Combine(directory, "CanvasTexture.jpg")));

                using (var canvas = Graphics.FromImage(_canvas)) // resize the canvas to fit the wallpaper size
                {
                    canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    canvas.DrawImage(canvasImage, new Rectangle(0, 0, ResW, ResH));
                    canvas.Save();
                }
            }

            BitmapData bitmapData1 = frame.LockBits(new Rectangle(0, 0,
                                     frame.Width, frame.Height),
                                     ImageLockMode.ReadWrite,
                                     PixelFormat.Format32bppArgb);

            BitmapData bitmapDataCanvas = _canvas.LockBits(new Rectangle(0, 0,
                                     _canvas.Width, _canvas.Height),
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
            frame.UnlockBits(bitmapData1);
            _canvas.UnlockBits(bitmapDataCanvas);

            _canvas.Dispose();
            _canvas = null;
        }

        public override bool CanRun(TimeManager timeManager)
        {
            return true;
        }

        public override Event Clone()
        {
            var newEvent = new BaseCanvas();
            newEvent.StartTime = StartTime;
            newEvent.EndTime = EndTime;

            return newEvent;
        }
    }
}
