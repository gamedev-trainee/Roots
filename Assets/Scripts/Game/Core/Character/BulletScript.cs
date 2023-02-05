using System.Collections.Generic;
using UnityEngine;

namespace Roots
{
    public class BulletScript : CharacterScript
    {
        public static readonly float MaxMoveDistance = 50f;

        public float moveSpeed = 0f;
        public AudioClip emitAudio = null;
        public AudioClip killAudio = null;

        private MoveAlongModule m_moveAlongModule = new MoveAlongModule();

        private List<CharacterScript> m_attackTargets = new List<CharacterScript>();

        protected override void onStart()
        {
            base.onStart();

            m_moveAlongModule.setTransform(transform);
            m_moveAlongModule.setSpeed(moveSpeed);
        }

        protected override void onUpdate(float deltaTime)
        {
            base.onUpdate(deltaTime);

            m_moveAlongModule.update(deltaTime);
        }

        protected override void onLateUpdate(float deltaTime)
        {
            base.onLateUpdate(deltaTime);

            if (m_moveAlongModule.isMoving())
            {
                if (m_moveAlongModule.getMovedDistance() >= MaxMoveDistance)
                {
                    kill();
                    return;
                }
            }
            if (m_attackTargets.Count > 0)
            {
                int count = m_attackTargets.Count;
                for (int i = 0; i < count; i++)
                {
                    m_attackTargets[i].kill();
                }
                m_attackTargets.Clear();
                kill();
                GameMain.Audio.play(killAudio);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            CharacterScript script = collision.GetComponent<CharacterScript>();
            if (script.group == group) return;
            if (script.type != CharacterTypes.Atom) return;
            if (m_attackTargets.Contains(script)) return;
            m_attackTargets.Add(script);
        }

        public void startMoveAlong(Vector3 forward)
        {
            m_moveAlongModule.setForward(forward);
            GameMain.Audio.play(emitAudio);
        }
    }
}
