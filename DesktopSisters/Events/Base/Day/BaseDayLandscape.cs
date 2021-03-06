﻿using System;
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
    class BaseDayLandscape : Event
    {
        public override void SetTimes(TimeManager timeManager)
        {
            StartTime = timeManager.DateTime;
            EndTime = StartTime + Length();
        }

        public override List<EventTags> Tags => new List<EventTags> { EventTags.Landscape };

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
            return 15;
        }

        private Image _landscape = null;
        public override void Draw(Bitmap frame, TimeManager timeManager)
        {
            using (var g = Graphics.FromImage(frame))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                if (_landscape == null)
                {
                    _landscape = ImageController.LoadDayImage("Landscape.png");
                }

                g.DrawImage(_landscape, new Rectangle(0, ResH - _landscape.Height, ((int)((double)ResW)), _landscape.Height));


                _landscape.Dispose();
                _landscape = null;


                g.Save();
            }
        }

        public override bool CanRun(TimeManager timeManager)
        {
            return timeManager.IsDayTime;
        }

        public override Event Clone()
        {
            var newEvent = new BaseDayLandscape();
            newEvent.StartTime = StartTime;
            newEvent.EndTime = EndTime;

            return newEvent;
        }
    }
}
