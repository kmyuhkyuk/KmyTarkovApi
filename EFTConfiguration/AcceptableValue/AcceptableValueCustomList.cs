namespace EFTConfiguration.AcceptableValue
{
    /// <summary>
    ///     Can be Modify Acceptable Values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AcceptableValueCustomList<T> : BepInEx.Configuration.AcceptableValueList<T>
        where T : System.IEquatable<T>
    {
        public override T[] AcceptableValues => acceptableValuesCustom;

        public T[] AcceptableValuesSet
        {
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
            acceptableValuesCustom = acceptableValues;
        }
    }
}