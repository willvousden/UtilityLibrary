namespace UtilityLibrary.Cryptography
{
    /// <summary>
    /// Specifies methods for encrypting and decrypting data.
    /// </summary>
    public interface ICipher
    {
        /// <summary>
        /// Gets a value indicating whether the current <see cref="UtilityLibrary.Cryptography.ICipher"/>
        /// can decrypt data.
        /// </summary>
        bool CanDecrypt
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the current <see cref="UtilityLibrary.Cryptography.ICipher"/>
        /// can encrypt data.
        /// </summary>
        bool CanEncrypt
        {
            get;
        }

        /// <summary>
        /// Encrypts the given plain text data.
        /// </summary>
        /// <param name="plainText">The plain text data to be encrypted.</param>
        /// <returns>The cipher text data.</returns>
        byte[] Encrypt(byte[] plainText);

        /// <summary>
        /// Decrypts the given cipher text data.
        /// </summary>
        /// <param name="cipherText">The cipher text data to be decrypted.</param>
        /// <returns>The plain text data.</returns>
        byte[] Decrypt(byte[] cipherText);
    }
}