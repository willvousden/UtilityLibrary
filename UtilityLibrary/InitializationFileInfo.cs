using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UtilityLibrary
{
    /// <summary>
    /// Represents an initialization file.
    /// </summary>
    public sealed class InitializationFileInfo
    {
        private const char Terminator = '\0';
        private static readonly int BufferSize = 1024;

        private string m_Path;

        /// <summary>
        /// Gets or sets the value for the specified key.
        /// </summary>
        /// <param name="section">The name of section that the key is under.</param>
        /// <param name="key">The name of the key.</param>
        /// <returns>The value of the key.</returns>
        public string this[string section, string key]
        {
            get
            {
                if (!Exists)
                {
                    throw new FileNotFoundException();
                }

                StringBuilder outputStringBuilder = new StringBuilder(BufferSize);
                int valueLength = NativeMethods.GetPrivateProfileString(section, key, null, outputStringBuilder, BufferSize, m_Path);
                return outputStringBuilder.ToString();
            }
            set
            {
                if (!Exists)
                {
                    throw new FileNotFoundException();
                }

                NativeMethods.WritePrivateProfileString(section, key, value, m_Path);
            }
        }

        /// <summary>
        /// Gets or sets the path to the file.
        /// </summary>
        public string Path
        {
            get
            {
                return m_Path;
            }
            set
            {
                m_Path = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the file exists.
        /// </summary>
        public bool Exists
        {
            get
            {
                return File.Exists(m_Path);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="UtilityLibrary.InitializationFileInfo"/> instance.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        public InitializationFileInfo(string path)
        {
            new { path }.CheckNotNull();

            m_Path = path;
        }

        /// <summary>
        /// Creates the file if it does not already exist.
        /// </summary>
        public void Create()
        {
            if (!Exists)
            {
                File.Create(m_Path).Dispose();
            }
        }

        /// <summary>
        /// Adds a new section to the file.
        /// </summary>
        /// <param name="name">The name of the section to sum.</param>
        public void AddSection(string name)
        {
            AddSection(name, new string[0]);
        }

        /// <summary>
        /// Adds a new section to the file.
        /// </summary>
        /// <param name="name">The name of the section to sum.</param>
        /// <param name="keys">An array of strings containing key names that will be set to empty values.</param>
        public void AddSection(string name, params string[] keys)
        {
            IDictionary<string, string> values = new Dictionary<string, string>();
            foreach (string key in keys)
            {
                bool keyExists = values.ContainsKey(key);
                if (!keyExists)
                {
                    values.Add(key, string.Empty);
                }
            }

            AddSection(name, values);
        }

        /// <summary>
        /// Adds a new section to the file.
        /// </summary>
        /// <param name="name">The name of the section to sum.</param>
        /// <param name="values">A dictionary containing the key-value pairs to be written.</param>
        public void AddSection(string name, IDictionary<string, string> values)
        {
            if (!Exists)
            {
                throw new FileNotFoundException();
            }

            StringBuilder pairBuilder = new StringBuilder();
            foreach (string key in values.Keys)
            {
                string keyValurPair = string.Format("{0}={1}{2}", key, values[key], Terminator);
                pairBuilder.Append(keyValurPair);
            }

            NativeMethods.WritePrivateProfileSection(name, pairBuilder.ToString() + Terminator, m_Path);
        }

        /// <summary>
        /// Gets a list of section names contained in the file.
        /// </summary>
        /// <returns>An array of strings containing the names of the sections in the file.</returns>
        public string[] GetSectionsNames()
        {
            if (!Exists)
            {
                throw new FileNotFoundException();
            }

            StringBuilder returnValue = new StringBuilder(BufferSize);
            int valueLength = NativeMethods.GetPrivateProfileSectionNames(returnValue, BufferSize, m_Path);
            string sectionNames = returnValue.ToString();

            return sectionNames.Split(new char[] { Terminator }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}