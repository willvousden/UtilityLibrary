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
        private static readonly string m_InnerExceptionIndent = "    ";
        private static readonly string m_ListItemIndent = "   ";

        /// <summary>
        /// Creates a detailed log of a <see cref="System.Exception"/> in text format.
        /// </summary>
        /// <param name="exception">The <see cref="System.Exception"/>. to log.</param>
        /// <param name="fileName">The name of the file to which the log should be saved.</param>
        public static void LogException(Exception exception, string fileName)
        {
            LogException(exception, fileName, null);
        }

        /// <summary>
        /// Creates a detailed log of a <see cref="System.Exception"/> in text format.
        /// </summary>
        /// <param name="exception">The <see cref="System.Exception"/>. to log.</param>
        /// <param name="fileName">The name of the file to which the log should be saved.</param>
        /// <param name="stateMessage">An additional message containing information about the circumstances under which the exception occured.</param>
        public static void LogException(Exception exception, string fileName, string stateMessage)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                LogException(exception, stream, stateMessage);
            }
        }

        /// <summary>
        /// Creates a detailed log of a <see cref="System.Exception"/> in text format.
        /// </summary>
        /// <param name="exception">The <see cref="System.Exception"/> to log.</param>
        /// <param name="stream">The <see cref="System.IO.Stream"/> to which the log should be written.</param>
        public static void LogException(Exception exception, Stream stream)
        {
            LogException(exception, stream, null);
        }

        /// <summary>
        /// Creates a detailed log of a <see cref="System.Exception"/> in text format.
        /// </summary>
        /// <param name="exception">The <see cref="System.Exception"/> to log.</param>
        /// <param name="stream">The <see cref="System.IO.Stream"/> to which the log should be written.</param>
        /// <param name="stateMessage">An additional message containing information about the circumstances under which the exception occured.</param>
        public static void LogException(Exception exception, Stream stream, string stateMessage)
        {
            new { stream }.CheckNotNull();

            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine("Timestamp:");
                writer.WriteLine(DateTime.Now.ToString());
                writer.WriteLine();

                WriteExceptionInfo(exception, writer, stateMessage);

                writer.Flush();
            }
        }

        /// <summary>
        /// Concatenates a number of arrays into a single array.
        /// </summary>
        /// <typeparam name="T">The type of the arrays to be concatenated.</typeparam>
        /// <param name="arrays">The arrays to concatenate.</param>
        /// <returns>A one-dimensional array of type <typeparamref name="T"/> containing the supplied arrays.</returns>
        public static T[] ConcatenateArrays<T>(params T[][] arrays)
        {
            int totalLength = 0;
            foreach (T[] array in arrays)
            {
                totalLength += array.Length;
            }

            T[] aggregateArray = new T[totalLength];
            for (int i = 0, offset = 0; i < arrays.Length; i++)
            {
                arrays[i].CopyTo(aggregateArray, offset);
                offset += arrays[i].Length;
            }
            return aggregateArray;
        }

        /// <summary>
        /// Adds a new entry or modifies an existing entry in the system's RunOnce registry key.
        /// </summary>
        /// <param name="runAtStartup"><c>true</c> for the application to be run at startup; <c>false</c>
        /// otherwise.</param>
        /// <param name="applicationName">The name of the application whose value to sum to the registry.</param>
        /// <param name="executablePath">The path to the executable to be run when Windows starts.</param>
        public static void SetRunAtStartup(bool runAtStartup, string applicationName, string executablePath)
        {
            SetRunAtStartup(runAtStartup, applicationName, executablePath, false);
        }

        /// <summary>
        /// Adds a new entry or modifies an existing entry in the system's RunOnce registry key.
        /// </summary>
        /// <param name="runAtStartup"><c>true</c> for the application to be run at startup; <c>false</c>
        /// otherwise.</param>
        /// <param name="applicationName">The name of the application whose value to sum to the registry.</param>
        /// <param name="executablePath">The path to the executable to be run when Windows starts.</param>
        /// <param name="localMachine"><c>true</c> for the application to be run before Windows log in; <c>false</c> for the application to be run as the user after log in.</param>
        public static void SetRunAtStartup(bool runAtStartup, string applicationName, string executablePath, bool localMachine)
        {
            RegistryKey rootKey = localMachine ? Registry.LocalMachine : Registry.CurrentUser;
            RegistryKey startupKey = rootKey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (runAtStartup)
            {
                startupKey.SetValue(applicationName, '"' + executablePath + '"');
            }
            else
            {
                startupKey.DeleteValue(applicationName, false);
            }
        }

        /// <summary>
        /// Gets a value specifying whether the application will be run at Windows startup.
        /// </summary>
        /// <param name="applicationName">The name of the application whose value to check.</param>
        /// <param name="executablePath">The path to the executable to check.</param>
        /// <returns><c>true</c> if the application is run at startup; <c>false</c> otherwise.</returns>
        public static bool GetRunAtStartup(string applicationName, string executablePath)
        {
            return GetRunAtStartup(applicationName, executablePath, false);
        }

        /// <summary>
        /// Gets a value specifying whether the application will be run at Windows startup.
        /// </summary>
        /// <param name="applicationName">The name of the application whose value to check.</param>
        /// <param name="executablePath">The path to the executable to check.</param>
        /// <returns><c>true</c> if the application is run at startup; <c>false</c> otherwise.</returns>
        /// <param name="localMachine"><c>true</c> to check the HKEYLOCALMACHINE registry hive; <c>false</c>
        /// to check the HKEYCURRENTUSER hive.</param>
        public static bool GetRunAtStartup(string applicationName, string executablePath, bool localMachine)
        {
            RegistryKey rootKey = localMachine ? Registry.LocalMachine : Registry.CurrentUser;
            RegistryKey startupKey = rootKey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            string startupPath = (string)startupKey.GetValue(applicationName);
            return startupPath == '"' + executablePath + '"';
        }

        /// <summary>
        /// Recursively writes the information of an exception and any inner exceptions to an exception log.
        /// </summary>
        /// <param name="exception">The <see cref="System.Exception"/> whose information is to be written.</param>
        /// <param name="writer">A <see cref="System.IO.StreamWriter"/> to which to write the exception information.</param>
        /// <param name="stateMessage">An additional message containing information about the circumstances under which the exception occured.</param>
        private static void WriteExceptionInfo(Exception exception, StreamWriter writer, string stateMessage)
        {
            WriteExceptionInfo(exception, writer, stateMessage, string.Empty);
        }

        /// <summary>
        /// Recursively writes the information of an exception and any inner exceptions to an exception log.
        /// </summary>
        /// <param name="exception">The <see cref="System.Exception"/> whose information is to be written.</param>
        /// <param name="writer">A <see cref="System.IO.StreamWriter"/> to which to write the exception information.</param>
        /// <param name="indent">The indentation at which to start writing exception information.</param>
        /// <param name="stateMessage">An additional message containing information about the circumstances under which the exception occured.</param>
        private static void WriteExceptionInfo(Exception exception, StreamWriter writer, string stateMessage, string indent)
        {
            new { exception, writer }.CheckNotNull();

            string[] lineSeparators = { Environment.NewLine };

            writer.WriteLine(indent + "Exception type:");
            writer.WriteLine(indent + exception.GetType().FullName);

            // Write exception message.
            if (exception.Message != null)
            {
                writer.WriteLine(indent);
                writer.WriteLine(indent + "Message:");

                // Prepend indendation to each line and write it.
                string[] messageLines = exception.Message.Split(lineSeparators, StringSplitOptions.None);
                foreach (string line in messageLines)
                {
                    writer.WriteLine(indent + line);
                }
            }

            // Write additional message.
            if (stateMessage != null)
            {
                writer.WriteLine(indent);
                writer.WriteLine("State message:");

                // Prepend indendation to each line and write it.
                string[] stateMessageLines = stateMessage.Split(lineSeparators, StringSplitOptions.None);
                foreach (string line in stateMessageLines)
                {
                    writer.WriteLine(indent + line);
                }
            }

            // Write provided exception data (if any).
            if (exception.Data != null && exception.Data.Count != 0)
            {
                writer.WriteLine(indent);
                writer.WriteLine(indent + "Exception data:");

                string itemIndent = indent + m_ListItemIndent;
                foreach (object key in exception.Data.Keys)
                {
                    writer.WriteLine(itemIndent + "{0} : {1}", key, exception.Data[key]);
                }
            }

            // Write stack trace.
            if (exception.StackTrace != null)
            {
                writer.WriteLine(indent);
                writer.WriteLine(indent + "Stack trace:");

                // Prepend indendation to each line and write.
                string[] stackTraceLines = exception.StackTrace.Split(lineSeparators, StringSplitOptions.None);
                foreach (string line in stackTraceLines)
                {
                    writer.WriteLine(indent + line);
                }
            }

            // Write inner exception (if any).
            if (exception.InnerException != null)
            {
                WriteExceptionInfo(exception, writer, indent + m_InnerExceptionIndent);
            }
        }
    }
}