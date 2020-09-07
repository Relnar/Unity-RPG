using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] int experiencePoints = 0;

        public int GetExperience()
        {
            return experiencePoints;
        }

        public void GainExperience(int experience)
        {
            experiencePoints += experience;
        }

        public object CaptureState()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            if (state is int)
            {
                experiencePoints = (int)state;
            }
        }
    }
}
