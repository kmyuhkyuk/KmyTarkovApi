namespace EFTConfiguration.AcceptableValue
{
    // ReSharper disable ConvertToAutoPropertyWhenPossible
    // ReSharper disable UnusedType.Global
    // ReSharper disable UnusedMember.Global
    /// <summary>
    ///     Can be modified Range Value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AcceptableValueCustomRange<T> : BepInEx.Configuration.AcceptableValueRange<T>
        where T : System.IComparable
    {
        [System.Obsolete("Used MinValueCustom", true)]
        public override T MinValue => MinValueCustom;

        [System.Obsolete("Used MaxValueCustom", true)]
        public override T MaxValue => MaxValueCustom;

        public T MinValueCustom
        {
            get => minValueCustom;
            set
            {
                if (value == null)
                {
                    throw new System.ArgumentNullException(nameof(value));
                }

                minValueCustom = value.CompareTo(maxValueCustom) < 0
                    ? value
                    : throw new System.ArgumentException("minValue has to be lower than maxValue");
            }
        }

        private T minValueCustom;

        public T MaxValueCustom
        {
            get => maxValueCustom;
            set => maxValueCustom = value;
        }

        private T maxValueCustom;

        public AcceptableValueCustomRange(T minValue, T maxValue) : base(minValue, maxValue)
        {
            //Base constructor already checked
            maxValueCustom = maxValue;
            minValueCustom = minValue;
        }
    }
}