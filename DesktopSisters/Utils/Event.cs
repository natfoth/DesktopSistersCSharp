using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DesktopSisters;

namespace DesktopSistersCSharpForm.Utils
{
    public enum EventTags
    {
        //Filter
        Filter,

        //Background
        Background,
        Landscape,
        Sun,
        Moon,
        Stars,
        Clouds,
        SkyEvent,

        //Ponies
        Celestia,
        Luna,
        Twilight,
        Fluttershy,
        RainbowDash,
        Applejack,
        Rarity,
        PinkiePie,


        Alicorn,
    };

    public abstract class Event : IEquatable<Event>
    {
        public DateTime StartTime = DateTime.Now;
        public DateTime EndTime = DateTime.Now;

        protected readonly int ResW = Screen.PrimaryScreen.Bounds.Width;
        protected readonly int ResH = Screen.PrimaryScreen.Bounds.Height;

        public virtual void Init(TimeManager timeManager) { }

        public abstract double Chance(); // between 0 - 100

        public abstract List<EventTags> Tags { get; }

        public abstract TimeSpan Length();
        public abstract int ZIndex();
        public virtual bool IsAllDay() { return false; }

        public virtual bool AllowDuplicateTags() { return false;}

        public virtual bool CanBeOverRidden => false;

        public virtual void SetTimes(TimeManager timeManager)
        {
            StartTime = timeManager.DateTime;
            EndTime = timeManager.DateTime.Add(Length());

            if (IsAllDay())
            {
                StartTime = DateTime.Parse("0:00");
                EndTime = DateTime.Parse("23:59");
            }
        }

        public virtual bool DrawDayBackground() { return false; }

        public virtual void Draw(Bitmap frame, TimeManager timeManager) { }

        public abstract bool CanRun(TimeManager timeManager);

        public virtual int MaxEvents()
        {
            return 1;
        }

        public double Ratio(DateTime time)
        {
            var ratio = (double)(time.Ticks - StartTime.Ticks) / (double)(EndTime.Ticks - StartTime.Ticks);

            return ratio;
        }

        public virtual Event Clone() { return null; }


        public virtual void Dispose()
        {
            
        }

        public bool Equals(Event other)
        {
            var thisClass = ((object) this).GetType().Name;
            var otherClass = ((object)other).GetType().Name;

            // Would still want to check for null etc. first.
            return thisClass == otherClass/* && this.StartTime == other.StartTime &&
                   this.EndTime == other.EndTime*/;
        }

        public override string ToString()
        {
            return String.Format("{0} - Start Time {1} : End Time {2}", GetType().Name, StartTime.ToString("t"), EndTime.ToString("t"));
        }
    }
}
