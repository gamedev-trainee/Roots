using System.Collections.Generic;

namespace Roots
{
    public class CoreManager
    {
        private Dictionary<CharacterGroups, List<CharacterScript>> m_groupCharacterMap = new Dictionary<CharacterGroups, List<CharacterScript>>();

        public void registerCharacter(CharacterScript script)
        {
            if (script.type != CharacterTypes.Atom) return;
            List<CharacterScript> list;
            if (!m_groupCharacterMap.TryGetValue(script.group, out list))
            {
                list = new List<CharacterScript>();
                m_groupCharacterMap.Add(script.group, list);
            }
            if (!list.Contains(script))
            {
                list.Add(script);
            }
        }

        public void unregisterCharacter(CharacterScript script)
        {
            if (script.type != CharacterTypes.Atom) return;
            List<CharacterScript> list;
            if (!m_groupCharacterMap.TryGetValue(script.group, out list)) return;
            list.Remove(script);
        }

        public List<CharacterScript> getGroupCharacters(CharacterGroups group)
        {
            if (m_groupCharacterMap.ContainsKey(group))
            {
                return m_groupCharacterMap[group];
            }
            return null;
        }
    }
}
