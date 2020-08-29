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
                transform.Translate(Vector3.forward * speed * Time.deltaTime);

                // Destroy if outside the frustum
                if (!IsVisibleFromMainCamera())
                {
                    Destroy(gameObject);
                }
            }
        }

        public void SetTarget(Health target, float damage)
        {
            this.target = target;
            this.damage = damage;

            // Change the transform in the target direction
            if (target)
            {
                transform.LookAt(GetAimLocation());
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

        private void OnTriggerEnter(Collider other)
        {
            Health collidingTarget = other.GetComponent<Health>();
            if (collidingTarget == target)
            {
                target.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}