// Allows for multiple events to be added to an object in an array and 
// later called through other events,
// Either by the listener's method name, or by the index in case multiple events
// Are calling the same method
// Was originally intended to be used for animation events, though I realize now
// that it could potentially be used elsewhere

// Written by George Castle '22

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WSoft.VFX
{
    public class AnimationEvents : MonoBehaviour
    {
        public struct eventInvoker
        {
            public UnityEvent myEvent;
            public string name;
            public void invokeEvent()
            {
                myEvent.Invoke();
            }
        }
        public List<eventInvoker> eventInvokers = new List<eventInvoker>();
        public UnityEvent[] myEvents;
        private void Start()
        {
            for (int i = 0; i < myEvents.Length; i++)
            {
                eventInvoker myInvoker = new eventInvoker();
                myInvoker.name = myEvents[i].GetPersistentMethodName(0);
                myInvoker.myEvent = myEvents[i];
                eventInvokers.Add(myInvoker);
            }
        }
        public void invokeEventByName(string name)
        {
            for (int i = 0; i < eventInvokers.Count; i++)
            {
                if (eventInvokers[i].name == name)
                {
                    eventInvokers[i].invokeEvent();
                }
            }
        }
        public void invokeEventByIndex(int index)
        {
            eventInvokers[index].invokeEvent();
        }
    }
}