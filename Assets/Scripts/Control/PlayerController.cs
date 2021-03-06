﻿using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using RPG.Attributes;
using RPG.Utils;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;
        Fighter fighter;
        Mover mover;

        [System.Serializable]
        struct CursorMapping
        {
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [EnumNamedArray(typeof(CursorType))]
        [SerializeField] CursorMapping[] cursorMappings = new CursorMapping[Enum.GetValues(typeof(CursorType)).Length];

        [SerializeField] float maxNavMeshProjectionDistance = 1.0f;
        [SerializeField] float maxNavPathLength = 40.0f;

        // Start is called before the first frame update
        void Awake()
        {
            health = GetComponent<Health>();
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            for (int i = 0; i < cursorMappings.Length; ++i)
            {
                if (cursorMappings[i].texture && cursorMappings[i].hotspot == Vector2.zero)
                {
                    cursorMappings[i].hotspot = new Vector2(cursorMappings[i].texture.width / 2,
                                                            cursorMappings[i].texture.height / 2);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (InteractWithUI())
            {
                return;
            }

            if (health.isDead)
            {
                SetCursor(CursorType.None);
                return;
            }

            Ray ray = GetMouseRay();

            if (InteractWithComponent(ray))
            {
                return;
            }

            if (InteractWithMovement(ray))
            {
                return;
            }
            SetCursor(CursorType.None);
        }

        bool InteractWithUI()
        {
            if (EventSystem.current &&
                EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        private bool InteractWithComponent(Ray ray)
        {
            // Raycast all to hit all possible targets in the ray
            var hits = RaycastAllSorted(ray);
            foreach (var hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        bool InteractWithMovement(Ray ray)
        {
            // Raycast to hit the first target
            Vector3 target;
            if (RaycastNavMesh(ray, out target))
            {
                // Mouse button can be held continously
                if (Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(target, 1.0f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }

            return false;
        }

        RaycastHit[] RaycastAllSorted(Ray ray)
        {
            var hits = Physics.RaycastAll(ray);
            float[] distances = new float[hits.Length];
            for (int i = 0; i < distances.Length; ++i)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }

        private static Ray GetMouseRay()
        {
            // Convert mouse position to a ray
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        CursorMapping GetCursorMapping(CursorType type)
        {
            return cursorMappings[(int)type];
        }

        bool RaycastNavMesh(Ray ray, out Vector3 target)
        {
            // Raycast to terrain
            target = new Vector3();
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Find the nearest position on the navmesh
                NavMeshHit navMeshHit;
                if (NavMesh.SamplePosition(hit.point,
                                           out navMeshHit,
                                           maxNavMeshProjectionDistance,
                                           NavMesh.AllAreas))
                {
                    target = navMeshHit.position;

                    NavMeshPath path = new NavMeshPath();
                    if (NavMesh.CalculatePath(transform.position, navMeshHit.position, NavMesh.AllAreas, path) &&
                        path.status == NavMeshPathStatus.PathComplete &&
                        GetPathLength(path) <= maxNavPathLength)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float totalLength = 0.0f;
            if (path.corners.Length > 1)
            {
                for (int i = 0; i < path.corners.Length - 1; ++i)
                {
                    totalLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
                }
            }
            return totalLength;
        }
    }
}