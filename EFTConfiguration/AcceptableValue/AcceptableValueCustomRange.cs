namespace EFTConfiguration.AcceptableValue
{
    /// <summary>
    ///     Can be Modify Range Value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AcceptableValueCustomRange<T> : BepInEx.Configuration.AcceptableValueRange<T>
        where T : System.IComparable
    {
        public override T MinValue => minValueCustom;

        public override T MaxValue => maxValueCustom;

        public T MinValueSet
        {
            set
            {
                if (value.CompareTo(maxValueCustom) < 0)
                {
                    minValueCustom = value;
                }
            }
        }

        private T minValueCustom;

        public T MaxValueSet
        {
            set => maxValueCustom = value;
        }

        private T maxValueCustom;

        public AcceptableValueCustomRange(T minValue, T maxValue) : base(minValue, maxValue)
        {
            maxValueCustom = maxValue;
            minValueCustom = minValue;
        }
    }
}