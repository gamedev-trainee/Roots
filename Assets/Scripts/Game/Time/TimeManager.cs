using System.Collections.Generic;
using UnityEngine;

namespace Roots
{
    public delegate void TimeDelayCallback();
    public delegate void TimeUpdateCallback(float deltaTime);

    public class TimeManager : ITimeLoopListener
    {
        public class TimeDelayInfo
        {
            public TimeDelayCallback callback = null;
            public float delay = 0f;
        }

        private List<TimeDelayInfo> m_delays = new List<TimeDelayInfo>();
        private List<TimeUpdateCallback> m_updateCallbacks = new List<TimeUpdateCallback>();
        private List<TimeUpdateCallback> m_lateUpdateCallbacks = new List<TimeUpdateCallback>();

        public TimeManager()
        {
            GameObject go = new GameObject("__loop__");
            TimeLoop loop = go.AddComponent<TimeLoop>();
            loop.setListener(this);
            GameObject.DontDestroyOnLoad(go);
        }

        public void addDelay(TimeDelayCallback callback, float delay = 0f)
        {
            m_delays.Add(new TimeDelayInfo()
            {
                callback = callback,
                delay = delay,
            });
        }

        public void listenUpdate(TimeUpdateCallback callback)
        {
            if (m_updateCallbacks.Contains(callback)) return;
            m_updateCallbacks.Add(callback);
        }

        public void unlistenUpdate(TimeUpdateCallback callback)
        {
            m_updateCallbacks.Remove(callback);
        }

        public void listenLateUpdate(TimeUpdateCallback callback)
        {
            if (m_lateUpdateCallbacks.Contains(callback)) return;
            m_lateUpdateCallbacks.Add(callback);
        }

        public void unlistenLateUpdate(TimeUpdateCallback callback)
        {
            m_lateUpdateCallbacks.Remove(callback);
        }

        public void update(float deltaTime)
        {
            lock (m_delays)
            {
                int count = m_delays.Count;
                for (int i = 0; i < count; i++)
                {
                    m_delays[i].delay -= deltaTime;
                    if (m_delays[i].delay > 0f) continue;
                    m_delays[i].callback.Invoke();
                    m_delays.RemoveAt(i);
                    i--;
                    count--;
                }
            }
            lock (m_updateCallbacks)
            {
                int count = m_updateCallbacks.Count;
                for (int i = 0; i < count; i++)
                {
                    m_updateCallbacks[i]?.Invoke(deltaTime);
                }
            }
        }

        public void lateUpdate(float deltaTime)
        {
            lock (m_lateUpdateCallbacks)
            {
                int count = m_lateUpdateCallbacks.Count;
                for (int i = 0; i < count; i++)
                {
                    m_lateUpdateCallbacks[i]?.Invoke(deltaTime);
                }
            }
        }
    }
}
