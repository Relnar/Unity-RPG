using System.Collections.Generic;
using RPG.Core;
using RPG.Movement;
using RPG.Resource;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] float timeBetweenAttacks = 1.0f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;
 
        Health target;
        Health health;
        Mover mover;
        BaseStats baseStats;
        float timeSinceLastAttack = 100.0f;
        Weapon currentWeapon = null;

        private void Awake()
        {
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
            baseStats = GetComponent<BaseStats>();
        }

        void Start()
        {
            if (currentWeapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (health.isDead)
            {
                return;
            }

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

        public Health GetTarget()
        {
            return target;
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

        public IEnumerable<float> GetAdditiveModifers(Stats.Stats stat)
        {
            if (stat == Stats.Stats.Damage)
            {
                yield return currentWeapon.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifers(Stats.Stats stat)
        {
            if (stat == Stats.Stats.Damage)
            {
                yield return currentWeapon.GetPercentageBonus();
            }
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
                float damage = baseStats.GetStat(Stats.Stats.Damage);
                if (currentWeapon.HasProjectile())
                {
                    currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, GetComponent<CapsuleCollider>(), damage);
                }
                else
                {
                    target.TakeDamage(gameObject, damage);
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
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(state as string);
            if (weapon)
            {
                EquipWeapon(weapon);
            }
        }
    }
}