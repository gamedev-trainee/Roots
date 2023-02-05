using System.Collections.Generic;
using UnityEngine;

namespace Roots
{
    public class UIManager
    {
        private Transform m_root = null;

        private Dictionary<string, GameObject> m_instanceMap = new Dictionary<string, GameObject>();

        public UIManager()
        {

        }

        public void setRoot(Transform value)
        {
            if (m_root == value) return;
            m_root = value;
            m_instanceMap.Clear();
            Transform child;
            int count = m_root.childCount;
            for (int i = 0; i < count; i++)
            {
                child = m_root.GetChild(i);
                m_instanceMap.Add(child.name, child.gameObject);
            }
        }

        public UIScript showUI(string name)
        {
            GameObject ui;
            if (!m_instanceMap.ContainsKey(name))
            {
                GameObject go = GameMain.Resource.getResource<GameObject>(name);
                if (go == null)
                {
                    return null;
                }
                ui = go;
                ui.transform.SetParent(m_root);
                ui.transform.localPosition = Vector3.zero;
                ui.transform.localScale = Vector3.one;
                ui.transform.localEulerAngles = Vector3.zero;
                m_instanceMap.Add(name, ui);
            }
            else
            {
                ui = m_instanceMap[name];
            }
            ui.SetActive(true);
            return ui.GetComponent<UIScript>();
        }

        public void hideUI(string name, System.Action callback)
        {
            if (!m_instanceMap.ContainsKey(name))
            {
                return;
            }
            GameObject inst = m_instanceMap[name];
            Animation ani = inst.GetComponent<Animation>();
            if (ani != null)
            {
                string aniName = "hide";
                AnimationClip clip = ani.GetClip(aniName);
                float duration = clip.length;
                ani.Play(aniName);
                GameMain.Time.addDelay(() =>
                {
                    inst.SetActive(false);
                    callback?.Invoke();
                }, duration);
            }
            else
            {
                inst.SetActive(false);
                callback?.Invoke();
            }
        }
    }
}
