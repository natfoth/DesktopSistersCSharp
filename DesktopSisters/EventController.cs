using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DesktopSisters;
using DesktopSistersCSharpForm.Events.Dynamic;
using DesktopSistersCSharpForm.Utils;

namespace DesktopSistersCSharpForm
{
    public abstract class Event
    {
        public DateTime StartTime = DateTime.Now;
        public DateTime EndTime = DateTime.Now;

        public abstract double Chance(); // between 0 - 100

        public abstract TimeSpan Length();

        public virtual bool DrawDayBackground() { return false; }

        public virtual bool DrawNightBackground() { return false; }

        public virtual void DrawDayForeground(Graphics g, TimeManager timeManager) { }

        public virtual void DrawNightForeground(Graphics g, TimeManager timeManager) { }

        public abstract bool CanRun(DateTime time);

        public double Ratio(DateTime time)
        {

            var currentTimeDec = time.ToDouble();
            var startTimeDec = StartTime.ToDouble();
            var stopTimeDec = EndTime.ToDouble();

            double ratio = (currentTimeDec - startTimeDec) / (stopTimeDec - startTimeDec);

            return ratio;

        }

        public virtual Event Clone() { return null; }

        /*public Event Clone()
        {
            var newEvent = new Event();
            newEvent.StartTime = StartTime;
            newEvent.EndTime = EndTime;

            return newEvent;
        }*/

        public override string ToString()
        {
            return String.Format("{0} - Start Time {1} : End Time {2}", GetType().Name, StartTime.ToString("t"), EndTime.ToString("t"));
        }
    }

    public class EventController
    {
        private List<Event> _dynamicEvents = new List<Event>();

        public List<Event> ActiveDynamicEvents = new List<Event>();

        private DesktopSistersRandom _randomGenerator;

        public EventController()
        {
            _dynamicEvents.Add(new TwilightRandomSpawn());

            _randomGenerator = new DesktopSistersRandom();

            GenerateRandomEvents();
        }

        public void RenderDayForgrounds(Graphics g, TimeManager timeManager)
        {
            var events = EventsForTime(timeManager.DateTime);

            foreach (var @event in events)
            {
                @event.DrawDayForeground(g, timeManager);
            }
        }

        public void RenderNightForgrounds(Graphics g, TimeManager timeManager)
        {
            var events = EventsForTime(timeManager.DateTime);

            foreach (var @event in events)
            {
                @event.DrawNightForeground(g, timeManager);
            }
        }

        private void GenerateRandomEvents()
        {
            var time = DateTime.Parse("0:00 am");
            const int updateTime = 5; // in minutes

            for (int i = 0; i < 288; i++) // 
            {
                if (EventsForTime(time).Count != 0)
                {
                    time = time.AddMinutes(updateTime);
                    continue;
                }

                

                foreach (var @event in GetRandomEvent())
                {
                    if(!@event.CanRun(time))
                        continue;

                    var newEvent = @event.Clone();

                    newEvent.StartTime = time;
                    newEvent.EndTime = time.Add(newEvent.Length());

                    ActiveDynamicEvents.Add(newEvent);
                }

                time = time.AddMinutes(updateTime);
            }

            int bob = 1;
        }

        private List<Event> GetRandomEvent()
        {
            var list = new List<Event>();


            var rand = _randomGenerator.Next(0, 100000) / 1000.0;

            foreach (var dynamicEvent in _dynamicEvents)
            {
                if (rand < dynamicEvent.Chance())
                    list.Add(dynamicEvent);

                rand = _randomGenerator.Next(0, 100000) / 1000.0;
            }


            return list;
        }

        private List<Event> EventsForTime(DateTime time)
        {
            var list = new List<Event>();

            foreach (var dynamicEvent in ActiveDynamicEvents)
            {
                if (time.Ticks > dynamicEvent.StartTime.Ticks && time.Ticks < dynamicEvent.EndTime.Ticks)
                {
                    list.Add(dynamicEvent);
                }
            }

            return list;
        } 

    }
}
