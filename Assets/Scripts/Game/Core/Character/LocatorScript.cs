using System.Collections.Generic;
using UnityEngine;

namespace Roots
{
    public class LocatorScript : MonoBehaviour
    {
        public float radius = 0f;
        public float moveSpeed = 0f;

        public AtomScript getLocatedAtom(List<AtomScript> atoms)
        {
            Vector3 pos = transform.position;
            float distance;
            AtomScript minAtom = null;
            float minDistance = 0f;
            int count = atoms.Count;
            for (int i = 0; i < count; i++)
            {
                distance = (atoms[i].transform.position - pos).magnitude;
                if (distance > atoms[i].radius + radius) continue;
                if (minAtom == null || distance < minDistance)
                {
                    minAtom = atoms[i];
                    minDistance = distance;
                }
            }
            return minAtom;
        }
    }
}
