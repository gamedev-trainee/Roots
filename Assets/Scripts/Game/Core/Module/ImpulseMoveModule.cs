using UnityEngine;

namespace Roots
{
    public class ImpulseMoveModule
    {
        private Transform m_transform = null;
        private float m_speed = 0f;
        private float m_damping = 0f;
        private Vector2 m_forward = Vector2.zero;

        private MoveFilter m_filter = null;

        private float m_currentSpeed = 0f;

        public void setTransform(Transform value)
        {
            m_transform = value;
        }

        public void setSpeed(float value)
        {
            m_speed = value;
        }

        public void setDamping(float value)
        {
            m_damping = value;
        }

        public void setForward(Vector2 value)
        {
            m_forward = value;
            m_currentSpeed = m_speed;
        }

        public void setFilter(MoveFilter value)
        {
            m_filter = value;
        }

        public bool isMoving()
        {
            return m_forward.sqrMagnitude > 0f;
        }

        public void update(float deltaTime)
        {
            if (m_forward.sqrMagnitude <= 0f)
            {
                return;
            }
            Vector3 pos = m_transform.position;
            Vector2 offset = Vector2.zero;
            offset.x = m_currentSpeed * m_forward.x * deltaTime;
            offset.y = m_currentSpeed * m_forward.y * deltaTime;
            m_currentSpeed -= m_damping * deltaTime;
            if (m_filter != null)
            {
                m_filter.filter(pos, ref offset);
            }
            if (offset.sqrMagnitude > 0f)
            {
                pos.x += offset.x;
                pos.y += offset.y;
                m_transform.position = pos;
            }
            if (m_currentSpeed <= 0f)
            {
                stop();
            }
        }

        public void stop()
        {
            m_forward = Vector3.zero;
        }
    }
}
