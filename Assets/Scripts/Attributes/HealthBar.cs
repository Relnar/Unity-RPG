using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas rootCanvas;

        void Update()
        {
            float scale = healthComponent.CurrentHealth / healthComponent.MaxHealth;
            if (rootCanvas &&
                (Mathf.Approximately(scale, 0.0f) ||
                 Mathf.Approximately(scale, 1.0f)))
            {
                rootCanvas.enabled = false;
            }
            else if (foreground && healthComponent)
            {
                rootCanvas.enabled = true;
                foreground.localScale = new Vector3(scale, 1.0f, 1.0f);
            }
        }
    }
}
