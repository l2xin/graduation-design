/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: EventProcessor.cs
			// Describle:  The game internal event processor
			// Created By:  hsu
			// Date&Time:  2016/1/19 10:03:15
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace Air2000
{
    public delegate void EventProcessorHandler(Event evt);

    public abstract class EventProcessor
    {
        private Dictionary<int, List<EventProcessorHandler>> m_RegisteredHandlers;
        private List<EventProcessorHandler> m_WillBeDeletedMsgHandlers;

        public EventProcessor()
        {
            m_RegisteredHandlers = new Dictionary<int, List<EventProcessorHandler>>();
            m_WillBeDeletedMsgHandlers = new List<EventProcessorHandler>();
        }

        /// <summary>
        /// Get all events' handler count.
        /// </summary>
        /// <returns></returns>
        public int GetAllHandlersCount()
        {
            return m_RegisteredHandlers.Count();
        }

        /// <summary>
        /// Get a specific event's handler count.
        /// </summary>
        /// <param name="eventID"></param>
        /// <returns></returns>
        public int GetHandlersCount(int eventID)
        {
            if (m_RegisteredHandlers == null || m_RegisteredHandlers.Count == 0)
            {
                return -1;
            }
            List<EventProcessorHandler> handlers = null;
            if (m_RegisteredHandlers.TryGetValue(eventID, out handlers))
            {
                return handlers.Count;
            }
            return 0;
        }

        /// <summary>
        /// Notify event to all registered handlers.
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="eventObj"></param>
        private void Dispatch(int eventID, Event eventObj)
        {
            List<EventProcessorHandler> handlers = null;
            if (m_RegisteredHandlers.TryGetValue(eventID, out handlers))
            {
                if (m_RegisteredHandlers.Count != 0 && handlers != null)
                {
                    for (int i = 0; i < m_WillBeDeletedMsgHandlers.Count; i++)
                    {
                        EventProcessorHandler temRem = m_WillBeDeletedMsgHandlers[i];
                        if (handlers.Contains(temRem))
                        {
                            handlers.Remove(temRem);
                            m_WillBeDeletedMsgHandlers.RemoveAt(i);
                            i--;
                        }
                    }
                }
                if (handlers != null && handlers.Count > 0)
                {
                    for (int i = 0; i < handlers.Count; i++)
                    {
                        EventProcessorHandler handler = handlers[i];
                        if (handler == null || handler.Target == null)
                        {
                            m_WillBeDeletedMsgHandlers.Add(handler);
                        }
                        else
                        {
                            handler(eventObj);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Add a handler.
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="handler"></param>
        private void AddHandler(int eventID, EventProcessorHandler handler)
        {
            if (handler == null)
            {
                return;
            }
            if (m_RegisteredHandlers == null)
            {
                m_RegisteredHandlers = new Dictionary<int, List<EventProcessorHandler>>();
            }
            List<EventProcessorHandler> handlers = null;
            if (m_RegisteredHandlers.TryGetValue((int)eventID, out handlers) == false)
            {
                handlers = new List<EventProcessorHandler>();
                m_RegisteredHandlers.Add(eventID, handlers);
            }
            else
            {
                if (m_RegisteredHandlers.Count != 0 && handlers != null)
                {
                    for (int i = 0; i < m_WillBeDeletedMsgHandlers.Count; i++)
                    {
                        EventProcessorHandler tempHandler = m_WillBeDeletedMsgHandlers[i];
                        if (handlers.Contains(tempHandler))
                        {
                            handlers.Remove(tempHandler);
                            m_WillBeDeletedMsgHandlers.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            for (int i = 0; i < handlers.Count; i++)
            {
                EventProcessorHandler tempHandler = handlers[i];
                if (tempHandler == handler)
                {
                    return;
                }
            }
            handlers.Add(handler);
        }

        /// <summary>
        /// Remove a handler.
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="handler"></param>
        private void RemoveHandler(int eventID, EventProcessorHandler handler)
        {
            List<EventProcessorHandler> handlers = null;
            if (m_RegisteredHandlers.TryGetValue(eventID, out handlers))
            {
                if (handlers != null && handlers.Count > 0)
                {
                    for (int i = handlers.Count - 1; i >= 0; --i)
                    {
                        EventProcessorHandler tempHandler = handlers[i];
                        if (tempHandler == null || tempHandler.Target == null)
                        {
                            m_WillBeDeletedMsgHandlers.Add(tempHandler);
                            continue;
                        }
                        if (handler != null && tempHandler.Target == handler.Target)
                        {
                            m_WillBeDeletedMsgHandlers.Add(tempHandler);
                            return;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Remove all handlers.
        /// </summary>
        private void RemoveAllHandlers()
        {
            if (m_RegisteredHandlers != null)
            {
                m_RegisteredHandlers.Clear();
            }
            if (m_WillBeDeletedMsgHandlers != null)
            {
                m_WillBeDeletedMsgHandlers.Clear();
            }
        }

        /// <summary>
        /// Notify event to all handlers.
        /// </summary>
        /// <param name="eventObj"></param>
        public void Notify(Event eventObj)
        {
            if (eventObj == null)
            {
                return;
            }
            Dispatch((int)eventObj.EventID, eventObj);
        }

        /// <summary>
        /// Register handler to event processor.
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="handler"></param>
        public void Register(int eventID, EventProcessorHandler handler)
        {
            if (handler != null)
            {
                AddHandler(eventID, handler);
            }
        }

        /// <summary>
        /// Unregister handler from event processor.
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="handler"></param>
        public void Unregister(int eventID, EventProcessorHandler handler)
        {
            if (handler != null)
            {
                RemoveHandler(eventID, handler);
            }
        }

        /// <summary>
        /// Unregister all handlers form event processor.
        /// </summary>
        public void UnregisterAll()
        {
            RemoveAllHandlers();
        }
    }

}
