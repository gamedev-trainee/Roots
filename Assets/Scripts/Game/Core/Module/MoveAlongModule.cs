using UnityEngine;

namespace Roots
{
    public class MoveAlongModule
    {
        private Transform m_transform = null;
        private float m_speed = 0f;
        private Vector2 m_forward = Vector2.zero;

        private MoveFilter m_filter = null;

        private float m_movedDistance = 0f;

        public void setTransform(Transform value)
        {
            m_transform = value;
        }

        public void setSpeed(float value)
        {
            m_speed = value;
        }

        public void setForward(Vector2 value)
        {
            m_forward = value;
            m_movedDistance = 0f;
        }

        public void setFilter(MoveFilter value)
        {
            m_filter = value;
        }

        public bool isMoving()
        {
            return m_forward.sqrMagnitude > 0f;
        }

        public float getMovedDistance()
        {
            return m_movedDistance;
        }

        public void update(float deltaTime)
        {
            if (m_forward.sqrMagnitude <= 0f)
            {
                return;
            }
            Vector3 pos = m_transform.position;
            Vector2 offset = Vector2.zero;
            offset.x = m_speed * m_forward.x * deltaTime;
            offset.y = m_speed * m_forward.y * deltaTime;
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
            m_movedDistance += m_speed * deltaTime;
        }
    }
}
