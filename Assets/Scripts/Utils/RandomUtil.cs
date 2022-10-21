namespace Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = System.Random;

    public static class RandomUtil
    {
        private static Random random = new();

        public static T RandomEnum<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(random.Next(v.Length));
        }

        public static bool RandomBoolean()
        {
            var randomBool = random.Next(2);
            return randomBool != 0;
        }
        private static readonly Random rd = new();

        public static int GetRandomValueExceptValues(ICollection<int> exclude, int minValue, int maxValue)
        {
            var range = Enumerable.Range(minValue, maxValue).Where(i => !exclude.Contains(i));
            var index = GetRandomValueInRange(minValue, maxValue - exclude.Count);
            return range.ElementAt(index);
        }
        public static int GetRandomValueExcept(int value, int minValue, int maxValue)
        {
            return GetRandomValueExceptValues(new[] { value }, minValue, maxValue);
        }

        public static int[] AddRandomValueToArray(int size, int minValue, int maxValue)
        {
            var array = new int[size];
            for (var i = 0; i < size; i++)
            {
                var random = GetRandomValueInRange(minValue, maxValue);
                array[i] = random;
            }

            return array;
        }

        public static List<int> GetIndexesOfValueFromArray(int[] arr, int value)
        {
            var result = new List<int>();
            for (var i = 0; i < arr.Length; i++)
                if (arr[i] == value)
                    result.Add(i);
            return result;
        }

        public static int GetRandomValueInRange(int minValue, int maxValue) { return rd.Next(minValue, maxValue); }
        public static double GetRandomDoubleNumber(double minValue, double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }

        /// <summary>
        ///     Make sure 3 consecutive tile have not a same type
        /// </summary>
        /// <param name="array">Array need to allocate</param>
        /// <param name="minValue">Min number of types</param>
        /// <param name="maxValue">Max number of types</param>
        public static void AllocateMemberInArray(int[] array, int minValue, int maxValue)
        {
            for (var i = 0; i < array.Length; i++)
            {
                if (i - 2 > 0 && array[i - 2] == array[i - 1] && array[i - 1] == array[i])
                    array[i] = GetRandomValueExcept(array[i - 2], minValue, maxValue);
                if (i + 1 < array.Length - 2 && array[i] == array[i + 1] && array[i + 1] == array[i + 2])
                    array[i + 2] = GetRandomValueExcept(array[i], minValue, maxValue);
            }
        }
        public static string GetRandomValueInList(List<string> list)
        {
            return (from value in list
                let index = rd.Next(list.Count)
                select list[index]).FirstOrDefault();
        }
        public static string RandomElementWithPercentage(List<KeyValuePair<string, double>> elementToPercentage)
        {
            var    diceRoll   = random.NextDouble();
            var cumulative = 0.0;
            for (var i = 0; i < elementToPercentage.Count; i++)
            {
                cumulative += elementToPercentage[i].Value;
                if(diceRoll < cumulative) return elementToPercentage[i].Key;
            }
            return null;
        }
    }
}