/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: Event.cs
			// Describle: The processor's event unit.
			// Created By:  hsu
			// Date&Time:  2016/1/19 10:03:15
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000
{
    public class Event
    {
        public int EventID;
        public Event(int evtID) { EventID = evtID; }
    }
    public class EventEX<T> : Event
    {
        public T Param;
        public EventEX(int evtID, T param) : base(evtID)
        {
            Param = param;
        }
    }

    public class EventHandlerQueue
    {
        private Dictionary<int, List<EventProcessorHandler>> m_Handlers;
        private EventProcessor m_Processor;

        public EventHandlerQueue(EventProcessor processor)
        {
            m_Handlers = new Dictionary<int, List<EventProcessorHandler>>();
            m_Processor = processor;
        }

        public void Add(int eventID, EventProcessorHandler handler)
        {
            if (handler == null)
            {
                return;
            }
            if (m_Handlers == null)
            {
                m_Handlers = new Dictionary<int, List<EventProcessorHandler>>();
            }
            List<EventProcessorHandler> handlers = null;
            if (m_Handlers.TryGetValue(eventID, out handlers) == false)
            {
                handlers = new List<EventProcessorHandler>();
                handlers.Add(handler);
                m_Handlers.Add(eventID, handlers);
                ApplicationEventProcessor.GetInstance().Register(eventID, handler);
            }
            else
            {
                if (handlers.Contains(handler) == false)
                {
                    handlers.Add(handler);
                    ApplicationEventProcessor.GetInstance().Register(eventID, handler);
                }
            }
        }

        public void Remove(int eventID, EventProcessorHandler handler)
        {
            if (handler == null)
            {
                return;
            }
            if (m_Handlers == null || m_Handlers.Count == 0)
            {
                return;
            }
            List<EventProcessorHandler> tmpDels = null;
            if (m_Handlers.TryGetValue(eventID, out tmpDels))
            {
                tmpDels.Clear();
                m_Handlers.Remove(eventID);
                ApplicationEventProcessor.GetInstance().Unregister(eventID, handler);
            }
        }

        public void RemoveAll()
        {
            if (m_Handlers == null || m_Handlers.Count == 0)
            {
                return;
            }
            Dictionary<int, List<EventProcessorHandler>>.Enumerator it = m_Handlers.GetEnumerator();
            for (int i = 0; i < m_Handlers.Count; i++)
            {
                it.MoveNext();
                KeyValuePair<int, List<EventProcessorHandler>> kvp = it.Current;
                if (kvp.Value == null || kvp.Value.Count == 0)
                {
                    continue;
                }
                for (int j = 0; j < kvp.Value.Count; j++)
                {
                    ApplicationEventProcessor.GetInstance().Unregister(kvp.Key, kvp.Value[j]);
                }
            }
            m_Handlers.Clear();
        }
    }

}
