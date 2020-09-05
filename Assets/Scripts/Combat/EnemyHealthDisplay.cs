using System;
using RPG.Resource;
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
            textDisplay.text = target.GetTarget() ? String.Format("{0:0} %", target.GetTarget().GetPercentage()) : "N/A";
        }
    }
}
