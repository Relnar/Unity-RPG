using System;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass = CharacterClass.Grunt;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpEffect = null;

        Experience experience;
        int currentLevel = 0;

        public event Action OnLevelUp;

        private void Start()
        {
            experience = GetComponent<Experience>();
            currentLevel = CalculateLevel();
            if (experience)
            {
                experience.OnExperienceGained += UpdateLevel;
            }
            this.OnLevelUp += LevelUpEffect;
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
                OnLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            if (levelUpEffect)
            {
                GameObject.Instantiate(levelUpEffect, transform);
            }
        }

        public float GetStat(Stats stat, int level)
        {
            return progression ? progression.GetStat(stat, characterClass, level) : 0.0f;
        }

        public float GetStat(Stats stat)
        {
            return GetStat(stat, currentLevel);
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
            if (currentLevel < 1)
            {
                currentLevel = startingLevel;
            }
            else if (experience && progression)
            {
                var currentXP = experience.GetExperience();
                var xpToLevelUp = GetStat(Stats.XPtoLevelUp);
                if (currentXP >= xpToLevelUp && currentLevel < 99)
                {
                    return currentLevel + 1;
                }
            }

            return currentLevel;
        }
    }
}
