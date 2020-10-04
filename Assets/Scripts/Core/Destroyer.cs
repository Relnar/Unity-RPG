using UnityEngine;

namespace RPG.Core
{
    public class Destroyer : MonoBehaviour
    {
        [SerializeField] GameObject targetToDestroy = null;

        public void DestroyTarget()
        {
            if (targetToDestroy)
            {
                Destroy(targetToDestroy);
            }
        }
    }
}
