using System.Collections.Generic;

namespace RPG.Stats
{
    public interface IModifierProvider
    {
        IEnumerable<float> GetAdditiveModifers(Stats stat);
        IEnumerable<float> GetPercentageModifers(Stats stat);
    }
}
