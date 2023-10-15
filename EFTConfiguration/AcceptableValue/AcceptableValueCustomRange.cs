namespace EFTConfiguration.AcceptableValue
{
    // ReSharper disable UnusedType.Global
    /// <summary>
    ///     Can be Modify Range Value
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
                if (value.CompareTo(maxValueCustom) < 0)
                {
                    minValueCustom = value;
                }
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