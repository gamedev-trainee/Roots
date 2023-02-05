using UnityEngine;

namespace Roots
{
    public class CameraScript : MonoBehaviour
    {
        private Camera m_camera = null;

        private void Start()
        {
            m_camera = GetComponent<Camera>();
            GameMain.Camera.registerCamera(m_camera);
        }

        private void OnDestroy()
        {
            GameMain.Camera.unregisterCamera(m_camera);
        }
    }
}
