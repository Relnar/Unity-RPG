﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Experience experience;
        Text textDisplay = null;

        private void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            textDisplay = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            textDisplay.text = String.Format("{0}", experience.GetExperience());
        }
    }
}
