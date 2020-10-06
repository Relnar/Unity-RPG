using System;
using GameDevTV.Utils;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        BaseStats baseStats = null;

        [Serializable]
        // Need to subclass the UnityEvent when we want to pass an argument
        public class TakeDamageEvent : UnityEvent<float>
        {}
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        LazyValue<float> healthPoints;
        public float CurrentHealth
        {
            get
            {
                return healthPoints.value;
            }
            private set
            {
                healthPoints.value = value;
            }
        }

        public float MaxHealth { get; private set; }
        public bool isDead { get => CurrentHealth <= 0.0f; }

        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
            baseStats = GetComponent<BaseStats>();
        }

        private void Start()
        {
            healthPoints.ForceInit();
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
                    onDie.Invoke();
                    Die();
                    AwardExperience(instigator);
                }
                else
                {
                    takeDamage.Invoke(damage);
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

        private float GetInitialHealth()
        {
            return baseStats.GetStat(Stats.Stats.Health);
        }
    }
}
