using UnityEngine;

namespace Roots
{
    public class AtomScript : CharacterScript
    {
        public float impulseSpeed = 0f;
        public float impulseDamping = 0f;

        public float backMoveSpeed = 0f;
        public AnimationCurve backMoveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        private ImpulseMoveModule m_impulseMoveModule = new ImpulseMoveModule();
        private MoveToModule m_moveToModule = new MoveToModule();
        private MoveFilter m_moveFilter = new MoveFilter();

        private bool m_inited = false;
        private bool m_isActivated = false;

        protected override void onStart()
        {
            base.onStart();

            init();
            m_impulseMoveModule.setTransform(transform);
            m_impulseMoveModule.setSpeed(impulseSpeed);
            m_impulseMoveModule.setDamping(impulseDamping);
            m_impulseMoveModule.setFilter(m_moveFilter);
            m_moveToModule.setTransform(transform);
            m_moveToModule.setSpeed(backMoveSpeed);
            m_moveToModule.setSpace(Space.Self);
            m_moveToModule.setCurve(backMoveCurve);
            m_moveFilter.setRadius(radius);
            m_moveFilter.setLayerMask(1 << (int)GameObjectLayers.Default);
        }

        protected override void onUpdate(float deltaTime)
        {
            base.onUpdate(deltaTime);

            m_impulseMoveModule.update(deltaTime);
            m_moveToModule.update(deltaTime);
        }

        protected void init()
        {
            if (m_inited) return;
            m_inited = true;
        }

        public void active()
        {
            if (m_isActivated) return;
            m_isActivated = true;
            init();
            m_moveToModule.stop();
        }

        public void deactive()
        {
            if (!m_isActivated) return;
            m_isActivated = false;
            init();
            m_impulseMoveModule.stop();
        }

        public bool isMoving()
        {
            if (m_isActivated)
            {
                return m_impulseMoveModule.isMoving();
            }
            else
            {
                return m_moveToModule.isMoving();
            }
        }

        public void startImpulseMove(Vector3 dir)
        {
            m_impulseMoveModule.setForward(dir);
        }

        public void startBackMove(Vector3 destination)
        {
            m_moveToModule.setDestination(destination);
        }

        public override bool isBeattackable()
        {
            if (m_isActivated)
            {
                return m_impulseMoveModule.isMoving();
            }
            return true;
        }
    }
}
