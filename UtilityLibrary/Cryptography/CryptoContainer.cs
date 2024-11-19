using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UtilityLibrary.Cryptography;

namespace UtilityLibrary
{
    /// <summary>
    /// Encrypts objects for safe storage using the Rijndael symmetric encryption algorithm.
    /// </summary>
    /// <typeparam name="T">The type of object to be encrypted and serialized.</typeparam>
    [Serializable]
    public abstract class CryptoContainer<T> : IXmlSerializable, ISerializable
    {
        [NonSerialized] private T m_Plaintext;
        [NonSerialized] private bool m_IsDecrypted = true;
        [NonSerialized] private object m_LazyDecryptionLock = new object();
        private byte[] m_IV = null;
        private byte[] m_CipherText = null;

        /// <summary>
        /// Gets or sets the object to be encrypted.
        /// </summary>
        public T Plaintext
        {
            get
            {
                lock (m_LazyDecryptionLock)
                {
                    if (!m_IsDecrypted)
                    {
                        Decrypt();
                        m_IsDecrypted = true;
                    }
                }

                return m_Plaintext;
            }
            set
            {
                m_Plaintext = value;
                m_IsDecrypted = true;
            }
        }

        /// <summary>
        /// When overriden in a derived class, gets the key to be used when encrypting or decrypting.
        /// </summary>
        protected abstract byte[] Key
        {
            get;
        }

        /// <summary>
        /// Initializes a new <see cref="UtilityLibrary.CryptoContainer{T}"/> instance.
        /// </summary>
        public CryptoContainer(T plaintext)
        {
            m_Plaintext = plaintext;
        }

        /// <summary>
        /// Initializes a new <see cref="CryptoContainer{T}"/> instance from serialized data.
        /// </summary>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> containing the information to deserialize.</param>
        /// <param name="context">The context for the deserialisation.</param>
        public CryptoContainer(SerializationInfo info, StreamingContext context)
        {
            m_IV = info.GetValue("IV", typeof(byte[])) as byte[];
            m_CipherText = info.GetValue("CipherText", typeof(byte[])) as byte[];
            m_IsDecrypted = false;
        }

        #region IXmlSerializable Members

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.Read();

            // Read IV.
            string ivBase64 = reader.ReadElementString("iv");
            m_IV = Convert.FromBase64String(ivBase64);

            // Read cipher text.
            string cipherTextBase64 = reader.ReadElementString("cipherText");
            m_CipherText = Convert.FromBase64String(cipherTextBase64);

            m_IsDecrypted = false;
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Encrypt();

            // Write IV.
            string ivBase64 = Convert.ToBase64String(m_IV, Base64FormattingOptions.InsertLineBreaks);
            writer.WriteElementString("iv", ivBase64);

            // Write cipher text.
            string cipherTextBase64 = Convert.ToBase64String(m_CipherText, Base64FormattingOptions.InsertLineBreaks);
            writer.WriteElementString("cipherText", cipherTextBase64);
        }

        #endregion

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Encrypt();
            info.AddValue("IV", m_IV);
            info.AddValue("CipherText", m_CipherText);
        }

        #endregion

        /// <summary>
        /// When overridden in a derived class, serializes an object to an array of bytes.
        /// </summary>
        /// <param name="plaintext">The object to be serialized.</param>
        /// <returns>An array of bytes containing the serialized object.</returns>
        protected abstract byte[] SerializePlaintext(T plaintext);

        /// <summary>
        /// When overridden in a derived class, deserializes an object from an array of bytes.
        /// </summary>
        /// <param name="data">The data containing the serialized object.</param>
        /// <returns>The deserialized object.</returns>
        protected abstract T DeserializePlaintext(byte[] data);

        /// <summary>
        /// Serializes and encrypts the current object.
        /// </summary>
        private void Encrypt()
        {
            RijndaelEncryptor encryptor;
            if (m_IV == null)
            {
                encryptor = new RijndaelEncryptor(Key);
                m_IV = encryptor.IV;
            }
            else
            {
                encryptor = new RijndaelEncryptor(m_IV, Key);
            }

            m_CipherText = encryptor.Encrypt(SerializePlaintext(m_Plaintext));
        }

        /// <summary>
        /// Decrypts and deserializes the contained cipher text.
        /// </summary>
        private void Decrypt()
        {
            if (m_IV != null && m_CipherText != null)
            {
                RijndaelEncryptor decryptor = new RijndaelEncryptor(m_IV, Key);
                byte[] plainText = decryptor.Decrypt(m_CipherText);
                m_Plaintext = DeserializePlaintext(plainText);
            }
            else
            {
                m_Plaintext = default(T);
            }
        }
    }
}