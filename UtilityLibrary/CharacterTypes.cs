using System;

namespace UtilityLibrary
{
    /// <summary>
    /// Specifies the possible combinations of character types.
    /// </summary>
    [Flags]
    public enum CharacterTypes
    {
        /// <summary>
        /// Specifies all lower case letters.
        /// </summary>
        LowerCaseLetters = 0x1,

        /// <summary>
        /// Specifies all upper case letters.
        /// </summary>
        UpperCaseLetters = 0x2,

        /// <summary>
        /// Specifies all digits.
        /// </summary>
        Digits = 0x4,

        /// <summary>
        /// Specifies all non-alphanumeric characters.
        /// </summary>
        Symbols = 0x8,

        /// <summary>
        /// Specifies the space character.
        /// </summary>
        Spaces = 0x10,

        /// <summary>
        /// Specifies both upper and lower case characters.
        /// </summary>
        AllLetters = LowerCaseLetters | UpperCaseLetters,

        /// <summary>
        /// Specifies lower and upper case characters and digits.
        /// </summary>
        Alphanumeric = AllLetters | Digits,

        /// <summary>
        /// Specifies all characters.
        /// </summary>
        All = LowerCaseLetters | UpperCaseLetters | Digits | Symbols | Spaces
    }
}