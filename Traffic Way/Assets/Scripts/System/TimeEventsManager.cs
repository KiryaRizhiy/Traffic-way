using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeEventsManager : MonoBehaviour
{
    private static GameObject timerObject;
    private static TimeEventsManager timer
    {
        get
        {
            return timerObject.GetComponent<TimeEventsManager>();
        }
    }
    private static List<TimeEvent> events;
    public static void Initialize()
    {
        timerObject = new GameObject();
        timerObject.name = "Timer";
        DontDestroyOnLoad(timerObject);
        timerObject.AddComponent<TimeEventsManager>();
        events = new List<TimeEvent>();
    }

    public static void RegisterTimeEvent(string EventName, long OccurTimeTicks)
    {
        events.Add(new TimeEvent(EventName, OccurTimeTicks));
    }
    public static void RemoveTimeEvent(string Name, long OccurTimeTicks)
    {
        events.RemoveAll(x => x.name == Name && x.occurTimeTicks == OccurTimeTicks);
    }
    public static void UpdateTimeEvent(string EventName, long PreviousOccurTimeTicks, long NewOccurTimeTicks)
    {
        events.Find(x => x.name == EventName && x.occurTimeTicks == PreviousOccurTimeTicks).ChangeTimeTo(NewOccurTimeTicks);
    }

    void Update()
    {
        foreach (TimeEvent _e in events.FindAll(x=>!x.alreadyThrown))
        {
            if(DateTime.UtcNow.Ticks >= _e.occurTimeTicks)
                _e.Occured();
        }
    }
    private class TimeEvent
    {
        public string name
        { get; private set; }
        public long occurTimeTicks
        { get; private set; }
        public bool alreadyThrown
        { get; private set; }
        public TimeEvent(string Name, long OccurTimeTicks)
        {
            name = Name;
            occurTimeTicks = OccurTimeTicks;
            alreadyThrown = false;
        }
        public void ChangeTimeTo(long NewOccurTimeTicks)
        {
            occurTimeTicks = NewOccurTimeTicks;
        }
        public void Occured()
        {
            Engine.Events.TimeEventOccured(name);
            alreadyThrown = true;
        }
    }
}

