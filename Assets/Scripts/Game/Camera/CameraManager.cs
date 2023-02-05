using Cinemachine;
using UnityEngine;

namespace Roots
{
    public class CameraManager
    {
        private Camera m_camera = null;
        private CinemachineBrain m_cameraBrain = null;

        private Transform m_follow = null;

        public CameraManager()
        {

        }

        public void registerCamera(Camera value)
        {
            if (m_camera == value || value == null) return;
            m_camera = value;
            m_cameraBrain = m_camera.GetComponent<CinemachineBrain>();
            GameMain.Time.listenUpdate(onRefreshFollow);
        }

        public void unregisterCamera(Camera value)
        {
            if (m_camera != value) return;
            m_camera = null;
            m_cameraBrain = null;
        }

        public void registerFollow(Transform value)
        {
            if (m_follow == value || value == null) return;
            m_follow = value;
            GameMain.Time.listenUpdate(onRefreshFollow);
        }

        public void unregisterFollow(Transform value)
        {
            if (m_follow != value) return;
            m_follow = null;
        }

        protected void onRefreshFollow(float detlaTime)
        {
            if (m_camera == null || m_follow == null) return;
            m_cameraBrain.ActiveVirtualCamera.Follow = m_follow;
            GameMain.Time.unlistenUpdate(onRefreshFollow);
        }
    }
}
