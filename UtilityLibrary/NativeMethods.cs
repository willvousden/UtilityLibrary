using System;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilityLibrary
{
    /// <summary>
    /// Contains imported native methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Sets the minimum and maximum working set sizes for the specified process.
        /// </summary>
        /// <param name="hProcess">A handle to the process whose working set sizes is to be set.</param>
        /// <param name="dwMinimumWorkingSetSize">The minimum working set size for the process, in bytes.</param>
        /// <param name="dwMaximumWorkingSetSize">The maximum working set size for the process, in bytes. </param>
        /// <returns><c>true</c> if successful; <c>false</c> otherwise.</returns>
        [DllImport("Kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetProcessWorkingSetSize(IntPtr hProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        /// <summary>
        /// Copies a string into the specified section of an initialization file.
        /// </summary>
        /// <param name="section">The name of the section to which the string will be copied.</param>
        /// <param name="key">The name of the key to be associated with a string.</param>
        /// <param name="val">A null-terminated string to be written to the file.</param>
        /// <param name="filePath">The name of the initialization file.</param>
        /// <returns><c>true</c> if successful; <c>false</c> otherwise.</returns>
        [DllImport("kernel32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);

        /// <summary>
        /// Retrieves a string from the specified section in an initialization file.
        /// </summary>
        /// <param name="section">The name of the section containing the key name.</param>
        /// <param name="key">The name of the key whose associated string is to be retrieved.</param>
        /// <param name="def">A default string.</param>
        /// <param name="returnValue">The buffer that receives the retrieved string. </param>
        /// <param name="size">The size of the buffer in characters.</param>
        /// <param name="filePath">The name of the initialization file.</param>
        /// <returns>The number of characters copied to the buffer, not including the terminating null character.</returns>
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder returnValue, int size, string filePath);

        /// <summary>
        /// Replaces the keys and values for the specified section in an initialization file.
        /// </summary>
        /// <param name="section">The name of the section in which data are written.</param>
        /// <param name="keys">The new key names and associated values that are to be written to the named section.</param>
        /// <param name="filePath">The name of the initialization file.</param>
        /// <returns><c>true</c> if successful; <c>false</c> otherwise.</returns>
        [DllImport("kernell32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WritePrivateProfileSection(string section, string keys, string filePath);

        /// <summary>
        /// Retrieves the names of all sections in an initialization file.
        /// </summary>
        /// <param name="returnValue">A buffer that receives the section names associated with the named file.</param>
        /// <param name="size">The size of the buffer pointed in characters. </param>
        /// <param name="filePath">The name of the initialization file.</param>
        /// <returns>The number of characters copied to the specified buffer, not including the terminating null character.</returns>
        [DllImport("kernell32")]
        public static extern int GetPrivateProfileSectionNames(StringBuilder returnValue, int size, string filePath);

        /// <summary>
        /// Retrieves the current value of the high-resolution performance counter if one is provided by the OEM.
        /// </summary>
        /// <param name="lpPerformanceCount">The current performance-counter value in counts.</param>
        /// <returns><c>true</c> if successful; <c>false</c> otherwise.</returns>
        [DllImport("Kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        /// <summary>
        /// Retrieves the frequency of the high-resolution performance counter, if one exists. The frequency cannot change while the system is running.
        /// </summary>
        /// <param name="lpFrequency">The current performance-counter frequency, in counts per second.</param>
        /// <returns><c>true</c> if successful; <c>false</c> otherwise.</returns>
        [DllImport("Kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);
    }
}