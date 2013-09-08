using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;
using System.Threading.Tasks;

namespace LogFlow
{
    public class EventLogWriter : AbstractWriter
    {
        private string source;
        private string logName;

        public EventLogWriter(string source, string logName = "Application",
            int batchSize = 1, TimeSpan? timeout = null) :
            base(LogTarget.EventLog, batchSize, GetTimeout(timeout))
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(source));
            Contract.Requires(!string.IsNullOrWhiteSpace(logName));

            this.source = source;
            this.logName = logName;
        }

        internal override void Setup()
        {
            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, logName);
        }

        internal override void Teardown()
        {
        }

        internal override async Task WriteAsync(IEnumerable<LogItem> logItems)
        {
            foreach (var logItem in logItems)
            {
                EventLogEntryType type;

                switch (logItem.Level)
                {
                    case LogLevel.Info:
                        type = EventLogEntryType.Information;
                        break;
                    case LogLevel.Debug:
                        type = EventLogEntryType.Information;
                        break;
                    case LogLevel.Error:
                        type = EventLogEntryType.Error;
                        break;
                    case LogLevel.Failure:
                        type = EventLogEntryType.FailureAudit;
                        break;
                    case LogLevel.Warning:
                        type = EventLogEntryType.Information;
                        break;
                    default:
                        return;
                }

                var sb = new StringBuilder();

                sb.AppendFormat("({0}, LoggedOn: {1}, ThreadId: {2})",
                    logItem.Level, logItem.LoggedOn.ToString("o"), logItem.ThreadId);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine(logItem.Message);

                await Task.Run(() => EventLog.WriteEntry(source, sb.ToString(), type));
            }
        }

        private static TimeSpan GetTimeout(TimeSpan? timeout)
        {
            return timeout.HasValue ? timeout.Value : TimeSpan.Zero;
        }
    }
}
