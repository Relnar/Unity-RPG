using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;


namespace RPG.Resource
{
    public class Health : MonoBehaviour, ISaveable
    {
        BaseStats baseStats = null;

        public float CurrentHealth { get; private set; } = -1.0f;
        public bool isDead { get => CurrentHealth <= 0.0f; }

        private void Start()
        {
            baseStats = GetComponent<BaseStats>();
            if (CurrentHealth < 0.0f)
            {
                CurrentHealth = baseStats.GetStat(Stats.Stats.Health);
            }
            baseStats.OnLevelUp += UpdateHealthOnLevelUp;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if (!isDead)
            {
                CurrentHealth = Mathf.Max(CurrentHealth - damage, 0.0f);
                if (isDead)
                {
                    Die();
                    AwardExperience(instigator);
                }
            }
        }

        void Die()
        {
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience && instigator.CompareTag("Player"))
            {
                experience.GainExperience((int)baseStats.GetStat(Stats.Stats.ExperienceReward));
            }
        }

        public object CaptureState()
        {
            return CurrentHealth;
        }

        public void RestoreState(object state)
        {
            if (state is float)
            {
                CurrentHealth = (float)state;
                if (isDead)
                {
                    Die();
                }
            }
        }

        public float GetPercentage()
        {
            return 100.0f * CurrentHealth / baseStats.GetStat(Stats.Stats.Health);
        }

        private void UpdateHealthOnLevelUp()
        {
            float oldMaxHealth = baseStats.GetStat(Stats.Stats.Health, baseStats.GetLevel() - 1);
            float newMaxHealth = baseStats.GetStat(Stats.Stats.Health);

            // Gain the extra health from leveling up
            CurrentHealth = Mathf.Min(CurrentHealth + (newMaxHealth - oldMaxHealth), newMaxHealth);
        }
    }
}
