namespace EFTConfiguration.AcceptableValue
{
    /// <summary>
    ///     Can be Modify Acceptable Values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AcceptableValueCustomList<T> : BepInEx.Configuration.AcceptableValueList<T>
        where T : System.IEquatable<T>
    {
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        [System.Obsolete("Used AcceptableValuesCustom", true)]
        public override T[] AcceptableValues => AcceptableValuesCustom;
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

        public T[] AcceptableValuesCustom
        {
            get => _acceptableValuesCustom;
            set
            {
                if (_acceptableValuesCustom == null)
                {
                    throw new System.ArgumentNullException(nameof(_acceptableValuesCustom));
                }

                _acceptableValuesCustom = value.Length > 0
                    ? value
                    : throw new System.ArgumentException("At least one acceptable value is needed",
                        nameof(_acceptableValuesCustom));
            }
        }

        private T[] _acceptableValuesCustom;

        public AcceptableValueCustomList(params T[] acceptableValues) : base(acceptableValues)
        {
            //Base constructor already checked
            _acceptableValuesCustom = acceptableValues;
        }
    }
}