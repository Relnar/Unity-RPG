using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float timeBetweenAttacks = 1.0f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;
 
        Health target;
        Mover mover;
        float timeSinceLastAttack = 100.0f;
        Weapon currentWeapon = null;

        // Start is called before the first frame update
        void Start()
        {
            mover = GetComponent<Mover>();

            if (currentWeapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
        }

        // Update is called once per frame
        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target && mover)
            {
                // Stop if in weapon's range
                if (currentWeapon && Vector3.Distance(transform.position, target.transform.position) <= currentWeapon.GetRange())
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

        public void EquipWeapon(Weapon weapon)
        {
            if (weapon && weapon != currentWeapon && rightHandTransform)
            {
                weapon.Spawn(rightHandTransform, leftHandTransform, GetComponent<Animator>());
            }
            currentWeapon = weapon;
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
            if (target && !target.isDead && currentWeapon)
            {
                if (currentWeapon.HasProjectile())
                {
                    currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, target, GetComponent<CapsuleCollider>());
                }
                else
                {
                    target.TakeDamage(currentWeapon.GetDamage());
                }

                if (target.isDead)
                {
                    Cancel();
                }
            }
        }

        // Animation event
        void Shoot()
        {
            Hit();
        }

        public object CaptureState()
        {
            return currentWeapon.name;
        }

        public void RestoreState(object state)
        {
            Weapon weapon = Resources.Load<Weapon>(state as string);
            if (weapon)
            {
                EquipWeapon(weapon);
            }
        }
    }
}