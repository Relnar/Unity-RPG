using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        const float waypointRadius = 0.2f;

        public void OnDrawGizmosSelected()
        {
            if (transform.childCount <= 0)
            {
                return;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                Vector3 currentPos = transform.GetChild(i).position;

                // Draw line
                if (transform.childCount > 1)
                {
                    Vector3 nextPos = transform.GetChild((i + 1) % transform.childCount).position;
                    Gizmos.DrawLine(currentPos, nextPos);
                }

                // Draw waypoint
                Gizmos.DrawSphere(currentPos, waypointRadius);
            }
        }

        public Vector3 GetWaypoint(int index)
        {
            return transform.GetChild(index).position;
        }

        public int waypointCount { get => transform.childCount; }
    }
}
