using UnityEngine;

namespace Roots
{
    public class TimeLoop : MonoBehaviour
    {
        private ITimeLoopListener m_listener = null;

        public void setListener(ITimeLoopListener value)
        {
            m_listener = value;
        }

        private void Update()
        {
            m_listener?.update(Time.deltaTime);
        }

        private void LateUpdate()
        {
            m_listener?.lateUpdate(Time.deltaTime);
        }
    }
}
