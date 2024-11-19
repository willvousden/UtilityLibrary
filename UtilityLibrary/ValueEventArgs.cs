using System;

namespace UtilityLibrary
{
    /// <summary>
    /// Contains event data pertaining to value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public class ValueEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Gets the encapsulated value.
        /// </summary>
        public T Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new <see cref="UtilityLibrary.ValueChangedEventArgs{T}"/> instance.
        /// </summary>
        /// <param name="value">The value to encapsulate.</param>
        public ValueEventArgs(T value)
        {
            Value = value;
        }
    }
}