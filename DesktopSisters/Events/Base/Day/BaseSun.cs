using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSisters;
using DesktopSistersCSharpForm.Utils;

namespace DesktopSistersCSharpForm.Events.Base.Day
{
    class BaseSun : Event
    {
        public override void SetTimes(TimeManager timeManager)
        {
            StartTime = timeManager.DateTime;
            EndTime = StartTime + Length();
        }

        public override List<EventTags> Tags => new List<EventTags> { EventTags.Sun };

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
            return 2;
        }

        private Image _sun = null;
        public override void Draw(Bitmap frame, TimeManager timeManager)
        {
            using (var g = Graphics.FromImage(frame))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                if (_sun == null)
                {
                    _sun = ImageController.LoadDayImage("Sun.png");
                }

                var sunHeight = Math.Min(_sun.Height, ResH / 3.4);
                var ratio = (double)sunHeight / _sun.Height;
                var sunWidth = (int)(_sun.Width * ratio);

                var sunX = timeManager.SunX - (double)sunWidth / 2.0;
                var sunY = timeManager.SunY - (double)sunHeight / 2.0;

                g.DrawImage(_sun, new Rectangle((int)sunX, (int)sunY, sunWidth, (int)sunHeight));


                _sun.Dispose();
                _sun = null;
                

                g.Save();
            }
        }

        public override bool CanRun(TimeManager timeManager)
        {
            return timeManager.IsDayTime;
        }

        public override Event Clone()
        {
            var newEvent = new BaseSun();
            newEvent.StartTime = StartTime;
            newEvent.EndTime = EndTime;

            return newEvent;
        }
    }
}
