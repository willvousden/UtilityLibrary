using System;
using System.Diagnostics;
using System.Security.Cryptography;
using UtilityLibrary.Properties;

namespace UtilityLibrary.Cryptography
{
    /// <summary>
    /// Handles asymmetric encryption and decryption with the RSA algorithm.
    /// </summary>
    public sealed class RSAEncryptor : ICipher
    {
        private static readonly int ProviderFullType = 1;

        private RSACryptoServiceProvider m_CryptoServiceProvider;

        /// <summary>
        /// Gets the XML containing the public key data.
        /// </summary>
        public string PublicKeyXml
        {
            get
            {
                return m_CryptoServiceProvider.ToXmlString(false);
            }
        }

        /// <summary>
        /// Gets the XML containing the private key data.
        /// </summary>
        public string KeyPairXml
        {
            get
            {
                if (m_CryptoServiceProvider.PublicOnly)
                {
                    throw new InvalidOperationException(Resources.RSANoPrivateKeyException);
                }

                return m_CryptoServiceProvider.ToXmlString(true);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="UtilityLibrary.Cryptography.RSAEncryptor"/>
        /// contains only the public key (and thus can only encrypt).
        /// </summary>
        public bool PublicKeyOnly
        {
            get
            {
                return m_CryptoServiceProvider.PublicOnly;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="UtilityLibrary.Cryptography.RSAEncryptor"/> instance and generates a new public and private key pair.
        /// </summary>
        /// <param name="keySize">The size, in bits, of the key.</param>
        public RSAEncryptor(int keySize)
        {
            CspParameters parameters = new CspParameters(ProviderFullType);
            parameters.KeyContainerName = Resources.RSAKeyContainerName;
            parameters.Flags = CspProviderFlags.UseMachineKeyStore;
            parameters.ProviderName = Resources.RSAProviderName;
            m_CryptoServiceProvider = new RSACryptoServiceProvider(keySize, parameters);
        }

        /// <summary>
        /// Sets the public key or public and private key pair.
        /// </summary>
        /// <param name="xml">The XML containing the key data.</param>
        public void SetKey(string xml)
        {
            m_CryptoServiceProvider.FromXmlString(xml);
        }

        #region ICipher Members

        /// <summary>
        /// Gets a value indicating whether the current <see cref="UtilityLibrary.Cryptography.RSAEncryptor"/>
        /// can decrypt data.
        /// </summary>
        public bool CanDecrypt
        {
            get
            {
                return !m_CryptoServiceProvider.PublicOnly;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current <see cref="UtilityLibrary.Cryptography.RSAEncryptor"/>
        /// can encrypt data. This will always return true.
        /// </summary>
        public bool CanEncrypt
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Encrypts the given data using the current key.
        /// </summary>
        /// <param name="plainText">The plain-text data to be encrypted.</param>
        /// <returns>The cipher-text result.</returns>
        public byte[] Encrypt(byte[] plainText)
        {
            return m_CryptoServiceProvider.Encrypt(plainText, false);
        }

        /// <summary>
        /// Decrypts the given data using the current key.
        /// </summary>
        /// <param name="cipherText">The cipher-text to be decrypted.</param>
        /// <returns>The plain-text result.</returns>
        /// <exception cref="System.InvalidOperationException">Decryption is attempted without a private key.</exception>
        public byte[] Decrypt(byte[] cipherText)
        {
            if (!m_CryptoServiceProvider.PublicOnly)
            {
                return m_CryptoServiceProvider.Decrypt(cipherText, false);
            }
            else
            {
                // Can't decrypt without private key.
                Debug.Fail(Resources.RSANoPrivateKeyException);
                throw new InvalidOperationException(Resources.RSANoPrivateKeyException);
            }
        }

        #endregion
    }
}