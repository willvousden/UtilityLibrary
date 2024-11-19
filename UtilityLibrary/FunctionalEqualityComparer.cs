using System;
using System.Collections.Generic;

namespace UtilityLibrary
{
    /// <summary>
    /// Implements comparison of values via a comparison callback.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public class FunctionalEqualityComparer<T> : IEqualityComparer<T>
    {
        private Func<T, T, bool> m_Comparison;

        /// <summary>
        /// Initializes a new <see cref="UtilityLibrary.FunctionalEqualityComparer{T}"/> instance.
        /// </summary>
        /// <param name="comparison">A comparison callback with which to compare values.</param>
        public FunctionalEqualityComparer(Func<T, T, bool> comparison)
        {
            m_Comparison = comparison;
        }

        #region IEqualityComparer<T> Members

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            return m_Comparison(x, y);
        }

        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        #endregion
    }
}