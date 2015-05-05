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
            return 3;
        }

        public override List<EventTags> Tags => new List<EventTags> { EventTags.Luna, EventTags.Alicorn };

        public override TimeSpan Length()
        {
            return TimeSpan.FromMinutes(5);
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

                var lunaImage = ImageController.LoadEventImage("luna/lunaChariot.png");

                var imageHeight = Math.Min(lunaImage.Height, ResH / 4);
                var ratio = (double)imageHeight / lunaImage.Height;
                var imageWidth = (int)(lunaImage.Width * ratio);

                // var twilightSparkle = new SceneObject("twilight.png", new Rectangle(ResW - imageWidth - 10, ResH - imageHeight - 10, imageWidth, imageHeight), 0);

                var xLocation = ResW - (ResW* timeRatio);

                g.DrawImage(lunaImage, new Rectangle((int) xLocation, imageHeight + 30, imageWidth, imageHeight));

                g.Save();

                lunaImage.Dispose();
            }
        }

        public override bool CanRun(TimeManager timeManager)
        {
            return timeManager.IsDayTime;
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
