using System;
using GameDevTV.Utils;
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
        LazyValue<int> currentLevel;
        int CurrentLevel { get => currentLevel.value; set => currentLevel.value = value; }
        IModifierProvider[] modifierProviders;

        public event Action OnLevelUp;

        private void Awake()
        {
            currentLevel = new LazyValue<int>(CalculateLevel);
            experience = GetComponent<Experience>();
            modifierProviders = GetComponents<IModifierProvider>();
        }

        private void Start()
        {
            currentLevel.ForceInit();
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
            if (newLevel > CurrentLevel)
            {
                CurrentLevel = newLevel;
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
            return GetStat(stat, CurrentLevel);
        }

        public int GetLevel()
        {
            return CurrentLevel;
        }

        public int CalculateLevel()
        {
            if (experience == null)
            {
                return startingLevel;
            }
            else if (experience && progression)
            {
                var currentXP = experience.GetExperience();
                if (!currentLevel.IsInitialized())
                {
                    int level = 1;
                    for ( ; level < 99; ++level)
                    {
                        var xpToLevelUp = GetStat(Stats.XPtoLevelUp, level);
                        if (currentXP < xpToLevelUp)
                        {
                            return level;
                        }
                    }
                }
                else
                {
                    var xpToLevelUp = GetStat(Stats.XPtoLevelUp);
                    if (currentXP >= xpToLevelUp && CurrentLevel < 99)
                    {
                        return CurrentLevel + 1;
                    }
                }
            }

            return CurrentLevel;
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
