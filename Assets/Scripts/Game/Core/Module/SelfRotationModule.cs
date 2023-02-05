using UnityEngine;

namespace Roots
{
    public class SelfRotationModule
    {
        private Transform m_transform = null;
        private float m_speed = 0f;
        private int m_direction = 0;

        public void setTransform(Transform value)
        {
            m_transform = value;
        }

        public void setSpeed(float value)
        {
            m_speed = value;
        }

        public void setDirection(int value)
        {
            m_direction = value;
        }

        public bool isRotating()
        {
            return m_direction != 0;
        }

        public void update(float deltaTime)
        {
            if (m_direction == 0)
            {
                return;
            }
            Vector3 rotation = m_transform.localEulerAngles;
            rotation.z += m_speed * m_direction * deltaTime;
            m_transform.localEulerAngles = rotation;
        }
    }
}
