using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DesktopSisters;

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

        public override void DrawDayForeground(Graphics g, TimeManager timeManager)
        {
            
        }

        public override void DrawNightForeground(Graphics g, TimeManager timeManager)
        {
            Rectangle resolution = Screen.PrimaryScreen.Bounds;

            var ResW = resolution.Width;
            var ResH = resolution.Height;

            var twilightImage = ImageController.LoadEventImage("twilight.png");

            var imageHeight = Math.Min(twilightImage.Height, ResH / 4);
            var ratio = (double) imageHeight/twilightImage.Height;
            var imageWidth = (int)(twilightImage.Width*ratio);



            g.DrawImage(twilightImage, new Rectangle(ResW - imageWidth - 10, ResH - imageHeight - 10, imageWidth, imageHeight));
        }

        public override bool CanRun(DateTime time)
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
