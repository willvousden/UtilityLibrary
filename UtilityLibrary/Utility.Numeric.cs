using System;
using System.IO;
using Microsoft.Win32;

namespace UtilityLibrary
{
    /// <summary>
    /// Contains commonly used properties and methods.
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// Rounds a number to a given number of significant figures. If the number is half way between two other numbers, it is rounded towards the even one.
        /// </summary>
        /// <param name="number">A double-precision floating point number to round.</param>
        /// <param name="figures">The number of significant figures to which to round.</param>
        /// <returns>The rounded number.</returns>
        public static double RoundToSignificantFigures(double number, int figures)
        {
            return RoundToSignificantFigures(number, figures, MidpointRounding.ToEven);
        }

        /// <summary>
        /// Rounds a number to a given number of significant figures.
        /// </summary>
        /// <param name="number">A double-precision floating point number to round.</param>
        /// <param name="figures">The number of significant figures to which to round.</param>
        /// <param name="mode">How the number should be rounded if it is half way between two other numbers.</param>
        /// <returns>The rounded number.</returns>
        public static double RoundToSignificantFigures(double number, int figures, MidpointRounding mode)
        {
            // Find the magnitude of the given number (in base 10).
            int magnitude = (int)Math.Ceiling(Math.Log10(Math.Abs(number)));
            double magnitudeCoefficient = Math.Pow(10, magnitude - 1);
            double roundedNumber = number;

            // Normalise the number's magnitude.
            roundedNumber /= magnitudeCoefficient;

            // Round to however many digits.
            roundedNumber = Math.Round((double)roundedNumber, figures - 1, mode);

            // Multiply back to the number's original magnitude.
            roundedNumber *= magnitudeCoefficient;

            return roundedNumber;
        }

        /// <summary>
        /// Rounds a number to a given number of significant figures. If the number is half way between two other numbers, it is rounded towards the even one.
        /// </summary>
        /// <param name="number">A decimal number to round.</param>
        /// <param name="figures">The number of significant figures to which to round.</param>
        /// <returns>The rounded number.</returns>
        public static decimal RoundToSignificantFigures(decimal number, int figures)
        {
            return RoundToSignificantFigures(number, figures, MidpointRounding.ToEven);
        }

        /// <summary>
        /// Rounds a number to a given number of significant figures.
        /// </summary>
        /// <param name="number">A decimal number to round.</param>
        /// <param name="figures">The number of significant figures to which to round.</param>
        /// <param name="mode">How the number should be rounded if it is half way between two other numbers.</param>
        /// <returns>The rounded number.</returns>
        public static decimal RoundToSignificantFigures(decimal number, int figures, MidpointRounding mode)
        {
            // Find the magnitude of the given number (in base 10).
            int magnitude = (int)Math.Ceiling(Math.Log10(Math.Abs((double)number)));
            decimal roundedNumber = number;

            // Calculate magnitude coefficient the long way — don't use Math.Pow as it returns a
            // floating point, so precision may be lost.
            decimal magnitudeCoefficient = 1;
            int exponent = 1 - magnitude;
            int exponentAbsolute = Math.Abs(exponent);
            if (exponent > 0)
            {
                exponentAbsolute.Times(() => magnitudeCoefficient /= 10);
            }
            else
            {
                exponentAbsolute.Times(() => magnitudeCoefficient *= 10);
            }

            // Normalise the number's magnitude.
            roundedNumber /= magnitudeCoefficient;

            // Round to however many digits.
            roundedNumber = Math.Round(roundedNumber, figures - 1, mode);

            // Multiply back to the number's original magnitude.
            roundedNumber *= magnitudeCoefficient;

            return roundedNumber;
        }
    }
}