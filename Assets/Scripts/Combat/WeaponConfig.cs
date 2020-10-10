using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Make New WeaponConfig", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon equippedPrefab = null;
        [SerializeField] float weaponRange = 2.0f;
        [SerializeField] float weaponDamage = 5.0f;
        [SerializeField] float percentageBonus = 0.0f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public float GetRange() { return weaponRange; }
        public float GetDamage() { return weaponDamage; }
        public float GetPercentageBonus() { return percentageBonus; }
        public bool HasProjectile() { return projectile != null; }

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            if (equippedPrefab)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                if (handTransform)
                {
                    Weapon weapon = Instantiate(equippedPrefab, handTransform);
                    weapon.gameObject.name = weaponName;
                }
            }
            if (animator)
            {
                if (animatorOverride)
                {
                    animator.runtimeAnimatorController = animatorOverride;
                }
                else
                {
                    var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
                    if (overrideController)
                    {
                        animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
                    }
                }
            }
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = null;
            if (rightHand)
            {
                oldWeapon = rightHand.Find(weaponName);
            }
            if (oldWeapon == null && leftHand)
            {
                oldWeapon = leftHand.Find(weaponName);
            }

            if (oldWeapon)
            {
                // Make sure not to confuse the new weapon with the old weapon
                oldWeapon.name = "DESTROYING";
                Destroy(oldWeapon.gameObject);
            }
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            if (isRightHanded)
            {
                if (rightHand)
                {
                    return rightHand;
                }
            }
            else
            {
                if (leftHand)
                {
                    return leftHand;
                }
            }
            return null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, CapsuleCollider capsuleCollider, float calculatedDamage)
        {
            if (projectile)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                if (handTransform)
                {
                    Projectile projectileInstance = Instantiate(projectile,
                                                                handTransform.position,
                                                                Quaternion.identity);
                    projectileInstance.SetTarget(target, instigator, calculatedDamage, capsuleCollider);
                }
            }
        }
    }
}
