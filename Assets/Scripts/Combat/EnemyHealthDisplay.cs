using System;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter target;
        Text textDisplay = null;

        private void Awake()
        {
            target = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            textDisplay = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            Health health = target.GetTarget();
            textDisplay.text = health ? String.Format($"{health.CurrentHealth}/{health.MaxHealth} {health.GetPercentage():0}%") : "N/A";
        }
    }
}
