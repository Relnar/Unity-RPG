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
        public float MaxHealth { get; private set; }
        public bool isDead { get => CurrentHealth <= 0.0f; }

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
        }

        private void Start()
        {
            if (CurrentHealth < 0.0f)
            {
                CurrentHealth = baseStats.GetStat(Stats.Stats.Health);
            }
            MaxHealth = baseStats.GetStat(Stats.Stats.Health);
        }

        private void OnEnable()
        {
            baseStats.OnLevelUp += UpdateHealthOnLevelUp;
            
        }

        private void OnDisable()
        {
            baseStats.OnLevelUp -= UpdateHealthOnLevelUp;
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
            float newMaxHealth = baseStats.GetStat(Stats.Stats.Health);

            // Gain the extra health from leveling up
            CurrentHealth = Mathf.Min(CurrentHealth + (newMaxHealth - MaxHealth), newMaxHealth);
            MaxHealth = newMaxHealth;
        }
    }
}
