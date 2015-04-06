using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DesktopSisters;
using DesktopSistersCSharpForm.Utils;

namespace DesktopSistersCSharpForm.Events.Dynamic
{
    public class LunaDayChariot : Event
    {
        public override double Chance()
        {
            return 0;
        }

        public override TimeSpan Length()
        {
            return TimeSpan.FromMinutes(30);
        }

        public override int ZIndex()
        {
            return 4;
        }

        public override void Draw(Bitmap frame, TimeManager timeManager)
        {
            using (var g = Graphics.FromImage(frame))
            {
                var timeRatio = Ratio(timeManager.DateTime);

                var lunaImage = ImageController.LoadEventImage("lunaChariot.png");

                var imageHeight = Math.Min(lunaImage.Height, _resH / 4);
                var ratio = (double)imageHeight / lunaImage.Height;
                var imageWidth = (int)(lunaImage.Width * ratio);

                // var twilightSparkle = new SceneObject("twilight.png", new Rectangle(ResW - imageWidth - 10, ResH - imageHeight - 10, imageWidth, imageHeight), 0);

                var xLocation = _resW - (_resW* timeRatio);

                g.DrawImage(lunaImage, new Rectangle((int) xLocation, imageHeight + 30, imageWidth, imageHeight));

                g.Save();

                lunaImage.Dispose();
            }
        }

        public override bool CanRun(TimeManager timeManager)
        {
            return true;
        }

        public override Event Clone()
        {
            var newEvent = new LunaDayChariot();
            newEvent.StartTime = StartTime;
            newEvent.EndTime = EndTime;

            return newEvent;
        }
    }
}
