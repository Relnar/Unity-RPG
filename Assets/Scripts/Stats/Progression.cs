using System;
using RPG.Utils;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [EnumNamedArray(typeof(CharacterClass))]
        [SerializeField] ProgressionCharacterClass[] characterClass = new ProgressionCharacterClass[Enum.GetValues(typeof(CharacterClass)).Length];

        [System.Serializable]
        class ProgressionCharacterClass
        {
            [NamedArray("Level", 1)]
            public ProgressionStat[] levels = new ProgressionStat[1];
        }

        [System.Serializable]
        class ProgressionStat
        {
            [EnumNamedArray(typeof(Stats))]
            public float[] stats = new float[Enum.GetValues(typeof(Stats)).Length];
        }

        public float GetStat(Stats stat, CharacterClass character, int level)
        {
            int characterIndex = (int)character;
            int statIndex = (int)stat;
            if (characterIndex < characterClass.Length &&
                level > 0 && level <= characterClass[characterIndex].levels.Length &&
                statIndex < characterClass[characterIndex].levels[level - 1].stats.Length)
            {
                return characterClass[characterIndex].levels[level - 1].stats[statIndex];
            }
            return 0.0f;
        }
    }
}
