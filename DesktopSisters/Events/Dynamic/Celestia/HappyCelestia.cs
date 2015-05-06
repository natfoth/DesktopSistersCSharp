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
    class HappyCelestia : Event
    {
        public int SelectedCelestia { get; set; }

        public override void Init(TimeManager timeManager)
        {
            var r = new DesktopSistersRandom();

            SelectedCelestia = r.Next(1, 6);
        }

        public override void SetTimes(TimeManager timeManager)
        {
            StartTime = timeManager.DateTime;
            EndTime = StartTime + Length();
        }

        public override List<EventTags> Tags => new List<EventTags> { EventTags.Celestia };

        public override bool CanBeOverRidden => true;

        public override double Chance()
        {
            return 30;
        }

        public override TimeSpan Length()
        {
            return TimeSpan.FromMinutes(30);
        }

        public override int ZIndex()
        {
            return 25;
        }

        private Image _celestia = null;
        public override void Draw(Bitmap frame, TimeManager timeManager)
        {
            using (var g = Graphics.FromImage(frame))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                if (_celestia == null)
                {
                    _celestia = ImageController.LoadEventImage(String.Format("Celestia/celestia_happy_{0:00}.png", SelectedCelestia));
                }

                var celestiaHeight = Math.Min(_celestia.Height, 450);
                var ratio = (double)celestiaHeight / _celestia.Height;
                var celestWidth = (int)(_celestia.Width * ratio);

                g.DrawImage(_celestia, new Rectangle(20, ResH - celestiaHeight - 10, celestWidth, celestiaHeight));


                _celestia.Dispose();
                _celestia = null;


                g.Save();
            }
        }

        public override bool CanRun(TimeManager timeManager)
        {
            return timeManager.IsDayTime;
        }

        public override Event Clone()
        {
            var newEvent = new HappyCelestia();
            newEvent.StartTime = StartTime;
            newEvent.EndTime = EndTime;
            newEvent.SelectedCelestia = SelectedCelestia;

            return newEvent;
        }
    }
}
