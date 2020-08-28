using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField]
        AnimatorOverrideController animatorOverride = null;
        [SerializeField]
        GameObject equippedPrefab = null;
        [SerializeField]
        float weaponRange = 2.0f;
        [SerializeField]
        float weaponDamage = 5.0f;
        [SerializeField]
        bool isRightHanded = true;
        [SerializeField]
        Projectile projectile = null;

        public float GetRange() { return weaponRange; }
        public float GetDamage() { return weaponDamage; }
        public bool HasProjectile() { return projectile != null; }

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            if (equippedPrefab)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                if (handTransform)
                {
                    Instantiate(equippedPrefab, handTransform);
                }
            }
            if (animator && animatorOverride)
            {
                animator.runtimeAnimatorController = animatorOverride;
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

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)
        {
            if (projectile)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                if (handTransform)
                {
                    Projectile projectileInstance = Instantiate(projectile,
                                                                handTransform.position,
                                                                Quaternion.identity);
                    projectileInstance.SetTarget(target, weaponDamage);
                }
            }
        }
    }
}
