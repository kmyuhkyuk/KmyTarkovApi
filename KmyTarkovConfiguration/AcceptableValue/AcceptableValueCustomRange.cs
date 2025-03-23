namespace KmyTarkovConfiguration.AcceptableValue
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
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        [System.Obsolete("Used MinValueCustom", true)]
        public override T MinValue => MinValueCustom;

        [System.Obsolete("Used MaxValueCustom", true)]
        public override T MaxValue => MaxValueCustom;
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

        public T MinValueCustom
        {
            get => _minValueCustom;
            set
            {
                if (value == null)
                {
                    throw new System.ArgumentNullException(nameof(value));
                }

                _minValueCustom = value.CompareTo(_maxValueCustom) < 0
                    ? value
                    : throw new System.ArgumentException("minValue has to be lower than maxValue");
            }
        }

        private T _minValueCustom;

        public T MaxValueCustom
        {
            get => _maxValueCustom;
            set => _maxValueCustom = value;
        }

        private T _maxValueCustom;

        public AcceptableValueCustomRange(T minValue, T maxValue) : base(minValue, maxValue)
        {
            //Base constructor already checked
            _maxValueCustom = maxValue;
            _minValueCustom = minValue;
        }
    }
}