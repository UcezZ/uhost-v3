namespace Uhost.Core.Common
{
    /// <summary>
    /// Represents value pair
    /// </summary>
    /// <typeparam name="T1">Value 1 type</typeparam>
    /// <typeparam name="T2">Value 2 type</typeparam>
    public struct ValuePair<T1, T2>
    {
        /// <summary>
        /// Value 1
        /// </summary>
        public T1 Value1 { get; set; }

        /// <summary>
        /// Value 2
        /// </summary>
        public T2 Value2 { get; set; }

        public ValuePair(T1 value1, T2 value2)
        {
            Value1 = value1;
            Value2 = value2;
        }
    }
}
