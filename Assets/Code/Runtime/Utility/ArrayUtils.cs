using System;

namespace TowerDefense.Runtime.Utility
{
    public class ArrayUtils
    {
        public static void Sort<T>(T[] array, Func<T, float> getScore)
        {
            for (var i = 0; i < array.Length - 1; i++)
            {
                for (var j = 1; j < array.Length - i; j++)
                {
                    var a = array[j - 1];
                    var b = array[j];
                    if (getScore(a) > getScore(b))
                    {
                        array[j - 1] = b;
                        array[j] = a;
                    }
                }
            }
        }
    }
}