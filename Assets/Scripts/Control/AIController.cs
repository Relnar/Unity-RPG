using GameDevTV.Utils;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField]
        float chaseDistance = 5.0f;

        [SerializeField]
        float suspicionTime = 5.0f;

        [SerializeField]
        PatrolPath patrolPath = null;

        [Range(0, 1)]
        [SerializeField]
        float patrolSpeedFraction = 0.2f;

        [SerializeField]
        float waypointTolerance = 1.0f;
        int waypointIndex = 0;
        [SerializeField]
        float waypointDwellTime = 1.0f;

        Fighter fighter;
        Mover mover;
        GameObject player;
        Health health;
        ActionScheduler actionScheduler;
        LazyValue<Vector3> guardPosition;
        Vector3 GuardPosition { get => guardPosition.value; set => guardPosition.value = value; }

        float timeSinceSawPlayer = Mathf.Infinity;
        float timeSinceAtWaypoint = 0.0f;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");
            health = fighter.GetComponent<Health>();
            actionScheduler = GetComponent<ActionScheduler>();
            guardPosition = new LazyValue<Vector3>(SetupDefaultGuardPosition);
        }

        private void Start()
        {
            guardPosition.ForceInit();
        }

        void Update()
        {
            if (health.isDead)
            {
                return;
            }

            if (Vector3.Distance(player.transform.position, transform.position) <= chaseDistance &&
                fighter.CanAttack(player))
            {
                timeSinceSawPlayer = 0.0f;
                AttackBehavior();
            }
            else if (timeSinceSawPlayer < suspicionTime)
            {
                // Suspicion state
                timeSinceSawPlayer += Time.deltaTime;
                SuspicionBehavior();
            }
            else
            {
                PatrolBehavior();
            }
        }

        private void AttackBehavior()
        {
            fighter.Attack(player);
        }

        private void SuspicionBehavior()
        {
            actionScheduler.CancelCurrentAction();
        }

        private void PatrolBehavior()
        {
            Vector3 nextPosition = GuardPosition;

            // Move along the patrol path
            if (patrolPath && patrolPath.waypointCount > 1)
            {
                nextPosition = patrolPath.GetWaypoint(waypointIndex);

                // Determine if near the next waypoint
                if (Vector3.Distance(transform.position, nextPosition) < waypointTolerance)
                {
                    if (timeSinceAtWaypoint > waypointDwellTime)
                    {
                        // Cycle to the next waypoint
                        waypointIndex = (waypointIndex + 1) % patrolPath.waypointCount;
                        nextPosition = patrolPath.GetWaypoint(waypointIndex);
                        timeSinceAtWaypoint = 0.0f;
                    }
                    else
                    {
                        timeSinceAtWaypoint += Time.deltaTime;
                    }
                }
            }

            mover.StartMoveAction(nextPosition, patrolSpeedFraction);
        }

        // Called by Unity
        private void OnDrawGizmosSelected()
        {
            Color oldColor = Gizmos.color;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
            Gizmos.color = oldColor;

            if (patrolPath)
            {
                patrolPath.OnDrawGizmosSelected();
            }
        }

        Vector3 SetupDefaultGuardPosition()
        {
            return transform.position;
        }
    }
}
