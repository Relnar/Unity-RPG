using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        float speed = 1.0f;

        [SerializeField]
        bool isHoming = false;

        Health target = null;
        float damage = 0.0f;
        BoxCollider projectileCollider = null;

        private void Start()
        {
            projectileCollider = GetComponent<BoxCollider>();
        }
        
        // Update is called once per frame
        void Update()
        {
            if (target)
            {
                if (isHoming && !target.isDead)
                {
                    transform.LookAt(GetAimLocation());
                }
                transform.Translate(Vector3.forward * speed * Time.deltaTime);

                // Destroy if outside the frustum
                if (!IsVisibleFromMainCamera())
                {
                    Destroy(gameObject);
                }
            }
        }

        public void SetTarget(Health target, float damage, CapsuleCollider capsuleCollider)
        {
            this.target = target;
            this.damage = damage;

            // Change the transform in the target direction
            if (target)
            {
                transform.LookAt(GetAimLocation());
            }
            if (capsuleCollider)
            {
                Physics.IgnoreCollision(GetComponent<BoxCollider>(), capsuleCollider);
            }
        }

        bool IsVisibleFromMainCamera()
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            return GeometryUtility.TestPlanesAABB(planes, projectileCollider.bounds);
        }

        private Vector3 GetAimLocation()
        {
            Vector3 aimLocation = target.transform.position;
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule)
            {
                // Adjust the aim location to be at the target mid height
                aimLocation += Vector3.up * targetCapsule.height * 0.5f;
            }
            return aimLocation;
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Friendly fire
            Health collidingTarget = collision.collider.GetComponent<Health>();
            if (collidingTarget)
            {
                if (!collidingTarget.isDead)
                {
                    collidingTarget.TakeDamage(damage);
                    if (collidingTarget.isDead)
                    {
                        // Disable collider, so that the next projectile launched will continue through
                        collision.collider.enabled = false;
                    }
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}