using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass = CharacterClass.Grunt;
        [SerializeField] Progression progression = null;

        Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();
        }

        public float GetStat(Stats stat)
        {
            return progression ? progression.GetStat(stat, characterClass, startingLevel) : 0.0f;
        }

        public int GetLevel()
        {
            if (experience && progression)
            {
                var currentXP = experience.GetExperience();
                var xpToLevelUp = GetStat(Stats.XPtoLevelUp);
                if (currentXP >= xpToLevelUp && startingLevel < 99)
                {
                    startingLevel++;
                }
            }

            return startingLevel;
        }
    }
}
