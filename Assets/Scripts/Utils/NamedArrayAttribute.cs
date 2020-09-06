using UnityEngine;

namespace RPG.Utils
{
    public class NamedArrayAttribute : PropertyAttribute
    {
        public string name;
        public int startIndex;
        public NamedArrayAttribute(string name, int startIndex = 0)
        {
            this.name = name;
            this.startIndex = startIndex;
        }
    }
}