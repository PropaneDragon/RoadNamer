using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadNamer.Managers
{

    public interface IEventSubscriber
    {
        #region Methods to implement
        /// <summary>
        /// handle event received from eventBus
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventData"></param>
        void onReceiveEvent(string eventName, object eventData);
        #endregion

    }

    /// <summary>
    /// Event bus, allows objects to publish/subscribe to events from other objects
    /// </summary>
    class EventBusManager
    {
        private static EventBusManager instance = null;

        private Dictionary<string, List<IEventSubscriber>> m_subscribers = new Dictionary<string, List<IEventSubscriber>>();

        public static EventBusManager Instance()
        {
            if (instance == null)
            {
                instance = new EventBusManager();
            }

            return instance;
        }

        public void Subscribe(string eventName, IEventSubscriber subscriber)
        {
            if (!m_subscribers.ContainsKey(eventName))
            {
                m_subscribers[eventName] = new List<IEventSubscriber>();
            }
            m_subscribers[eventName].Add(subscriber);
        }

        public void UnSubscribe(string eventName, IEventSubscriber subscriber)
        {
            if (!m_subscribers.ContainsKey(eventName))
            {
                m_subscribers[eventName] = new List<IEventSubscriber>();
            }
            m_subscribers[eventName].Remove(subscriber);
        }

        public void Publish(string eventName, object eventData)
        {
            if (m_subscribers.ContainsKey(eventName))
            {
                foreach(IEventSubscriber subscriber in m_subscribers[eventName])
                {
                    subscriber.onReceiveEvent(eventName, eventData);
                }
            }
        }
    }
}
