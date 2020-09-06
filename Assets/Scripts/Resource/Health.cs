﻿using System;
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
        [SerializeField]
        float healthPoints = 100.0f;

        public float CurrentHealth { get => healthPoints; }
        public bool isDead { get => healthPoints <= 0.0f; }

        private void Start()
        {
            healthPoints = GetComponent<BaseStats>().GetHealth();
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if (!isDead)
            {
                healthPoints = Mathf.Max(healthPoints - damage, 0.0f);
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
            if (experience)
            {
                experience.GainExperience(GetComponent<BaseStats>().GetExperienceReward());
            }
        }

        public object CaptureState()
        {
            return healthPoints;
        }

        public void RestoreState(object state)
        {
            if (state is float)
            {
                bool wasAlreadyDead = isDead;
                healthPoints = (float)state;
                if (isDead && !wasAlreadyDead)
                {
                    Die();
                }
            }
        }

        public float GetPercentage()
        {
            return 100.0f * healthPoints / GetComponent<BaseStats>().GetHealth();
        }
    }
}
