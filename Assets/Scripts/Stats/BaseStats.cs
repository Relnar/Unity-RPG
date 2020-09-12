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
        int currentLevel = 0;

        private void Start()
        {
            currentLevel = CalculateLevel();
            experience = GetComponent<Experience>();
            if (experience)
            {
                experience.OnExperienceGained += UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
            }
        }

        public float GetStat(Stats stat)
        {
            return progression ? progression.GetStat(stat, characterClass, startingLevel) : 0.0f;
        }

        public int GetLevel()
        {
            if (currentLevel < 1)
            {
                currentLevel = CalculateLevel();
            }
            return currentLevel;
        }

        public int CalculateLevel()
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
