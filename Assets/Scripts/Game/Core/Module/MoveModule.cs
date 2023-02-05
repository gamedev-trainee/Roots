using UnityEngine;

namespace Roots
{
    public class MoveModule
    {
        private Transform m_transform = null;
        private float m_speed = 0f;
        private Vector2Int m_direction = Vector2Int.zero;

        private MoveFilter m_filter = null;

        public void setTransform(Transform value)
        {
            m_transform = value;
        }

        public void setSpeed(float value)
        {
            m_speed = value;
        }

        public void setDirection(int vx, int vy)
        {
            m_direction.x = vx;
            m_direction.y = vy;
        }

        public void setDirectionX(int value)
        {
            m_direction.x = value;
        }

        public void setDirectionY(int value)
        {
            m_direction.y = value;
        }

        public void setFilter(MoveFilter value)
        {
            m_filter = value;
        }

        public bool isMoving()
        {
            return m_direction.sqrMagnitude > 0f;
        }

        public void update(float deltaTime)
        {
            if (m_direction.sqrMagnitude <= 0f)
            {
                return;
            }
            Vector3 pos = m_transform.position;
            Vector2 offset = Vector2.zero;
            offset.x = m_speed * m_direction.x * deltaTime;
            offset.y = m_speed * m_direction.y * deltaTime;
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
        }
    }
}
