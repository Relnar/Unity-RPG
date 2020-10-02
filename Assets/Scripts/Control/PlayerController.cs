using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using RPG.Resource;
using RPG.Utils;
using System;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;

        enum CursorType
        {
            None,
            Movement,
            Combat
        }

        [System.Serializable]
        struct CursorMapping
        {
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [EnumNamedArray(typeof(CursorType))]
        [SerializeField] CursorMapping[] cursorMappings = new CursorMapping[Enum.GetValues(typeof(CursorType)).Length];

        // Start is called before the first frame update
        void Awake()
        {
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            if (health.isDead)
            {
                return;
            }

            Ray ray = GetMouseRay();

            if (InteractWithCombat((Ray)ray))
            {
                return;
            }

            if (InteractWithMovement((Ray)ray))
            {
                return;
            }
            SetCursor(CursorType.None);
        }

        bool InteractWithCombat(Ray ray)
        {
            // Raycast all to hit all possible targets in the ray
            var hits = Physics.RaycastAll(ray);
            foreach (var hit in hits)
            {
                if (hit.transform.TryGetComponent(out CombatTarget combatTarget) &&
                    TryGetComponent(out Fighter fighter) &&
                    fighter.CanAttack(combatTarget.gameObject))
                {
                    // Mouse button click only
                    if (Input.GetMouseButtonDown(0))
                    {
                        fighter.Attack(combatTarget.gameObject);
                    }
                    SetCursor(CursorType.Combat);
                    return true;
                }
            }

            return false;
        }

        bool InteractWithMovement(Ray ray)
        {
            // Raycast to hit the first target
            if (Physics.Raycast(ray, out RaycastHit hit) &&
                TryGetComponent(out Mover mover))
            {
                // Mouse button can be held continously
                if (Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(hit.point, 1.0f);
                }
                return true;
            }

            return false;
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
    }
}