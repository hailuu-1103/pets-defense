namespace Utils
{
    using System.Collections.Generic;
    using System.Linq;

    public static class Utility
    {
        public static string ValueToKey(Dictionary<string, int> dictionary, int value)
        {
            return (from kvp in dictionary where kvp.Value == value select kvp.Key).FirstOrDefault();
        }
    }
}