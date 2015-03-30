using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DesktopSistersCSharpForm.Events.Dynamic;

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

        public virtual void DrawDayForeground(Graphics g) { }

        public virtual void DrawNightForeground(Graphics g) { }

        public abstract bool CanRun(DateTime time);

        public virtual Event Clone() { return null; }

        /*public Event Clone()
        {
            var newEvent = new Event();
            newEvent.StartTime = StartTime;
            newEvent.EndTime = EndTime;

            return newEvent;
        }*/
    }

    public class EventController
    {
        private List<Event> _dynamicEvents = new List<Event>();

        public List<Event> ActiveDynamicEvents = new List<Event>(); 

        public EventController()
        {
            _dynamicEvents.Add(new TwilightRandomSpawn());

            GenerateRandomEvents();
        }

        public void RenderDayForgrounds(Graphics g, DateTime time)
        {
            var events = EventsForTime(time);

            foreach (var @event in events)
            {
                @event.DrawDayForeground(g);
            }
        }

        public void RenderNightForgrounds(Graphics g, DateTime time)
        {
            var events = EventsForTime(time);

            foreach (var @event in events)
            {
                @event.DrawNightForeground(g);
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
                    var newEvent = @event.Clone();

                    newEvent.StartTime = time;
                    newEvent.EndTime = time.Add(newEvent.Length());

                    ActiveDynamicEvents.Add(newEvent);
                }

                time = time.AddMinutes(updateTime);
            }

            int bob = 1;
        }

        private Random random = new Random();
        private List<Event> GetRandomEvent()
        {
            var list = new List<Event>();

            var rand = (double)random.Next(0, 100000) / 1000.0;

            foreach (var dynamicEvent in _dynamicEvents)
            {
                if (rand < dynamicEvent.Chance())
                    list.Add(dynamicEvent);

                rand = random.Next(0, 100);
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
