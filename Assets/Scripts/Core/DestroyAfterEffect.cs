using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        ParticleSystem partSystem = null;
        [SerializeField] GameObject targetToDestroy = null;

        void Awake()
        {
            partSystem = GetComponent<ParticleSystem>();
        }

        void Update()
        {
            if (partSystem && !partSystem.IsAlive())
            {
                if (targetToDestroy)
                {
                    Destroy(targetToDestroy);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}