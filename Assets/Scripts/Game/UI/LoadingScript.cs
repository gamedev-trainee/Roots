using UnityEngine;

namespace Roots
{
    public class LoadingScript : UIScript
    {
        public GameObject mouseNode = null;

        public void setMouseActive(bool value)
        {
            mouseNode.SetActive(value);
        }

        public void onButtonClick()
        {
            setMouseActive(false);
            GameMain.UI.hideUI(GameConsts.LoadingUI, onLoadingHide);
        }

        protected void onLoadingHide()
        {
            GameMain.UI.showUI(GameConsts.GuideUI);
            CoreStatics.ControlAvailable = true;
        }
    }
}
