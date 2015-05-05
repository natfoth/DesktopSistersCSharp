using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSisters;
using DesktopSistersCSharpForm.Utils;

namespace DesktopSistersCSharpForm.Events.Base.Night
{
    class BaseNightBackground : Event
    {
        public override void SetTimes(TimeManager timeManager)
        {
            StartTime = timeManager.DateTime;
            EndTime = StartTime + Length();
        }

        public override List<EventTags> Tags => new List<EventTags> { EventTags.Background };

        public override bool BaseEvent => true;

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
            var innerCircle = 500;

            var moonX = timeManager.MoonX;
            var moonY = timeManager.MoonY;

            Color HorizonColor = Color.FromArgb(77, 145, 157);
            Color NightColor = Color.FromArgb(114, 49, 187);
            Color BleedColor = Color.FromArgb(52, 50, 101);
            Color ShineColor = Color.FromArgb(64, 255, 255, 255);
            Color BackgroundColor = Color.FromArgb(80, 77, 157);

            byte[] backgroundColor = new[] {BackgroundColor.B, BackgroundColor.G, BackgroundColor.R, BackgroundColor.A};
            byte[] horizonColor = new[] {HorizonColor.B, HorizonColor.G, HorizonColor.R, HorizonColor.A};
            byte[] bleedColor = new[] {BleedColor.B, BleedColor.G, BleedColor.R, BleedColor.A};
            byte[] nightColor = new[] {NightColor.B, NightColor.G, NightColor.R, NightColor.A};

            if (timeManager.IsTwilight)
            {
                var item1 = timeManager.GetSunPos().Item1;
                backgroundColor = ImgProcLib.BlendColor(Color.FromArgb(204, 102, 102).ToByteArray(), backgroundColor,
                    (item1/-6));
                horizonColor = ImgProcLib.BlendColor(Color.FromArgb(255, 200, 178).ToByteArray(), horizonColor,
                    (item1/-6));
            }


            var bleedWidth = ResW*2/5;


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

                        var Dist = Math.Sqrt((moonX - x)*(moonX - x) + (moonY - y)*(moonY - y));

                        var Base = ImgProcLib.BlendColor(backgroundColor, horizonColor, (double) y/(double) ResH);
                        var Night = ImgProcLib.BlendColor(bleedColor, nightColor,
                            ((double) (Math.Max(x - ResW + bleedWidth, 0)))/(double) bleedWidth);
                        var BG = ImgProcLib.BlendColor(Base, Night, Night[2]/255.0f); // blends the value of (red / 255)

                        var color = BG;
                        if (Dist < innerCircle)
                        {
                            var alphaToUse = Dist/innerCircle;
                            alphaToUse = (1.0 - alphaToUse)*255.0;

                            color = ImgProcLib.BlendColor(BG, Color.FromArgb(255, 255, 255, 255).ToByteArray(),
                                (alphaToUse/255)*0.2);
                        }

                        imagePointer1[0] = color[0];
                        imagePointer1[1] = color[1];
                        imagePointer1[2] = color[2];
                        imagePointer1[3] = 255;
                    });
                });

            } //end unsafe
            frame.UnlockBits(bitmapData1);
        }

        public override bool CanRun(TimeManager timeManager)
        {
            return timeManager.IsNightTime;
        }

        public override Event Clone()
        {
            var newEvent = new BaseNightBackground();
            newEvent.StartTime = StartTime;
            newEvent.EndTime = EndTime;

            return newEvent;
        }
    }
}
