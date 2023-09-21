namespace EFTConfiguration.AcceptableValue
{
    /// <summary>
    ///     Can be Modify Acceptable Values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AcceptableValueCustomList<T> : BepInEx.Configuration.AcceptableValueList<T>
        where T : System.IEquatable<T>
    {
        [System.Obsolete("Used AcceptableValuesCustom", true)]
        public override T[] AcceptableValues => AcceptableValuesCustom;

        public T[] AcceptableValuesCustom
        {
            get => acceptableValuesCustom;
            set
            {
                if (acceptableValuesCustom.Length != 0)
                {
                    acceptableValuesCustom = value;
                }
            }
        }

        private T[] acceptableValuesCustom;

        public AcceptableValueCustomList(params T[] acceptableValues) : base(acceptableValues)
        {
            //Base constructor already checked
            acceptableValuesCustom = acceptableValues;
        }
    }
}