using System;
using System.Collections.Generic;
using System.Linq;

namespace EFTReflection
{
    public static class Utils
    {
        public enum Multiplicity
        {
            None,
            One,
            Many
        }

        public static Multiplicity GetMultiplicity<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            var count = enumerable.Take(2).Count();
            switch (count)
            {
                case 0:
                    return Multiplicity.None;
                case 1:
                    return Multiplicity.One;
                case 2:
                    return Multiplicity.Many;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Multiplicity), count, null);
            }
        }
    }
}