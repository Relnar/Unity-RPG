using System.Collections.Generic;
using RPG.Core;
using RPG.Resource;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField]
        float maxSpeed = 6.0f;

        NavMeshAgent navMeshAgent;
        Health health;

        void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            navMeshAgent.enabled = !health.isDead;
            UpdateAnimator();
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            if (navMeshAgent)
            {
                navMeshAgent.destination = destination;
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            }
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void Cancel()
        {
            if (navMeshAgent)
            {
                navMeshAgent.isStopped = true;
            }
        }

        void UpdateAnimator()
        {
            if (navMeshAgent &&
                TryGetComponent(out Animator animator))
            {
                Vector3 velocity = navMeshAgent.velocity;
                Vector3 localVelocity = transform.InverseTransformDirection(velocity);
                float speed = localVelocity.z;
                animator.SetFloat("forwardSpeed", speed);
            }
        }

        public object CaptureState()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.rotation.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            if (state is Dictionary<string, object>)
            {
                navMeshAgent.enabled = false;

                Dictionary<string, object> data = (Dictionary<string, object>)state;
                transform.position = ((SerializableVector3)data["position"]).ToVector();
                transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();

                navMeshAgent.enabled = true;
            }
        }
    }
}