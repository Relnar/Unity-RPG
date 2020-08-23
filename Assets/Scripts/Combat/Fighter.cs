using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField]
        float weaponRange = 2.0f;
        [SerializeField]
        float timeBetweenAttacks = 1.0f;
        [SerializeField]
        float weaponDamage = 5.0f;

        Health target;
        Mover mover;
        float timeSinceLastAttack = 100.0f;

        // Start is called before the first frame update
        void Start()
        {
            mover = GetComponent<Mover>();
        }

        // Update is called once per frame
        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target && mover)
            {
                // Stop if in weapon's range
                if (Vector3.Distance(transform.position, target.transform.position) <= weaponRange)
                {
                    mover.Cancel();
                    AttackBehavior();
                }
                else
                {
                    mover.MoveTo(target.transform.position, 1.0f);
                }
            }
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget)
            {
                Health targetHealth = combatTarget.GetComponent<Health>();
                return targetHealth && !targetHealth.isDead;
            }
            return false;
        }

        void AttackBehavior()
        {
            transform.LookAt(target.transform);

            // Wait some time before triggering another attack
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                // This will trigger the Hit() event
                if (!target.isDead)
                {
                    TriggerAttack();
                }
                timeSinceLastAttack = 0.0f;
            }
        }

        private void TriggerAttack()
        {
            if (TryGetComponent(out Animator animator))
            {
                animator.ResetTrigger("stopAttack");
                animator.SetTrigger("attack");
            }
        }

        // Animation event
        void Hit()
        {
            if (target && !target.isDead)
            {
                target.TakeDamage(weaponDamage);
                if (target.isDead)
                {
                    Cancel();
                }
            }
        }

        public void Cancel()
        {
            if (TryGetComponent(out Animator animator))
            {
                animator.ResetTrigger("attack");
                animator.SetTrigger("stopAttack");
            }
            GetComponent<Mover>().Cancel();
            target = null;
        }
    }
}