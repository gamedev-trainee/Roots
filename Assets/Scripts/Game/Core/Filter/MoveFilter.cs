using UnityEngine;

namespace Roots
{
    public class MoveFilter
    {
        private float m_radius = 0f;
        private int m_layerMask = 0;

        public void setRadius(float value)
        {
            m_radius = value;
        }

        public void setLayerMask(int value)
        {
            m_layerMask = value;
        }

        public bool filter(Vector2 lastPos, ref Vector2 offset)
        {
            bool changed = false;
            if (offset.x != 0f)
            {
                Vector2 dir = new Vector2(offset.x > 0f ? 1f : -1f, 0f);
                float distance = m_radius + Mathf.Abs(offset.x);
                RaycastHit2D hit = Physics2D.Raycast(lastPos, dir, distance, m_layerMask);
                if (hit.collider != null)
                {
                    Vector3 nextPos = hit.point - dir * m_radius;
                    float nextOffsetX = nextPos.x - lastPos.x;
                    if (offset.x > 0f)
                    {
                        offset.x = Mathf.Max(0f, nextOffsetX);
                        changed = true;
                    }
                    else if (offset.x < 0f)
                    {
                        offset.x = Mathf.Min(0f, nextOffsetX);
                        changed = true;
                    }
                }
            }
            if (offset.y != 0f)
            {
                Vector2 dir = new Vector2(0f, offset.y > 0f ? 1f : -1f);
                float distance = m_radius + Mathf.Abs(offset.y);
                RaycastHit2D hit = Physics2D.Raycast(lastPos, dir, distance, m_layerMask);
                if (hit.collider != null)
                {
                    Vector3 nextPos = hit.point - dir * m_radius;
                    float nextOffsetY = nextPos.y - lastPos.y;
                    if (offset.y > 0f)
                    {
                        offset.y = Mathf.Max(0f, nextOffsetY);
                        changed = true;
                    }
                    else if (offset.y < 0f)
                    {
                        offset.y = Mathf.Min(0f, nextOffsetY);
                        changed = true;
                    }
                }
            }
            return changed;
        }
    }
}
