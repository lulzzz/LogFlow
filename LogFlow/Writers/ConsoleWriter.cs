using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LogFlow
{
    public class ConsoleWriter : AbstractWriter
    {
        public ConsoleWriter(int batchSize = 1, TimeSpan? timeout = null) :
            base(LogTarget.Console, batchSize, GetTimeout(timeout))
        {
        }

        internal override void Setup()
        {
        }

        internal override void Teardown()
        {
        }

        internal override async Task WriteAsync(IEnumerable<LogItem> logItems)
        {
            var sb = new StringBuilder();

            foreach (var logItem in logItems)
            {
                sb.Append(logItem.LoggedOn.ToString("o"));
                sb.Append(',');
                sb.Append(logItem.ThreadId);
                sb.Append(',');
                sb.Append(logItem.Level);
                sb.Append(',');
                sb.Append(logItem.Message);
            }

            await Task.Run(() => Console.WriteLine(sb.ToString()));
        }

        private static TimeSpan GetTimeout(TimeSpan? timeout)
        {
            return timeout.HasValue ? timeout.Value : TimeSpan.Zero;
        }
    }
}
