﻿using System;
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
    class BaseNightLandscape : Event
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
                    _landscape = ImageController.LoadNightImage("LandscapeNight.png");
                }

                g.DrawImage(_landscape, new Rectangle(0, _resH - _landscape.Height, _resW, _landscape.Height));

                _landscape.Dispose();
                _landscape = null;


                g.Save();
            }
        }

        public override bool CanRun(TimeManager timeManager)
        {
            return timeManager.IsNightTime;
        }

        public override Event Clone()
        {
            var newEvent = new BaseNightLandscape();
            newEvent.StartTime = StartTime;
            newEvent.EndTime = EndTime;

            return newEvent;
        }
    }
}