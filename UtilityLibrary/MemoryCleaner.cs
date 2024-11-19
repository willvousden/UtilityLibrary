using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace UtilityLibrary
{
    /// <summary>
    /// Facilitates manual and automatic resource cleanup for the current application instance.
    /// </summary>
    public static class MemoryCleaner
    {
        private static readonly object m_EventLock = new object();
        private static readonly TimeSpan m_DefaultAutoCleanInterval = TimeSpan.FromMinutes(5);
        private static EventHandler<CancelEventArgs> m_Cleaning = null;
        private static EventHandler m_Cleaned = null;
        private static Timer m_CleanTimer;
        private static bool m_IsCleaning = false;
        private static object m_CleanLock = new object();

        /// <summary>
        /// Occurs immediately before a cleanup.
        /// </summary>
        public static event EventHandler<CancelEventArgs> Cleaning
        {
            add
            {
                lock (m_EventLock)
                {
                    m_Cleaning += value;
                }
            }
            remove
            {
                lock (m_EventLock)
                {
                    m_Cleaning -= value;
                }
            }
        }

        /// <summary>
        /// Occurs immediately after a cleanup.
        /// </summary>
        public static event EventHandler Cleaned
        {
            add
            {
                lock (m_EventLock)
                {
                    m_Cleaned += value;
                }
            }
            remove
            {
                lock (m_EventLock)
                {
                    m_Cleaned -= value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the interval between automatic cleans.
        /// </summary>
        public static TimeSpan AutoCleanInterval
        {
            get
            {
                return TimeSpan.FromMilliseconds(m_CleanTimer.Interval);
            }
            set
            {
                bool initiallyEnabled = m_CleanTimer.Enabled;
                m_CleanTimer.Enabled = false;
                m_CleanTimer.Interval = value.TotalMilliseconds;
                m_CleanTimer.Enabled = initiallyEnabled;
            }
        }

        /// <summary>
        /// Gets or sets a value specifying whether auto memory cleaning should occur.
        /// </summary>
        public static bool EnableAutoClean
        {
            get
            {
                return m_CleanTimer.Enabled;
            }
            set
            {
                m_CleanTimer.Enabled = value;
            }
        }

        /// <summary>
        /// Initializes static members.
        /// </summary>
        static MemoryCleaner()
        {
            m_CleanTimer = new Timer(m_DefaultAutoCleanInterval.TotalMilliseconds);
            m_CleanTimer.Elapsed += CleanTimerElapsed;
        }

        /// <summary>
        /// Manually cleans out memory for the current application instance.
        /// </summary>
        public static void Clean()
        {
            Clean(TimeSpan.Zero);
        }

        /// <summary>
        /// Manually cleans out memory for the current application instance.
        /// </summary>
        /// <param name="delay">The delay before memory is cleaned.</param>
        public static void Clean(TimeSpan delay)
        {
            Clean(delay, ThreadPriority.BelowNormal);
        }

        /// <summary>
        /// Manually cleans out memory for this application.
        /// </summary>
        /// <param name="delay">The delay before memory is cleaned.</param>
        /// <param name="priority">The priority to assign to the cleanup worker thread.</param>
        private static void Clean(TimeSpan delay, ThreadPriority priority)
        {
            ThreadStart callback = () =>
                                   {
                                       Thread.Sleep(delay);
                                       FlushMemory();
                                   };
            Thread cleanThread = new Thread(callback);
            cleanThread.Priority = priority;
            cleanThread.Start();
        }

        /// <summary>
        /// Performs memory cleanup.
        /// </summary>
        private static void FlushMemory()
        {
            if (!m_IsCleaning)
            {
                lock (m_CleanLock)
                {
                    m_IsCleaning = true;
                    CancelEventArgs e = new CancelEventArgs();
                    OnCleaning(e);
                    if (e.Cancel)
                    {
                        return;
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    {
                        NativeMethods.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
                    }

                    OnCleaned(EventArgs.Empty);
                    m_IsCleaning = false;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="MemoryCleaner.Cleaning"/> event.
        /// </summary>
        private static void OnCleaning(CancelEventArgs e)
        {
            ExtensionMethods.Raise(null, ref m_Cleaning, m_EventLock, e);
        }

        /// <summary>
        /// Raises the <see cref="MemoryCleaner.Cleaned"/> event.
        /// </summary>
        private static void OnCleaned(EventArgs e)
        {
            ExtensionMethods.Raise(null, ref m_Cleaned, m_EventLock, e);
        }

        #region Event handlers

        private static void CleanTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Call asynchronously to avoid tying up the current thread pool thread.
            Clean(TimeSpan.Zero, ThreadPriority.Lowest);
        }

        #endregion
    }
}