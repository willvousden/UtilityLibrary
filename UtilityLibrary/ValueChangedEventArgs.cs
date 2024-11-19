using System;

namespace UtilityLibrary
{
    /// <summary>
    /// Contains event data pertaining to a changed value.
    /// </summary>
    /// <typeparam name="T">The type of the value that has changed.</typeparam>
    public class ValueChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Gets the encapsulated previous value.
        /// </summary>
        public T OldValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the encapsulated new value.
        /// </summary>
        public T NewValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new <see cref="UtilityLibrary.ValueChangedEventArgs{T}"/> instance.
        /// </summary>
        /// <param name="oldValue">The old value to encapsulate.</param>
        /// <param name="newValue">The new value to encapsulate.</param>
        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}