﻿using System;
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
    internal class Luna_Statue : Event
    {
        public override double Chance()
        {
            return 1;
        }

        public override List<EventTags> Tags => new List<EventTags> { EventTags.Luna, EventTags.Alicorn };

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


                var twilightImage = ImageController.LoadEventImage("Luna/luna_statue.png");

                var imageHeight = Math.Min(twilightImage.Height, ResH/4);
                var ratio = (double) imageHeight/twilightImage.Height;
                var imageWidth = (int) (twilightImage.Width*ratio);

                // var twilightSparkle = new SceneObject("twilight.png", new Rectangle(ResW - imageWidth - 10, ResH - imageHeight - 10, imageWidth, imageHeight), 0);

                g.DrawImage(twilightImage,
                    new Rectangle((int) (ResW - (ResW * 0.1) - imageWidth), ResH - imageHeight - 10, imageWidth, imageHeight));

                g.Save();
            }
        }

        public override bool CanRun(TimeManager timeManager)
        {
            return timeManager.IsNightTime;
        }

        public override Event Clone()
        {
            var newEvent = new Luna_Statue();
            newEvent.StartTime = StartTime;
            newEvent.EndTime = EndTime;

            return newEvent;
        }
    }
}
