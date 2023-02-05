using UnityEngine;

namespace Roots
{
    public class MoveToModule
    {
        private Transform m_transform = null;
        private float m_speed = 0f;
        private Space m_space = Space.World;
        private Vector2 m_destination = Vector2.zero;
        private AnimationCurve m_curve = null;

        private MoveFilter m_filter = null;

        private Vector2 m_startPosition = Vector2.zero;
        private float m_duration = 0f;
        private float m_passed = 0f;
        private bool m_isMoving = false;

        public void setTransform(Transform value)
        {
            m_transform = value;
        }

        public void setSpeed(float value)
        {
            m_speed = value;
        }

        public void setSpace(Space value)
        {
            m_space = value;
        }

        public void setDestination(Vector2 value)
        {
            m_destination = value;
            m_startPosition = m_space == Space.Self ? m_transform.localPosition : m_transform.position;
            m_duration = Vector2.Distance(m_destination, m_startPosition) / m_speed;
            m_passed = 0f;
            m_isMoving = true;
        }

        public void setCurve(AnimationCurve value)
        {
            m_curve = value;
        }

        public void setFilter(MoveFilter value)
        {
            m_filter = value;
        }

        public bool isMoving()
        {
            return m_isMoving;
        }

        public void update(float deltaTime)
        {
            if (!m_isMoving)
            {
                return;
            }
            m_passed += deltaTime;
            float progress = m_passed / m_duration;
            if (m_curve != null) progress = m_curve.Evaluate(progress);
            if (progress > 1f) progress = 1f;
            Vector2 pos = m_space == Space.Self ? m_transform.localPosition : m_transform.position;
            Vector2 nextPos = Vector2.Lerp(m_startPosition, m_destination, progress);
            Vector2 offset = Vector2.zero;
            offset.x = nextPos.x - pos.x;
            offset.y = nextPos.y - pos.y;
            if (m_filter != null)
            {
                m_filter.filter(pos, ref offset);
            }
            if (offset.sqrMagnitude > 0f)
            {
                pos.x += offset.x;
                pos.y += offset.y;
                if (m_space == Space.Self)
                {
                    m_transform.localPosition = pos;
                }
                else
                {
                    m_transform.position = pos;
                }
            }
            if (progress >= 1f)
            {
                stop();
            }
        }

        public void stop()
        {
            m_isMoving = false;
        }
    }
}
