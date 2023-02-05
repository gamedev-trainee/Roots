using System.Collections.Generic;
using UnityEngine;

namespace Roots
{
    public class ResourceManager
    {
        private Dictionary<string, ResourceInfo> m_infoMap = new Dictionary<string, ResourceInfo>();
        private Dictionary<string, Object> m_cacheMap = new Dictionary<string, Object>();

        public ResourceManager()
        {

        }

        public void addResource(string name, Object res)
        {
            m_cacheMap[name] = res;
        }

        public T getResource<T>(string name) where T : Object
        {
            if (m_cacheMap.ContainsKey(name))
            {
                return (T)m_cacheMap[name];
            }
            return loadResourceSync<T>(name);
        }

        public T loadResourceSync<T>(string name) where T : Object
        {
            ResourceInfo info;
            if (!m_infoMap.TryGetValue(name, out info))
            {
                return default(T);
            }
            return Resources.Load<T>(info.path);
        }

        public ResourceRequest loadResource(string name)
        {
            ResourceInfo info;
            if (!m_infoMap.TryGetValue(name, out info))
            {
                return null;
            }
            return Resources.LoadAsync(info.path);
        }
    }
}
