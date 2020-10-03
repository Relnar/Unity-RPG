using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText damageTextPrefab = null;

        // Start is called before the first frame update
        void Start()
        {
            Spawn(11);
        }

        public void Spawn(float damageAmount)
        {
            if (damageTextPrefab)
            {
                DamageText instance = GameObject.Instantiate(damageTextPrefab, transform);
            }
        }
    }
}
