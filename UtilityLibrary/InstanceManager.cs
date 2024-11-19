using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UtilityLibrary.Properties;

namespace UtilityLibrary
{
    /// <summary>
    /// Facilitates management of multiple application instances.
    /// </summary>
    public sealed class InstanceManager : IDisposable
    {
        private static List<string> m_UsedNames = new List<string>();

        private readonly object m_EventLock = new object();
        private EventHandler m_Signalled;
        private string m_Name;
        private string m_MutexName;
        private Semaphore m_Semaphore;
        private Mutex m_Mutex;
        private bool m_IsOnlyInstance;
        private bool m_IsDisposed = false;

        /// <summary>
        /// Occurs when a signal is received from another instance of the application.
        /// </summary>
        public event EventHandler Signalled
        {
            add
            {
                lock (m_EventLock)
                {
                    if (m_IsDisposed)
                    {
                        throw new ObjectDisposedException(GetType().Name);
                    }

                    m_Signalled += value;
                }
            }
            remove
            {
                lock (m_EventLock)
                {
                    if (m_IsDisposed)
                    {
                        throw new ObjectDisposedException(GetType().Name);
                    }

                    m_Signalled -= value;
                }
            }
        }

        /// <summary>
        /// Gets the name of the semaphore used to communicate with other instances.
        /// </summary>
        public string Name
        {
            get
            {
                if (m_IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_Name;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are other instances currently running.
        /// </summary>
        public bool IsOnlyInstance
        {
            get
            {
                if (m_IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_IsOnlyInstance;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="UtilityLibrary.InstanceManager"/>.
        /// </summary>
        /// <param name="name">The name of the semaphore that the instance manager should use to communicate with other instances.</param>
        public InstanceManager(string name)
        {
            bool nameAlreadyInUse = m_UsedNames.Contains(name);
            if (nameAlreadyInUse)
            {
                throw new ArgumentException(Resources.InstanceManagerNameInUseException, "name");
            }

            m_Name = name;
            m_UsedNames.Add(name);
            m_Semaphore = new Semaphore(0, 1, name, out m_IsOnlyInstance);

            WaitOrTimerCallback callback = (state, timedOut) =>
                                           {
                                               OnSignalled(EventArgs.Empty);
                                           };
            ThreadPool.RegisterWaitForSingleObject(m_Semaphore, callback, null, Timeout.Infinite, false);
        }

        /// <summary>
        /// Initializes a new <see cref="UtilityLibrary.InstanceManager"/>.
        /// </summary>
        /// <param name="name">The name of the semaphore that the instance manager should use to communicate with other instances.</param>
        /// <param name="mutexName">The name of a mutex that should also be created; this must not be the same as the name used for the semaphore.</param>
        public InstanceManager(string name, string mutexName)
            : this(name)
        {
            bool nameAlreadyInUse = m_UsedNames.Contains(mutexName);
            if (nameAlreadyInUse)
            {
                throw new ArgumentException(Resources.InstanceManagerNameInUseException, "name");
            }

            m_MutexName = mutexName;
            m_UsedNames.Add(m_MutexName);

            bool createdNew;
            m_Mutex = new Mutex(false, mutexName, out createdNew);
            m_IsOnlyInstance = m_IsOnlyInstance || createdNew;
        }

        /// <summary>
        /// Releases resources in use by the current instance.
        /// </summary>
        ~InstanceManager()
        {
            if (!m_IsDisposed)
            {
                Dispose(false);
            }
        }

        /// <summary>
        /// Sends a signal to other instances.
        /// </summary>
        public void Signal()
        {
            if (m_IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            m_Semaphore.Release();
        }

        /// <summary>
        /// Releases all resources used by the current <see cref="UtilityLibrary.InstanceManager"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases all resources used by the current <see cref="UtilityLibrary.InstanceManager"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> if managed resources should be disposed; <c>flase</c> otherwise.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (m_Semaphore != null)
                {
                    m_Semaphore.Close();
                }
                else
                {
                    Debug.Fail("Semaphore is null on disposal.");
                }

                if (m_Mutex != null)
                {
                    m_Mutex.Close();
                }
            }

            m_UsedNames.Remove(m_Name);
            if (m_MutexName != null)
            {
                m_UsedNames.Remove(m_MutexName);
            }

            m_IsDisposed = true;
            m_Signalled = null;
        }

        /// <summary>
        /// Raises the <see cref="UtilityLibrary.InstanceManager.Signalled"/> event.
        /// </summary>
        /// <param name="e">The arguments for the event.</param>
        private void OnSignalled(EventArgs e)
        {
            if (m_IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            this.Raise(ref m_Signalled, m_EventLock, e);
        }
    }
}