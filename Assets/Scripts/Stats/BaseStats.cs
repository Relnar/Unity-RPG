using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass = CharacterClass.Grunt;
        [SerializeField] Progression progression = null;

        public float GetStat(Stats stat)
        {
            return progression ? progression.GetStat(stat, characterClass, startingLevel) : 0.0f;
        }
    }
}
