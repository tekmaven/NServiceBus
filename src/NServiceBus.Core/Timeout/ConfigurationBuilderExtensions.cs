﻿namespace NServiceBus
{
    using System;

    /// <summary>
    /// Extension methods declarations.
    /// </summary>
    public static class ConfigurationTimeoutExtensions
    {
        /// <summary>
        /// A critical error is raised when timeout retrieval fails.
        /// By default we wait for 2 seconds for the storage to come back.
        /// This method allows to change the default and extend the wait time.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="timeToWait">Time to wait before raising a critical error.</param>
        public static void TimeToWaitBeforeTriggeringCriticalErrorOnTimeoutOutages(this BusConfiguration config, TimeSpan timeToWait)
        {
            config.Settings.Set("TimeToWaitBeforeTriggeringCriticalErrorForTimeoutPersisterReceiver", timeToWait);
        }
    }
}
