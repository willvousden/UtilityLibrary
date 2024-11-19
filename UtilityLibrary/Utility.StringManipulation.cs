using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UtilityLibrary
{
    /// <summary>
    /// Contains commonly used properties and methods.
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// Combines a number of path segments into a single path.
        /// </summary>
        /// <param name="segments">The path segments to combine.</param>
        /// <returns>A single path consisting of the individual paths combined into one.</returns>
        public static string CombinePathSegments(params string[] segments)
        {
            // Filter to non-null and non-empty segments.
            string[] usefulSegments = Array.FindAll(segments, segment => !string.IsNullOrEmpty(segment));

            StringBuilder pathBuilder = new StringBuilder();
            for (int i = 0; i < usefulSegments.Length; i++)
            {
                string segment = usefulSegments[i];

                // Discard everything up until now if we encounter a rooted segment.
                bool isRooted = Path.IsPathRooted(segment);
                if (isRooted)
                {
                    pathBuilder.Remove(0, pathBuilder.Length);
                }
                else
                {
                    // Append a directory separator to the path builder if there isn't already one
                    // there.
                    char previous = pathBuilder[pathBuilder.Length - 1];
                    if (pathBuilder.Length > 0 && previous != Path.DirectorySeparatorChar && 
                        previous != Path.AltDirectorySeparatorChar)
                    {
                        pathBuilder.Append(Path.DirectorySeparatorChar);
                    }
                }

                pathBuilder.Append(segment);
            }

            return pathBuilder.ToString();
        }

        /// <summary>
        /// Generates a string of random characters of the specified length.
        /// </summary>
        /// <param name="length">The length of the string to be generated.</param>
        /// <param name="characterTypes">The character classes that should be used when generating the string.</param>
        /// <returns>A string of random characters.</returns>
        public static string GenerateRandomString(int length, CharacterTypes characterTypes)
        {
            char[] characters = GetCharacters(characterTypes);
            return GenerateRandomString(length, characters);
        }

        /// <summary>
        /// Generates a string of random characters of the specified length.
        /// </summary>
        /// <param name="length">The length of the string to be generated.</param>
        /// <param name="characters">The characters that should be used when generating the string.</param>
        /// <returns>A string of random characters.</returns>
        public static string GenerateRandomString(int length, params char[] characters)
        {
            // Remove duplicate characters from character list.
            List<char> uniqueCharacters = new List<char>();
            foreach (char character in characters)
            {
                bool isAdded = uniqueCharacters.Contains(character);
                if (!isAdded)
                {
                    uniqueCharacters.Add(character);
                }
            }
            characters = uniqueCharacters.ToArray();

            StringBuilder randomStringBuilder = new StringBuilder(length);
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                int index = random.Next(0, characters.Length);
                randomStringBuilder.Append(characters[index]);
            }

            return randomStringBuilder.ToString();
        }

        /// <summary>
        /// Normalises in size and suffixes a number representing a number of bytes using SI prefixes.
        /// </summary>
        /// <param name="byteCount">The number to format.</param>
        /// <returns>A formatted string representing the given number of bytes, with the appropriate SI prefix.</returns>
        public static string FormatByteCount(long byteCount)
        {
            return FormatByteCount(byteCount, false);
        }

        /// <summary>
        /// Normalises in size and suffixes a number representing a number of bytes.
        /// </summary>
        /// <param name="byteCount">The number to format.</param>
        /// <param name="useBinaryPrefixes"><c>true</c> to use IEC binary prefixes; <c>false</c> to use SI prefixes.</param>
        /// <returns>A formatted string representing the given number of bytes, with the appropriate SI or IEC binary prefix.</returns>
        public static string FormatByteCount(long byteCount, bool useBinaryPrefixes)
        {
            double size;
            string formatString;
            string prefix = string.Empty;
            double divisor;
            if (byteCount >= 1L << 10)
            {
                formatString = "{0:F2} {1}B";
                if (byteCount >= 1L << 40)
                {
                    prefix = "T";
                    divisor = 1L << 40;
                }
                else if (byteCount >= 1L << 30)
                {
                    prefix = "G";
                    divisor = 1L << 30;
                }
                else if (byteCount >= 1L << 20)
                {
                    prefix = "M";
                    divisor = 1L << 20;
                }
                else
                {
                    prefix = "K";
                    divisor = 1L << 10;
                }
            }
            else
            {
                prefix = string.Empty;
                formatString = "{0} B";
                divisor = 1;
            }

            size = byteCount / divisor;

            if (useBinaryPrefixes && (prefix != string.Empty))
            {
                prefix += "i";
            }
            string formattedString = string.Format(formatString, size, prefix);
            return formattedString;
        }

        /// <summary>
        /// Gets an array of characters represented by the given <see cref="UtilityLibrary.CharacterTypes"/>.
        /// </summary>
        /// <param name="characterTypes">The character classes to be collapsed into an array.</param>
        /// <returns>An array of characters.</returns>
        private static char[] GetCharacters(CharacterTypes characterTypes)
        {
            if ((int)characterTypes == 0)
            {
                throw new ArgumentException();
            }

            List<char> characters = new List<char>();

            if ((characterTypes & CharacterTypes.Digits) == CharacterTypes.Digits)
            {
                for (int i = 0; i < 10; i++)
                {
                    characters.Add(Convert.ToChar(i + 48));
                }
            }

            if ((characterTypes & CharacterTypes.UpperCaseLetters) == CharacterTypes.UpperCaseLetters)
            {
                for (int i = 0; i < 26; i++)
                {
                    characters.Add(Convert.ToChar(i + 65));
                }
            }

            if ((characterTypes & CharacterTypes.LowerCaseLetters) == CharacterTypes.LowerCaseLetters)
            {
                for (int i = 0; i < 26; i++)
                {
                    characters.Add(Convert.ToChar(i + 97));
                }
            }

            if ((characterTypes & CharacterTypes.Symbols) == CharacterTypes.Symbols)
            {
                for (int i = 33; i < 48; i++)
                {
                    characters.Add(Convert.ToChar(i));
                }

                for (int i = 58; i < 65; i++)
                {
                    characters.Add(Convert.ToChar(i));
                }

                for (int i = 91; i < 97; i++)
                {
                    characters.Add(Convert.ToChar(i));
                }

                for (int i = 123; i < 127; i++)
                {
                    characters.Add(Convert.ToChar(i));
                }
            }

            if ((characterTypes & CharacterTypes.Spaces) == CharacterTypes.Spaces)
            {
                characters.Add(' ');
            }

            return characters.ToArray();
        }
    }
}