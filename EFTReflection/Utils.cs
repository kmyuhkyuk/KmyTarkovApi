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

        /// <summary>
        ///     Get <see cref="Multiplicity" /> from <see cref="IEnumerable{T}" /> count
        ///     <para>If count is zero then return <see cref="Multiplicity.None" /></para>
        ///     <para>If count is one then return <see cref="Multiplicity.One" /></para>
        ///     <para>If count pass one then return <see cref="Multiplicity.Many" /></para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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