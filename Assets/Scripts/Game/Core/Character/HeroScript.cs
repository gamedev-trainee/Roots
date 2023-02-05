using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Roots
{
    public class HeroScript : CharacterScript, ICharacterScriptListener
    {
        public enum AtomStates
        {
            None,
            Breaking,
            Breaked,
            Combining,
            Combined,
        }

        // Move

        public float moveSpeed = 0f;

        private MoveModule m_moveModule = new MoveModule();
        private MoveFilter m_moveFilter = new MoveFilter();

        // Atoms

        public GameObject atomTemplate = null;
        public float breakForce = 0f;

        private float m_atomRadius = 0f;
        private AtomScript m_mainAtom = null;
        private List<AtomScript> m_atomInstances = new List<AtomScript>();
        private AtomStates m_atomState = AtomStates.Combined;

        private int m_spaceFrame = 0;

        // Locator

        public GameObject locatorTemplate = null;

        private bool m_isLocating = false;
        private LocatorScript m_locator = null;

        // Animation

        public Animator ani = null;
        public string beattackStateName = string.Empty;

        protected override void onStart()
        {
            base.onStart();

            initMove();
            initAtoms();
        }

        protected override void onUpdate(float deltaTime)
        {
            base.onUpdate(deltaTime);
            
            updateMove(deltaTime);
            updateAtoms(deltaTime);
        }

        private void OnGUI()
        {
            if (!CoreStatics.ControlAvailable) return;
            handleMoveInput();
            handleAtomsInput();
        }

        #region Animation

        protected void activeAnimation()
        {
            ani.enabled = true;
        }

        protected void deactiveAnimation()
        {
            ani.enabled = false;
            transform.localScale = Vector3.one;
        }

        #endregion

        #region Move

        protected void initMove()
        {
            m_moveFilter.setRadius(radius);
            m_moveFilter.setLayerMask(1 << (int)GameObjectLayers.Default);
            resetMove();
        }

        protected void updateMove(float deltaTime)
        {
            m_moveModule.update(deltaTime);
        }

        protected void handleMoveInput()
        {
            if (Input.GetKey(KeyCode.LeftArrow)) m_moveModule.setDirectionX(-1);
            else if (Input.GetKey(KeyCode.RightArrow)) m_moveModule.setDirectionX(1);
            else m_moveModule.setDirectionX(0);
            if (Input.GetKey(KeyCode.UpArrow)) m_moveModule.setDirectionY(1);
            else if (Input.GetKey(KeyCode.DownArrow)) m_moveModule.setDirectionY(-1);
            else m_moveModule.setDirectionY(0);
        }

        public void resetMove()
        {
            m_moveModule.setTransform(transform);
            m_moveModule.setSpeed(moveSpeed);
            m_moveModule.setFilter(m_moveFilter);
        }

        #endregion

        #region Locator

        protected void enterLocateMode()
        {
            if (m_isLocating) return;
            m_isLocating = true;
            if (m_locator == null)
            {
                GameObject go = locatorTemplate.Clone();
                m_locator = go.GetComponent<LocatorScript>();
            }
            m_locator.gameObject.SetActive(true);
            m_locator.transform.position = transform.position;
            m_moveModule.setTransform(m_locator.transform);
            m_moveModule.setSpeed(m_locator.moveSpeed);
            m_moveModule.setFilter(null);
        }

        protected void leaveLocateMode()
        {
            if (!m_isLocating) return;
            m_isLocating = false;
            resetMove();
            m_locator.gameObject.SetActive(false);
        }

        protected void hideLocator()
        {
            if (m_locator == null) return;
            m_locator.gameObject.SetActive(false);
        }

        #endregion

        #region Atoms

        // ICharacterScriptListener

        public void onCharacterKill(CharacterScript script)
        {
            int index = m_atomInstances.IndexOf(script as AtomScript);
            if (index != -1)
            {
                m_atomInstances.RemoveAt(index);
                ani.Play(beattackStateName);
                if (m_atomInstances.Count <= 0)
                {
                    GameMain.Time.addDelay(() =>
                    {
                        SceneManager.LoadSceneAsync(GameConsts.EndScene);
                    });
                }
            }
        }

        protected void initAtoms()
        {
            initAtomInfo();
            generateAtoms();
            sortAtoms();
        }

        protected void initAtomInfo()
        {
            AtomScript atomScript = atomTemplate.GetComponent<AtomScript>();
            m_atomRadius = atomScript.radius;
        }

        protected void generateAtoms()
        {
            AtomScript atomScript;
            GameObject atomInst;
            int startCount = m_atomInstances.Count;
            int count = calculateAtomCount();
            for (int i = startCount; i < count; i++)
            {
                atomInst = atomTemplate.Clone(transform);
                atomScript = atomInst.GetComponent<AtomScript>();
                atomScript.deactive();
                atomScript.setListener(this);
                m_atomInstances.Add(atomScript);
            }
        }

        protected void sortAtoms()
        {
            if (m_atomInstances.Count <= 0) return;
            sortAtomsWith(m_atomInstances[0], false, false);
        }

        protected void sortAtomsWith(AtomScript mainAtom, bool asNewCenter, bool animate)
        {
            m_mainAtom = mainAtom;
            if (asNewCenter)
            {
                transform.position = m_mainAtom.transform.position;
            }
            m_mainAtom.transform.localPosition = Vector3.zero;
            int mainIndex = m_atomInstances.IndexOf(m_mainAtom);
            if (mainIndex > 0)
            {
                m_atomInstances.RemoveAt(mainIndex);
                m_atomInstances.Insert(0, m_mainAtom);
            }
            sortAtomsByDepth(1, 0, animate);
        }

        protected void sortAtomsByDepth(int index, int depth, bool animate = false)
        {
            float distance = (depth + 1) * m_atomRadius * 2f;
            float angleR = Mathf.Asin(m_atomRadius / distance);
            float angle = Mathf.Rad2Deg * angleR;
            int maxCount = Mathf.CeilToInt(360f / angle) + 1;
            int startIndex = index;
            for (int i = 0; i < maxCount; i++)
            {
                index = startIndex + i;
                if (index >= m_atomInstances.Count) break;
                setAtomPosition(m_atomInstances[index], distance, angle * i, animate);
            }
            if (index >= m_atomInstances.Count) return;
            sortAtomsByDepth(index + 1, depth + 1, animate);
        }

        protected void setAtomPosition(AtomScript atom, float distance, float angle, bool animate = false)
        {
            float angleR = Mathf.Deg2Rad * angle;
            Vector3 pos = atom.transform.localPosition;
            pos.x = Mathf.Sin(angleR) * distance;
            pos.y = Mathf.Cos(angleR) * distance;
            if (animate)
            {
                atom.startBackMove(pos);
            }
            else
            {
                atom.transform.localPosition = pos;
                atom.transform.localEulerAngles = Vector3.zero;
            }
        }

        protected int calculateAtomCount()
        {
            int maxCount = 0;
            int maxDepth = Mathf.CeilToInt((radius - m_atomRadius) / (m_atomRadius * 2f));
            for (int depth = 0; depth < maxDepth; depth++)
            {
                maxCount += calculateAtomCount(depth);
            }
            return maxCount;
        }

        protected int calculateAtomCount(int depth)
        {
            float distance = (depth + 1) * m_atomRadius * 2f;
            float angleR = Mathf.Asin(m_atomRadius / distance);
            float angle = Mathf.Rad2Deg * angleR;
            int maxCount = Mathf.CeilToInt(360f / angle) + 1;
            return maxCount;
        }

        public void breakAtoms()
        {
            if (m_atomState == AtomStates.Breaking || m_atomState == AtomStates.Breaked) return;
            deactiveAnimation();
            int count = m_atomInstances.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                m_atomInstances[i].active();
                m_atomInstances[i].startImpulseMove(m_atomInstances[i].transform.localPosition.normalized);
            }
            m_atomState = AtomStates.Breaking;
        }

        protected void combineAtoms()
        {
            if (m_atomState == AtomStates.Combining || m_atomState == AtomStates.Combined) return;
            AtomScript mainAtom = m_locator.getLocatedAtom(m_atomInstances);
            if (mainAtom == null) return;
            if (m_atomInstances.Count == 1)
            {
                growAtoms();
                return;
            }
            int count = m_atomInstances.Count;
            for (int i = 0; i < count; i++)
            {
                m_atomInstances[i].deactive();
            }
            m_atomState = AtomStates.Combining;
            hideLocator();
            sortAtomsWith(mainAtom, true, true);
        }

        protected void growAtoms()
        {
            m_atomState = AtomStates.Combined;
            generateAtoms();
            sortAtoms();
            leaveLocateMode();
            activeAnimation();
        }

        public void toggleAtomState()
        {
            if (m_atomInstances.Count == 1)
            {
                growAtoms();
                return;
            }
            if (m_atomState == AtomStates.Combined)
            {
                breakAtoms();
            }
            else if (m_atomState == AtomStates.Breaked)
            {
                combineAtoms();
            }
        }

        protected void updateAtoms(float deltaTime)
        {
            switch (m_atomState)
            {
                case AtomStates.Breaking:
                    {
                        bool done = true;
                        int count = m_atomInstances.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (m_atomInstances[i].isMoving())
                            {
                                done = false;
                                break;
                            }
                        }
                        if (done)
                        {
                            m_atomState = AtomStates.Breaked;
                            enterLocateMode();
                        }
                    }
                    break;
                case AtomStates.Combining:
                    {
                        bool done = true;
                        int count = m_atomInstances.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (m_atomInstances[i].isMoving())
                            {
                                done = false;
                                break;
                            }
                        }
                        if (done)
                        {
                            m_atomState = AtomStates.Combined;
                            activeAnimation();
                            leaveLocateMode();
                        }
                    }
                    break;
            }
        }

        protected void handleAtomsInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Time.frameCount == m_spaceFrame) return;
                m_spaceFrame = Time.frameCount;
                toggleAtomState();
            }
        }

        #endregion
    }
}
