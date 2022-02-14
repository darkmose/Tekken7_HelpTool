using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class EventsAgregator 
{
    public static void Subscribe<TEvent>(System.Action<object, TEvent> eventHandler) 
    {
        EventHelper<TEvent>.Event += eventHandler;
    }
    public static void Unsubscribe<TEvent>(System.Action<object, TEvent> eventHandler) 
    {
        EventHelper<TEvent>.Event -= eventHandler;
    }

    public static void Post<TEvent>(object sender, TEvent eventData) 
    {
        EventHelper<TEvent>.Post(sender, eventData);
    }
    
    private static class EventHelper <TEvent>
    {
        public static event System.Action<object, TEvent> Event;

        public static void Post(object sender, TEvent eventData) 
        {
            Event?.Invoke(sender, eventData);
	    }
    
    }
}
