using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClass = null;

        [System.Serializable]
        class ProgressionCharacterClass
        {
            [SerializeField] CharacterClass characterClass;
            [SerializeField] public ProgressionCharacterClassLevel[] levels = null;
        }

        [System.Serializable]
        class ProgressionCharacterClassLevel
        {
            [SerializeField] public float health = 1.0f;
            [SerializeField] public float damage = 1.0f;
        }

        public float GetHealth(CharacterClass character, int level)
        {
            int characterIndex = (int)character;
            if (characterIndex < characterClass.Length &&
                level > 0 && level <= characterClass[characterIndex].levels.Length)
            {
                return characterClass[characterIndex].levels[level - 1].health;
            }
            return 0.0f;
        }
    }
}
