using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Resource
{
    public class Experience : MonoBehaviour
    {
        [SerializeField] int experiencePoints = 0;

        public void GainExperience(int experience)
        {
            experiencePoints += experience;
        }
    }
}
