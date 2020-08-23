using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;


namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField]
        float healthPoints = 100.0f;

        public float CurrentHealth { get => healthPoints; }
        public bool isDead { get => healthPoints <= 0.0f; }

        public void TakeDamage(float damage)
        {
            if (!isDead)
            {
                healthPoints = Mathf.Max(healthPoints - damage, 0.0f);
                if (isDead)
                {
                    Die();
                }

            }
        }

        void Die()
        {
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
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
    }
}
