namespace CruiseControl.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using NLog;
    using NLog.Config;
    using NLog.Targets;

    public class LogHelper
        : IDisposable
    {
        private readonly LoggingConfiguration oldConfig;
        private readonly IList<string> messages;

        private LogHelper(LoggingConfiguration config, IList<string> messages)
        {
            this.oldConfig = config;
            this.messages = messages;
        }

        public IList<string> Messages
        {
            get { return this.messages; }
        }

        public static LogHelper InterceptLogging(params Type[] typesToIntercept)
        {
            var config = new LoggingConfiguration();
            var target = new MemoryTarget
                             {
                                 Layout = "${level}|${logger}|${message}"
                             };
            config.AddTarget("UnitTest", target);
            foreach (var type in typesToIntercept)
            {
                config.LoggingRules.Add(
                    new LoggingRule(type.FullName, LogLevel.Trace, target));
            }

            var oldConfig = LogManager.Configuration;
            LogManager.Configuration = config;
            LogManager.ReconfigExistingLoggers();
            return new LogHelper(oldConfig, target.Logs);
        }

        public void Dispose()
        {
            LogManager.Configuration = this.oldConfig;
            LogManager.ReconfigExistingLoggers();
        }
    }
}
