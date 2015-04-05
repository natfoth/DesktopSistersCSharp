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
    class BaseLuna : Event
    {
        public override void SetTimes(TimeManager timeManager)
        {
            StartTime = timeManager.SunSet;
            EndTime = timeManager.SunRise;
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
            return 25;
        }

        private Image _luna = null;
        public override void Draw(Bitmap frame, TimeManager timeManager)
        {
            using (var g = Graphics.FromImage(frame))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                if (_luna == null)
                {
                    _luna = ImageController.LoadNightImage("Luna.png");
                }

                g.DrawImage(_luna, new Rectangle(20, _resH - _luna.Height - 10, _luna.Width, _luna.Height));


                _luna.Dispose();
                _luna = null;


                g.Save();
            }
        }

        public override bool CanRun(TimeManager timeManager)
        {
            return timeManager.IsNightTime;
        }

        public override Event Clone()
        {
            var newEvent = new BaseLuna();
            newEvent.StartTime = StartTime;
            newEvent.EndTime = EndTime;

            return newEvent;
        }
    }
}
