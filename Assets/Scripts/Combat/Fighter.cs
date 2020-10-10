using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
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
        [SerializeField] WeaponConfig defaultWeapon = null;
 
        Health target;
        Health health;
        Mover mover;
        Animator animator;
        CapsuleCollider capsuleCollider;
        ActionScheduler actionScheduler;
        BaseStats baseStats;
        float timeSinceLastAttack = 100.0f;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;
        Weapon CurrentWeapon { get => currentWeapon.value; set => currentWeapon.value = value; }

        private void Awake()
        {
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
            animator = GetComponent<Animator>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            actionScheduler = GetComponent<ActionScheduler>();
            baseStats = GetComponent<BaseStats>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        void Start()
        {
            AttachWeapon(currentWeaponConfig);
            currentWeapon.ForceInit();
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
                if (currentWeaponConfig && Vector3.Distance(transform.position, target.transform.position) <= currentWeaponConfig.GetRange())
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
            actionScheduler.StartAction(this);
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

        public void EquipWeapon(WeaponConfig weapon)
        {
            if (weapon && weapon != currentWeaponConfig && rightHandTransform)
            {
                CurrentWeapon = AttachWeapon(weapon);
            }
            currentWeaponConfig = weapon;
        }

        Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
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
            mover.Cancel();
            target = null;
        }

        public IEnumerable<float> GetAdditiveModifers(Stats.Stats stat)
        {
            if (stat == Stats.Stats.Damage)
            {
                yield return currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifers(Stats.Stats stat)
        {
            if (stat == Stats.Stats.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonus();
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
            if (animator)
            {
                animator.ResetTrigger("stopAttack");
                animator.SetTrigger("attack");
            }
        }

        // Animation event
        void Hit()
        {
            if (target && !target.isDead && currentWeaponConfig)
            {
                float damage = baseStats.GetStat(Stats.Stats.Damage);

                if (CurrentWeapon != null)
                {
                    CurrentWeapon.OnHit();
                }

                if (currentWeaponConfig.HasProjectile())
                {
                    currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, capsuleCollider, damage);
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
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(state as string);
            if (weapon)
            {
                EquipWeapon(weapon);
            }
        }

        Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }
    }
}