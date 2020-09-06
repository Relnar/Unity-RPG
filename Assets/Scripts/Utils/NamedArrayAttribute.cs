using UnityEngine;

namespace RPG.Utils
{
    public class NamedArrayAttribute : PropertyAttribute
    {
        public string name;
        public NamedArrayAttribute(string name)
        {
            this.name = name;
        }
    }
}