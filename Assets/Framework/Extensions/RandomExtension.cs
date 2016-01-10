using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Extensions
{
    public static class RandomExtension
    {
        public static T GetRandomElement<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            if (!enumerable.Any())
            {
                return default(T);
            }

            var random = new Random();

            var castedEnum = enumerable.ToArray();

            int index = random.Next(0, castedEnum.Length);
            return castedEnum[index];
        }

        public static float GetRandomFloat(this Random random, float min = 0, float max = 1)
        {
            var randomValue = random.NextDouble();

            return min + (max - min)* Convert.ToSingle(randomValue);

        }

    }
}
