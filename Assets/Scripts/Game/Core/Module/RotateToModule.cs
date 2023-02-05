using UnityEngine;

namespace Roots
{
    public class RotateToModule
    {
        private Transform m_transform = null;
        private float m_speed = 0f;
        private AnimationCurve m_curve = null;
        private float m_targetRotation = 0f;

        private bool m_isRotating = false;
        private float m_startRotation = 0f;
        private float m_endRotation = 0f;
        private float m_duration = 0f;
        private float m_passed = 0f;

        public void setTransform(Transform value)
        {
            m_transform = value;
        }

        public void setSpeed(float value)
        {
            m_speed = value;
        }

        public void setCurve(AnimationCurve value)
        {
            m_curve = value;
        }

        public bool compareTargetRotation(float value)
        {
            return m_targetRotation == value;
        }

        public void setTargetRotation(float value)
        {
            m_targetRotation = value;
            m_startRotation = m_transform.localEulerAngles.z;
            m_endRotation = m_targetRotation;
            if (CoreUtils.FilterRotation(ref m_startRotation, ref m_endRotation))
            {
                m_duration = Mathf.Abs(m_endRotation - m_startRotation) / m_speed;
                m_passed = 0f;
                m_isRotating = true;
            }
        }

        public bool isRotating()
        {
            return m_isRotating;
        }

        public void update(float deltaTime)
        {
            if (!m_isRotating)
            {
                return;
            }
            m_passed += deltaTime;
            float progress = m_passed / m_duration;
            if (m_curve != null) progress = m_curve.Evaluate(progress);
            if (progress > 1f) progress = 1f;
            float currentRotation = Mathf.Lerp(m_startRotation, m_endRotation, progress);
            Vector3 rotation = m_transform.localEulerAngles;
            rotation.z = -currentRotation;
            m_transform.localEulerAngles = rotation;
            if (progress >= 1f)
            {
                m_isRotating = false;
            }
        }
    }
}
