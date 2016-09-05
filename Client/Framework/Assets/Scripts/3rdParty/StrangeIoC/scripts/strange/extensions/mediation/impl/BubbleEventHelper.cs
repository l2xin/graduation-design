using UnityEngine;
using System.Collections;
using strange.extensions.mediation.impl;

public class BubbleEventHelper
{
    private const int LOOP_MAX = 50;
    private EventView m_eventView;

    public BubbleEventHelper()
    {
        m_eventView = null;
    }

    ///
    /// <summary>
    /// Recurses through Transform.parent to find the GameObject to which EventView is attached
    /// Parent ventView will dispatch the event
    /// Has a loop limit of 15 levels.
    /// By default, raises an Exception if no EventView is found.
    /// </summary>
    /// <param name="trans">transform of the child</param>
    /// <param name="eventType">event type enum</param>
    /// <param name="data">data, default is null</param>
    public void BubbleDispatch(Transform trans, object eventType, object data = null)
    {
        if (m_eventView != null && m_eventView.dispatcher != null)
        {
            m_eventView.dispatcher.Dispatch(eventType, data);
        }
        else
        {
            bool isSuccess = false;
            int loopLimiter = 0;
            if (trans != null)
            {
                while (trans.parent != null && loopLimiter < LOOP_MAX)
                {
                    loopLimiter++;
                    trans = trans.parent;
                    m_eventView = trans.gameObject.GetComponent<EventView>();
                    if (m_eventView != null && m_eventView.dispatcher != null)
                    {
                        m_eventView.dispatcher.Dispatch(eventType, data);
                        isSuccess = true;
                        break;
                    }
                }
            }
            if (!isSuccess)
            {
                Debug.LogError("BubbleDispatch Failed. EventType:" + eventType);
            }
        }
    }

    /// <summary>
    /// clear the reference to the EventView
    /// </summary>
    public void ClearViewCache()
    {
        m_eventView = null;
    }
}
