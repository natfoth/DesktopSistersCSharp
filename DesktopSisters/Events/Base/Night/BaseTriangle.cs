using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSisters;
using DesktopSistersCSharpForm.Utils;

namespace DesktopSistersCSharpForm.Events.Base.Night
{
    class BaseTriangle : Event
    {
        public override void SetTimes(TimeManager timeManager)
        {
            StartTime = timeManager.SunSet;
            EndTime = timeManager.SunRise;
        }

        public override List<EventTags> Tags => new List<EventTags> { EventTags.SkyEvent };

        public override bool AllowDuplicateTags()
        {
            return true;
        }

        public override double Chance()
        {
            return 100;
        }

        public override TimeSpan Length()
        {
            return TimeSpan.FromMinutes(30);
        }

        public override int ZIndex()
        {
            return 1;
        }

        private Image _triangle = null;
        public override void Draw(Bitmap frame, TimeManager timeManager)
        {
            using (var g = Graphics.FromImage(frame))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                if (_triangle == null)
                {
                    _triangle = ImageController.LoadNightImage("Triangle.png");
                }

                g.DrawImageUnscaled(_triangle, (int)(ResW - ((double)(1.0 - timeManager.NightRatio) * (double)ResW * 0.5)) - 400, 50);


                _triangle.Dispose();
                _triangle = null;


                g.Save();
            }
        }

        public override bool CanRun(TimeManager timeManager)
        {
            return timeManager.IsNightTime;
        }

        public override Event Clone()
        {
            var newEvent = new BaseTriangle();
            newEvent.StartTime = StartTime;
            newEvent.EndTime = EndTime;

            return newEvent;
        }
    }
}
