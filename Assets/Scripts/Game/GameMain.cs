using UnityEngine;
using UnityEngine.SceneManagement;

namespace Roots
{
    public class GameMain
    {
        private static GameMain ms_instance = null;
        public static GameMain Instance
        {
            get
            {
                if (ms_instance == null)
                {
                    ms_instance = new GameMain();
                }
                return ms_instance;
            }
        }

        public static TimeManager Time => Instance.m_timeManager;
        public static ResourceManager Resource => Instance.m_resourceManager;
        public static UIManager UI => Instance.m_uiManager;
        public static AudioManager Audio => Instance.m_audioManager;
        public static CameraManager Camera => Instance.m_cameraManager;
        public static CoreManager Core => Instance.m_coreManager;

        private TimeManager m_timeManager = null;
        private ResourceManager m_resourceManager = null;
        private UIManager m_uiManager = null;
        private AudioManager m_audioManager = null;
        private CameraManager m_cameraManager = null;
        private CoreManager m_coreManager = null;

        private AsyncOperation m_sceneOperation = null;

        private LoadingScript m_loadingScript = null;

        public GameMain()
        {
            ms_instance = this;

            m_timeManager = new TimeManager();
            m_resourceManager = new ResourceManager();
            m_uiManager = new UIManager();
            m_audioManager = new AudioManager();
            m_cameraManager = new CameraManager();
            m_coreManager = new CoreManager();
        }

        public void launch()
        {
            Canvas canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                m_uiManager.setRoot(canvas.transform);
            }
            m_loadingScript = UI.showUI(GameConsts.LoadingUI) as LoadingScript;
            m_sceneOperation = SceneManager.LoadSceneAsync(GameConsts.StartScene);
            Time.listenUpdate(onSceneLoadUpdate);
        }

        protected void onSceneLoadUpdate(float deltaTime)
        {
            if (m_sceneOperation == null)
            {
                onLaunchEnd();
                return;
            }
            if (m_sceneOperation.isDone)
            {
                m_sceneOperation = null;
            }
        }

        protected void onLaunchEnd()
        {
            m_loadingScript.setMouseActive(true);
        }
    }
}
