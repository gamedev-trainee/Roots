using System.Collections.Generic;
using UnityEngine;

namespace Roots
{
    public class MonsterScript : CharacterScript
    {
        public float selfRotationSpeed = 0f;

        private SelfRotationModule m_selfRotationModule = new SelfRotationModule();

        public float attackRadius = 0f;
        public float attackRotateSpeed = 0f;
        public AnimationCurve attackRotateCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        private RotateToModule m_rotateToModule = new RotateToModule();

        public GameObject bulletTemplate = null;
        public Transform bulletBorn = null;
        public float bulletEmitInterval = 0f;

        private float m_currentEmitInterval = 0f;

        public GameObject lockOnTemplate = null;

        private Transform m_lockOn = null;
        private int m_targetHash = 0;
        private CharacterScript m_target = null;

        protected override void onStart()
        {
            base.onStart();

            m_selfRotationModule.setTransform(transform);
            m_selfRotationModule.setSpeed(selfRotationSpeed);
            startSelfRotation();

            m_rotateToModule.setTransform(transform);
            m_rotateToModule.setSpeed(attackRotateSpeed);
            m_rotateToModule.setCurve(attackRotateCurve);
        }

        protected override void onUpdate(float deltaTime)
        {
            base.onUpdate(deltaTime);

            m_selfRotationModule.update(deltaTime);
            m_rotateToModule.update(deltaTime);
        }

        protected override void onLateUpdate(float deltaTime)
        {
            base.onLateUpdate(deltaTime);

            bool changed = false;
            if (hasTarget())
            {
                if (!checkTarget())
                {
                    clearTarget();
                    changed = true;
                }
            }
            if (!hasTarget())
            {
                setTarget(findTarget());
                if (hasTarget())
                {
                    changed = true;
                }
            }
            if (changed)
            {
                if (m_target != null)
                {
                    stopSelfRotation();
                    m_currentEmitInterval = 0f;
                    lookAtTarget(m_target);
                }
                else
                {
                    startSelfRotation();
                }
                changeLockOn(m_target);
                return;
            }
            if (m_target == null)
            {
                return;
            }
            updateLockOn(m_target);
            if (m_rotateToModule.isRotating())
            {
                return;
            }
            lookAtTarget(m_target);
            if (m_currentEmitInterval > 0f)
            {
                m_currentEmitInterval -= Time.deltaTime;
                return;
            }
            m_currentEmitInterval = bulletEmitInterval;
            emitBullet();
        }

        protected void startSelfRotation()
        {
            m_selfRotationModule.setDirection(Random.Range(0, 100) % 2 == 0 ? -1 : 1);
        }

        protected void stopSelfRotation()
        {
            m_selfRotationModule.setDirection(0);
        }

        protected void lookAtTarget(CharacterScript target)
        {
            if (target == null) return;
            Vector3 pos = transform.position;
            Vector3 targetPos = target.transform.position;
            Vector3 dir = targetPos - pos;
            float distance = Vector3.Distance(pos, targetPos);
            float currentAngle = -transform.eulerAngles.z;
            float targetAngle = Vector2.SignedAngle(new Vector2(dir.x, dir.y), Vector2.up);
            float angleDiff = CoreUtils.GetRotationDiff(currentAngle, targetAngle);
            float attackableAngleR = Mathf.Asin(target.radius / distance);
            float attackableAngle = Mathf.Rad2Deg * attackableAngleR;
            bool animate = angleDiff > attackableAngle;
            if (animate)
            {
                m_rotateToModule.setTargetRotation(targetAngle);
            }
            else
            {
                Vector3 rotation = transform.localEulerAngles;
                rotation.z = -targetAngle;
                transform.localEulerAngles = rotation;
            }
        }

        protected void onRotateTo()
        {
            emitBullet();
        }

        protected void emitBullet()
        {
            GameObject go = bulletTemplate.Clone();
            go.transform.position = bulletBorn.position;
            Vector3 rotation = go.transform.localEulerAngles;
            rotation.z = transform.eulerAngles.z;
            go.transform.localEulerAngles = rotation;
            BulletScript script = go.GetComponent<BulletScript>();
            Vector3 dir = m_target.transform.position - transform.position;
            float angle = Vector2.SignedAngle(new Vector2(dir.x, dir.y), Vector2.up);
            float angleR = Mathf.Deg2Rad * angle;
            Vector2 forward = Vector2.zero;
            forward.x = Mathf.Sin(angleR);
            forward.y = Mathf.Cos(angleR);
            script.startMoveAlong(forward);
        }

        protected CharacterScript findTarget()
        {
            CharacterGroups enemyGroup = group == CharacterGroups.Friend ? CharacterGroups.Enemy : CharacterGroups.Friend;
            List<CharacterScript> enemies = GameMain.Core.getGroupCharacters(enemyGroup);
            if (enemies == null || enemies.Count <= 0) return null;
            Vector3 pos = transform.position;
            CharacterScript minEnemy = null;
            float minDistance = 0f;
            float distance;
            int count = enemies.Count;
            for (int i = 0; i < count; i++)
            {
                if (!enemies[i].isAlive()) continue;
                if (!enemies[i].isBeattackable()) continue;
                distance = (enemies[i].transform.position - pos).magnitude;
                if (distance >= attackRadius + enemies[i].radius) continue;
                if (minEnemy == null || distance < minDistance)
                {
                    minEnemy = enemies[i];
                    minDistance = distance;
                }
            }
            return minEnemy;
        }

        protected bool isValidTarget(CharacterScript script)
        {
            if (!script.isAlive()) return false;
            if (!script.isBeattackable()) return false;
            Vector3 pos = transform.position;
            float distance = (script.transform.position - pos).magnitude;
            if (distance >= attackRadius + script.radius) return false;
            return true;
        }

        protected void changeLockOn(CharacterScript target)
        {
            if (target != null)
            {
                if (m_lockOn == null)
                {
                    m_lockOn = lockOnTemplate.Clone().transform;
                }
                m_lockOn.gameObject.SetActive(true);
                m_lockOn.position = target.transform.position;
            }
            else
            {
                if (m_lockOn == null)
                {
                    return;
                }
                m_lockOn.gameObject.SetActive(false);
            }
        }

        protected void updateLockOn(CharacterScript target)
        {
            if (m_lockOn != null && target != null)
            {
                m_lockOn.position = target.transform.position;
            }
        }

        protected bool hasTarget()
        {
            return m_targetHash != 0;
        }

        protected bool checkTarget()
        {
            if (m_target != null)
            {
                if (!isValidTarget(m_target))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        protected void clearTarget()
        {
            m_target = null;
            m_targetHash = 0;
        }

        protected void setTarget(CharacterScript value)
        {
            m_target = value;
            m_targetHash = m_target != null ? m_target.GetHashCode() : 0;
        }
    }
}
