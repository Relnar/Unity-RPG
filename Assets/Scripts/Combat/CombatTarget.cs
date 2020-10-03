using RPG.Control;
using RPG.Resource;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public bool HandleRaycast(PlayerController callingController)
        {
            Fighter fighter = callingController.GetComponent<Fighter>();
            if (fighter.CanAttack(gameObject))
            {
                // Mouse button click only
                if (Input.GetMouseButtonDown(0))
                {
                    fighter.Attack(gameObject);
                }
                return true;
            }
            return false;
        }
    }
}
