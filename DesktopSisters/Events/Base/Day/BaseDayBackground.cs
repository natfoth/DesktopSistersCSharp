using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSisters;
using DesktopSistersCSharpForm.Events.Dynamic;
using DesktopSistersCSharpForm.Utils;

namespace DesktopSistersCSharpForm.Events.Base
{
    class BaseDayBackground : Event
    {
        public override void SetTimes(TimeManager timeManager)
        {
            StartTime = timeManager.DateTime;
            EndTime = StartTime + Length();
        }

        public override List<EventTags> Tags => new List<EventTags> { EventTags.Background };

        public override bool CanBeOverRidden => true;
        public override double Chance()
        {
            return 100;
        }

        public override TimeSpan Length()
        {
            return TimeSpan.FromMinutes(Sisters.UpdateTime);
        }


        public override int ZIndex()
        {
            return 0;
        }

        public override void Draw(Bitmap frame, TimeManager timeManager)
        {
            var innerCircle = 1000.0*ResW*1.0/1680.0;
            //INNER_CIRCLE = 1000;
            var outerCircle = 1000.0*ResW*1.0/1680.0;
            var SunX = timeManager.SunX;
            var SunY = timeManager.SunY;

            Color startColor = Color.FromArgb(255, 243, 102);
            Color innerColor = Color.FromArgb(255, 166, 124);
            Color outerColor = Color.FromArgb(255, 200, 178);
            Color rayColor = Color.FromArgb(255, 255, 159);


            byte[] inner_color = new[] {innerColor.B, innerColor.G, innerColor.R, innerColor.A};
            byte[] start_color = new[] {startColor.B, startColor.G, startColor.R, startColor.A};


            BitmapData bitmapData1 = frame.LockBits(new Rectangle(0, 0,
                frame.Width, frame.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            byte bitsPerPixel = 4;

            unsafe
            {
                byte* scan0 = (byte*) bitmapData1.Scan0.ToPointer();

                Parallel.For(0, bitmapData1.Height, y =>
                {
                    Parallel.For(0, bitmapData1.Width, x =>
                    {
                        var imagePointer1 = scan0 + y*bitmapData1.Stride + x*bitsPerPixel;

                        var dist = Math.Sqrt((SunX - x)*(SunX - x) + (SunY - y)*(SunY - y));

                        if (dist < innerCircle)
                        {
                            var alphaToUse = dist/innerCircle;
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

            } //end unsafe

            frame.UnlockBits(bitmapData1);
        }

        public override bool CanRun(TimeManager timeManager)
        {
            return timeManager.IsDayTime;
        }

        public override Event Clone()
        {
            var newEvent = new BaseDayBackground();
            newEvent.StartTime = StartTime;
            newEvent.EndTime = EndTime;

            return newEvent;
        }
    }
}
