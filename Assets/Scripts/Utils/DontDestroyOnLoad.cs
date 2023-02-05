using UnityEngine;

namespace Roots
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GameObject.DontDestroyOnLoad(gameObject);
        }
    }
}
