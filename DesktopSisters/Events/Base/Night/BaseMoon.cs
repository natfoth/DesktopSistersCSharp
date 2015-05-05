using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSisters;
using DesktopSistersCSharpForm.Events.Base.Day;
using DesktopSistersCSharpForm.Utils;

namespace DesktopSistersCSharpForm.Events.Base.Night
{
    class BaseMoon : Event
    {
        public override void SetTimes(TimeManager timeManager)
        {
            StartTime = timeManager.SunSet;
            EndTime = timeManager.SunRise;
        }

        public override List<EventTags> Tags => new List<EventTags> { EventTags.Moon };

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
            return 3;
        }

        private Image _moon = null;
        public override void Draw(Bitmap frame, TimeManager timeManager)
        {
            using (var g = Graphics.FromImage(frame))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                if (_moon == null)
                {
                    _moon = ImageController.LoadNightImage("Moon.png");
                }


                var moonX = timeManager.MoonX - (double)_moon.Width / 2.0;
                var moonY = timeManager.MoonY - (double)_moon.Height / 2.0;

                g.DrawImageUnscaled(_moon, (int)moonX, (int)moonY);


                _moon.Dispose();
                _moon = null;


                g.Save();
            }
        }

        public override bool CanRun(TimeManager timeManager)
        {
            return timeManager.IsNightTime;
        }

        public override Event Clone()
        {
            var newEvent = new BaseMoon();
            newEvent.StartTime = StartTime;
            newEvent.EndTime = EndTime;

            return newEvent;
        }
    }
}
