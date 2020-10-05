using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;

        void Update()
        {
            if (foreground && healthComponent)
            {
                float scale = healthComponent.CurrentHealth / healthComponent.MaxHealth;
                foreground.localScale = new Vector3(scale, 1.0f, 1.0f);
            }
        }
    }
}
