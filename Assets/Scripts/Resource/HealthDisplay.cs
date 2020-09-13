using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Resource
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health;
        Text textDisplay = null;

        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            textDisplay = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            textDisplay.text = $"{health.CurrentHealth}/{health.MaxHealth} {health.GetPercentage():0}%";
        }
    }
}
