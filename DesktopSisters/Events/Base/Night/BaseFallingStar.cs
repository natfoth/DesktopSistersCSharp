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
    class BaseFallingStar : Event
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
            return 1;
        }

        private Image _fallingStar = null;
        public override void Draw(Bitmap frame, TimeManager timeManager)
        {
            using (var g = Graphics.FromImage(frame))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                if (_fallingStar == null)
                {
                    _fallingStar = ImageController.LoadNightImage("FallingStar.png");
                }

                g.DrawImageUnscaled(_fallingStar, (int)((double)timeManager.NightRatio * (double)_resW) + 500, 200);


                _fallingStar.Dispose();
                _fallingStar = null;


                g.Save();
            }
        }

        public override bool CanRun(TimeManager timeManager)
        {
            return timeManager.IsNightTime;
        }

        public override Event Clone()
        {
            var newEvent = new BaseFallingStar();
            newEvent.StartTime = StartTime;
            newEvent.EndTime = EndTime;

            return newEvent;
        }

    }
}