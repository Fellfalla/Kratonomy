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
            var random = new Random();
            
            int index = random.Next(0, enumerable.Count());
            return enumerable.ToArray()[index];
        }
    }
}
