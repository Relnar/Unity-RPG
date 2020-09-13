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
        [SerializeField] bool shouldUseModifiers = false;

        Experience experience;
        int currentLevel = 0;
        IModifierProvider[] modifierProviders;

        public event Action OnLevelUp;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            modifierProviders = GetComponents<IModifierProvider>();
        }

        private void Start()
        {
            currentLevel = CalculateLevel();
        }

        private void OnEnable()
        {
            // Callbacks should be registered in OnEnabled
            if (experience)
            {
                experience.OnExperienceGained += UpdateLevel;
            }
            this.OnLevelUp += LevelUpEffect;
        }

        private void OnDisable()
        {
            // Callbacks should be unregistered in OnDisabled
            if (experience)
            {
                experience.OnExperienceGained -= UpdateLevel;
            }
            this.OnLevelUp -= LevelUpEffect;
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
            return ApplyModifiers(stat, GetBaseStat(stat, level));
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

        private float GetBaseStat(Stats stat, int level)
        {
            return progression.GetStat(stat, characterClass, level);
        }

        private float ApplyModifiers(Stats stat, float baseStat)
        {
            if (shouldUseModifiers)
            {
                float additiveModifier = 0.0f;
                float percentageModifier = 0.0f;
                foreach (IModifierProvider provider in modifierProviders)
                {
                    foreach (float value in provider.GetAdditiveModifers(stat))
                    {
                        additiveModifier += value;
                    }
                    foreach (float value in provider.GetPercentageModifers(stat))
                    {
                        percentageModifier += value;
                    }
                }
                return (baseStat + additiveModifier) * (1.0f + percentageModifier * 0.01f);
            }
            return baseStat;
        }
    }
}
