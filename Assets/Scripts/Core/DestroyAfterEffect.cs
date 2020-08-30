using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        ParticleSystem partSystem = null;

        void Start()
        {
            partSystem = GetComponent<ParticleSystem>();
        }

        void Update()
        {
            if (partSystem && !partSystem.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}