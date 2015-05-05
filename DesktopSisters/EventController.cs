using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DesktopSisters;
using DesktopSisters.Utils;
using DesktopSistersCSharpForm.Events.Base;
using DesktopSistersCSharpForm.Events.Base.Day;
using DesktopSistersCSharpForm.Events.Base.Night;
using DesktopSistersCSharpForm.Events.Dynamic;
using DesktopSistersCSharpForm.Utils;

namespace DesktopSistersCSharpForm
{
    public class SceneContainer
    {
        public SceneContainer(DateTime time, List<Event> listOfEventsForTime)
        {
            Time = time;
            Events = listOfEventsForTime;
        }

        public DateTime Time { get; set; }
        public List<Event> Events { get; set; }

        public override string ToString()
        {
            return String.Format("{0} - Event Count : {1}", Time.ToString("t"), Events.Count);
        }
    }

    public class EventController
    {
        private List<Event> _events = new List<Event>();

        public List<SceneContainer> ActiveEvents = new List<SceneContainer>();

        private DesktopSistersRandom _randomGenerator;

        public EventController(Configuration config)
        {
            //base Events
            _events.Add(new BaseDayBackground());
            _events.Add(new BaseDayClouds());
            _events.Add(new BaseSun());
            _events.Add(new BaseDayLandscape());
            _events.Add(new BaseCelestia());

            _events.Add(new BaseNightBackground());
            _events.Add(new BaseStars());
            _events.Add(new BaseNightClouds());
            _events.Add(new BaseFallingStar());
            _events.Add(new BaseTriangle());
            _events.Add(new BaseMoon());
            _events.Add(new BaseNightLandscape());
            _events.Add(new BaseLuna());




            _events.Add(new BaseCanvas());

            //end base events

            //Luna Events
            _events.Add(new LunaDayChariot());
            _events.Add(new Luna_Statue());

            //End Luna Events

            _events.Add(new TwilightRandomSpawn());


            _randomGenerator = new DesktopSistersRandom();

            GenerateEvents(config);
        }

        public void RenderEvents(Bitmap scene, TimeManager timeManager)
        {
            Benchmark.Start();

            var events = EventsForTime(timeManager).OrderBy(ntf => ntf.ZIndex()).ToList();

            foreach (var @event in events)
            {
                @event.Draw(scene, timeManager);
            }

            Benchmark.End();
            var miliseconds = Benchmark.GetMiliseconds();
            int bob = 1;
        }

        private void GenerateEvents(Configuration config)
        {
            var time = DateTime.Parse("0:00 am");
            const int updateTime = Sisters.UpdateTime; // in minutes

            for (int i = 0; i < 288; i++) // 
            {
                if (time.Hour >= 6)
                {
                    int asdf = 1;
                }

                var timeManager = new TimeManager(time, config.Latitude, config.Longitude);
                timeManager.Update();

                var eventsForTime = EventsForTime(timeManager);

                var newEventsForScene = eventsForTime;


                foreach (var @event in GetRandomEvent(timeManager))
                {
                    var newEvent = @event.Clone();
                    newEvent.Init(timeManager);
                    newEvent.SetTimes(timeManager);



                    if (eventsForTime.Count(ntf => ntf.Equals(@event)) >= newEvent.MaxEvents())
                    {
                        newEvent.Dispose();
                        continue;
                    }

                    if (!newEvent.AllowDuplicateTags())
                    {
                        var listOfSameNonOverridableTags =
                            newEventsForScene.Where(
                                x => !x.CanBeOverRidden && x.Tags.Any(y => newEvent.Tags.Contains(y))).ToList();

                        if (listOfSameNonOverridableTags.Count > 0)
                            continue;


                        var listOfSameTag =
                            newEventsForScene.Where(x => x.CanBeOverRidden && x.Tags.Any(y => newEvent.Tags.Contains(y)))
                                .ToList();

                        foreach (var sameEvent in listOfSameTag)
                        {
                            newEventsForScene.Remove(sameEvent);
                        }
                    }

                    newEventsForScene.Add(newEvent);

                }

                var newEventContainer = new SceneContainer(time, newEventsForScene);

                ActiveEvents.Add(newEventContainer);

                time = time.AddMinutes(updateTime);
            }

            int end = 1;
        }

        private List<Event> GetRandomEvent(TimeManager time)
        {
            var list = new List<Event>();


            var rand = _randomGenerator.Next(0, 100000)/1000.0;

            foreach (var dynamicEvent in _events)
            {
                if (!dynamicEvent.CanRun(time))
                    continue;

                if (rand <= dynamicEvent.Chance())
                    list.Add(dynamicEvent);

                rand = _randomGenerator.Next(0, 100000)/1000.0;
            }


            return list;
        }

        private List<Event> EventsForTime(TimeManager timeManager)
        {
            var list = new List<Event>();

            var hour = timeManager.DateTime.Hour;
            var minute = (int) Math.Round(timeManager.DateTime.Minute/5.0)*5;

            /*var eventForTime = ActiveEvents.FirstOrDefault(ev => ev.Time.Hour == hour && ev.Time.Minute == minute);

            if (eventForTime == null)
                return list;*/

            foreach (var sceneContainer in ActiveEvents)
            {
                foreach (var dynamicEvent in sceneContainer.Events)
                {
                    if(list.Contains(dynamicEvent))
                        continue;

                    if (timeManager.IsNightTime)
                    {
                        var currentTimeDec = timeManager.DateTime.ToDouble();
                        var startEventTimeDec = dynamicEvent.StartTime.ToDouble();
                        var endEventTimeDec = dynamicEvent.EndTime.ToDouble();

                        if (currentTimeDec >= 12)
                        {
                            if (startEventTimeDec < endEventTimeDec)
                            {
                                if (currentTimeDec > startEventTimeDec && currentTimeDec < endEventTimeDec)
                                    list.Add(dynamicEvent);
                            }
                            else
                            {
                                var totalMins = Math.Abs((dynamicEvent.EndTime - dynamicEvent.StartTime).TotalMinutes);
                                var eventLength = totalMins/60;

                                var startPosition = endEventTimeDec - eventLength;

                                var currentPosition = 0 - (24 - currentTimeDec);

                                if (currentPosition > startPosition && currentPosition < endEventTimeDec)
                                    list.Add(dynamicEvent);
                            }
                        }
                        else
                        {
                            if (startEventTimeDec < endEventTimeDec)
                            {
                                if (currentTimeDec > startEventTimeDec && currentTimeDec < endEventTimeDec)
                                    list.Add(dynamicEvent);
                            }
                            else
                            {
                                var startPosition = endEventTimeDec - 12;

                                if (currentTimeDec > startPosition && currentTimeDec < endEventTimeDec)
                                    list.Add(dynamicEvent);
                            }
                        }
                    }
                    else
                    {
                        if (timeManager.DateTime.Ticks > dynamicEvent.StartTime.Ticks &&
                            timeManager.DateTime.Ticks < dynamicEvent.EndTime.Ticks)
                        {
                            list.Add(dynamicEvent);
                        }
                    }


                }
            }

            return list;
        }

    }
}
