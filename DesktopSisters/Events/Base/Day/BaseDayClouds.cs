using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopSisters;
using DesktopSistersCSharpForm.Utils;

namespace DesktopSistersCSharpForm.Events.Base.Day
{
    class BaseDayClouds : Event
    {
        public override void SetTimes(TimeManager timeManager)
        {
            StartTime = timeManager.DateTime;
            EndTime = StartTime + Length();
        }

        public override List<EventTags> Tags => new List<EventTags> { EventTags.Clouds };

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
            return 5;
        }

        private Image[] _clouds = null;
        public override void Draw(Bitmap frame, TimeManager timeManager)
        {
            using (var g = Graphics.FromImage(frame))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                if (_clouds == null)
                {
                    _clouds = new Image[3] { ImageController.LoadDayImage("DayCloud1.png"), ImageController.LoadDayImage("DayCloud2.png"), ImageController.LoadDayImage("DayCloud3.png") };
                }

                #region Clouds
                g.DrawImageUnscaled(_clouds[2], (int)(ResW - ((double)(1.0 - timeManager.DayRatio) * (double)ResW * 0.5)), -20);
                g.DrawImageUnscaled(_clouds[1], (int)(ResW - ((double)(1.0 - timeManager.DayRatio) * (double)ResW * 0.5)) - 220, -20);
                g.DrawImageUnscaled(_clouds[0], (int)(ResW - ((double)(1.0 - timeManager.DayRatio) * (double)ResW * 0.5)) - 400, -20);
                #endregion

                foreach (var cloud in _clouds)
                {
                    cloud.Dispose();
                    _clouds = null;
                }

                g.Save();
            }
        }

        public override bool CanRun(TimeManager timeManager)
        {
            return timeManager.IsDayTime;
        }

        public override Event Clone()
        {
            var newEvent = new BaseDayClouds();
            newEvent.StartTime = StartTime;
            newEvent.EndTime = EndTime;

            return newEvent;
        }
    }
}
