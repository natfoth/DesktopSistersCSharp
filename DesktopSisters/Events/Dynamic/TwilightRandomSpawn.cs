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
    internal class TwilightRandomSpawn : Event
    {
        public override double Chance()
        {
            return 1;
        }

        public override TimeSpan Length()
        {
            return TimeSpan.FromMinutes(30);
        }

        public override int ZIndex()
        {
            return 25;
        }

        public override void Draw(Bitmap frame, TimeManager timeManager)
        {
            using (var g = Graphics.FromImage(frame))
            {
                var ratioTime = Ratio(timeManager.DateTime);


                var twilightImage = ImageController.LoadEventImage("twilight/twilight.png");

                var imageHeight = Math.Min(twilightImage.Height, _resH/4);
                var ratio = (double) imageHeight/twilightImage.Height;
                var imageWidth = (int) (twilightImage.Width*ratio);

                // var twilightSparkle = new SceneObject("twilight.png", new Rectangle(ResW - imageWidth - 10, ResH - imageHeight - 10, imageWidth, imageHeight), 0);

                g.DrawImage(twilightImage,
                    new Rectangle(_resW - imageWidth - 10, _resH - imageHeight - 10, imageWidth, imageHeight));

                g.Save();
            }
        }

        public override bool CanRun(TimeManager timeManager)
        {
            return true;
        }

        public override Event Clone()
        {
            var newEvent = new TwilightRandomSpawn();
            newEvent.StartTime = StartTime;
            newEvent.EndTime = EndTime;

            return newEvent;
        }
    }
}
