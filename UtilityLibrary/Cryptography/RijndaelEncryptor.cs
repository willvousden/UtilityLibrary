using System;
using System.IO;
using System.Security.Cryptography;

namespace UtilityLibrary.Cryptography
{
    /// <summary>
    /// Handles symmetric encryption and decryption with the Rijndael algorithm.
    /// </summary>
    public sealed class RijndaelEncryptor : ICipher
    {
        private const int BitsPerByte = 8;

        private RijndaelManaged m_Rijndael;
        private int m_KeySize;

        /// <summary>
        /// Gets or sets the size, in bits, of the key to be used by the algorithm.
        /// </summary>
        public int KeySize
        {
            get
            {
                return m_KeySize;
            }
            set
            {
                m_KeySize = value;
            }
        }

        /// <summary>
        /// Gets or sets the bytes to be used for the IV (initialisation vector) for encryption and decryption.
        /// </summary>
        public byte[] IV
        {
            get
            {
                return m_Rijndael.IV;
            }
            set
            {
                m_Rijndael.IV = value;
            }
        }

        /// <summary>
        /// Gets or sets the bytes to be used as the key for encryption and decryption.
        /// </summary>
        public byte[] Key
        {
            get
            {
                return m_Rijndael.Key;
            }
            set
            {
                m_Rijndael.Key = value;
            }
        }

        /// <summary>
        /// Gets or sets the cipher mode to be used for encryption and decryption.
        /// </summary>
        public CipherMode CipherMode
        {
            get
            {
                return m_Rijndael.Mode;
            }
            set
            {
                m_Rijndael.Mode = value;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="UtilityLibrary.Cryptography.RijndaelEncryptor"/> instance and generates a key and initialisation vector to be used for encryption and decryption.
        /// </summary>
        /// <param name="keySize">The size, in bits, of the key to be used for encryption and decryption.</param>
        public RijndaelEncryptor(int keySize)
        {
            m_Rijndael = new RijndaelManaged();
            m_Rijndael.Mode = CipherMode.CBC;
            m_Rijndael.KeySize = keySize;
            m_KeySize = keySize;
        }

        /// <summary>
        /// Initializes a new <see cref="UtilityLibrary.Cryptography.RijndaelEncryptor"/> instance and generates an initialisation vector to be used for encryption and decryption.
        /// </summary>
        /// <param name="key">The secret key to use for encryption and decryption.</param>
        public RijndaelEncryptor(byte[] key)
            : this(key.Length * BitsPerByte)
        {
            m_Rijndael.Key = key;
        }

        /// <summary>
        /// Initializes a new <see cref="UtilityLibrary.Cryptography.RijndaelEncryptor"/> instance.
        /// </summary>
        /// <param name="iv">The IV (initialisation vector) to use for encryption and decryption.</param>
        /// <param name="key">The secret key to use for encryption and decryption.</param>
        public RijndaelEncryptor(byte[] iv, byte[] key)
            : this(key.Length * BitsPerByte)
        {
            m_Rijndael.IV = iv;
            m_Rijndael.Key = key;
        }

        /// <summary>
        /// Generates an IV (initialisation vector).
        /// </summary>
        public void GenerateIV()
        {
            m_Rijndael.GenerateIV();
        }

        /// <summary>
        /// Generates a new key.
        /// </summary>
        public void GenerateKey()
        {
            m_Rijndael.GenerateKey();
        }

        #region ICipher Members

        /// <summary>
        /// Gets a value indicating whether the current <see cref="UtilityLibrary.Cryptography.RijndaelEncryptor"/> can decrypt data. This will always
        /// return true
        /// </summary>
        public bool CanDecrypt
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current <see cref="UtilityLibrary.Cryptography.RijndaelEncryptor"/> can encrypt data. This will always
        /// return true.
        /// </summary>
        public bool CanEncrypt
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Encrypts the given data using the current key and IV.
        /// </summary>
        /// <param name="plainText">The plain-text data to be encrypted.</param>
        /// <returns>The cipher-text result.</returns>
        public byte[] Encrypt(byte[] plainText)
        {
            using (ICryptoTransform encryptor = m_Rijndael.CreateEncryptor())
            using (MemoryStream memoryStream = new MemoryStream())
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(plainText, 0, plainText.Length);
                cryptoStream.FlushFinalBlock();
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Decrypts the given data using the current key and IV.
        /// </summary>
        /// <param name="cipherText">The cipher-text to be decrypted.</param>
        /// <returns>The plain-text result.</returns>
        public byte[] Decrypt(byte[] cipherText)
        {
            using (ICryptoTransform decryptor = m_Rijndael.CreateDecryptor())
            using (MemoryStream memoryStream = new MemoryStream(cipherText))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            {
                byte[] plainTextBuffer = new byte[cipherText.Length];
                int bytesRead = cryptoStream.Read(plainTextBuffer, 0, cipherText.Length);

                byte[] plainText = new byte[bytesRead];
                Array.Copy(plainTextBuffer, plainText, bytesRead);
                return plainText;
            }
        }

        #endregion
    }
}