using UnityEngine;

namespace Roots
{
    public interface ICharacterScriptListener
    {
        void onCharacterKill(CharacterScript script);
    }

    public abstract class CharacterScript : MonoBehaviour
    {
        public CharacterTypes type = CharacterTypes.None;
        public CharacterGroups group = CharacterGroups.None;
        public float radius = 0f;
        public bool beattackable = false;

        private ICharacterScriptListener m_listener = null;

        private bool m_killed = false;

        private void Start()
        {
            GameMain.Core.registerCharacter(this);
            onStart();
        }

        private void Update()
        {
            if (m_killed)
            {
                GameObject.Destroy(gameObject);
                return;
            }
            onUpdate(Time.deltaTime);
        }

        private void LateUpdate()
        {
            onLateUpdate(Time.deltaTime);
        }

        private void OnDestroy()
        {
            onDestroy();
            GameMain.Core.unregisterCharacter(this);
        }

        protected virtual void onStart()
        {

        }

        protected virtual void onUpdate(float deltaTime)
        {

        }

        protected virtual void onLateUpdate(float deltaTime)
        {

        }

        protected virtual void onDestroy()
        {

        }

        public void setListener(ICharacterScriptListener value)
        {
            m_listener = value;
        }

        public virtual bool isBeattackable()
        {
            return beattackable;
        }

        public bool isAlive()
        {
            return !m_killed;
        }

        public void kill()
        {
            if (m_killed) return;
            m_killed = true;
            m_listener?.onCharacterKill(this);
        }
    }
}
