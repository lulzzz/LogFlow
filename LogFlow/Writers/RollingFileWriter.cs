using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LogFlow
{
    public class RollingFileWriter : AbstractWriter
    {
        private string lastFileName = null;
        private StreamWriter writer = null;

        private string path;
        private string prefix;
        private RollOn rollOn;

        public RollingFileWriter(string path, string prefix, RollOn rollOn,
            int batchSize = 50, TimeSpan? timeout = null) :
            base(LogTarget.RollingFile, batchSize, GetTimeout(timeout))
        {
            Contract.Requires(path.IsPath());
            Contract.Requires(!string.IsNullOrWhiteSpace(prefix));
            Contract.Requires(prefix.IndexOfAny(Path.GetInvalidFileNameChars()) == -1);
            Contract.Requires(prefix == prefix.Trim());
            Contract.Requires(Enum.IsDefined(typeof(RollOn), rollOn));

            this.path = path;
            this.prefix = prefix;
            this.rollOn = rollOn;

            AutoFlush = true;
        }

        public bool AutoFlush { get; set; }

        internal override void Setup()
        {
        }

        internal override void Teardown()
        {
            if (writer != null)
                writer.Dispose();
        }

        internal override async Task WriteAsync(IEnumerable<LogItem> logItems)
        {
            string fileName;

            if (rollOn == RollOn.PerRun)
            {
                if (lastFileName == null)
                {
                    fileName = Path.Combine(path,
                        string.Format("{0}_{1}.log", prefix, Guid.NewGuid()));
                }
                else
                {
                    fileName = lastFileName;
                }
            }
            else
            {
                string date = null;

                var now = DateTime.UtcNow;

                switch (rollOn)
                {
                    case RollOn.Month:
                        date = string.Format("{0:yyyyMM}", now);
                        break;
                    case RollOn.Week:
                        date = string.Format("{0}{1:00}", now.Year, now.WeekOfYear());
                        break;
                    case RollOn.Day:
                        date = string.Format("{0:yyyyMMdd}", now);
                        break;
                    case RollOn.Hour:
                        date = string.Format("{0:yyyyMMddHH}", now);
                        break;
                    case RollOn.Minute:
                        date = string.Format("{0:yyyyMMddHHmm}", now);
                        break;
                }

                fileName = Path.Combine(path, string.Format("{0}_{1}.log", prefix, date));
            }

            if (fileName != lastFileName)
            {
                Teardown();

                await Task.Run(() =>
                    {
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                    });

                writer = new StreamWriter(fileName);

                writer.AutoFlush = AutoFlush;
            }

            lastFileName = fileName;

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
                sb.AppendLine();
            }

            await writer.WriteAsync(sb.ToString());
        }

        private static TimeSpan GetTimeout(TimeSpan? timeout)
        {
            return timeout.HasValue ? timeout.Value : TimeSpan.FromSeconds(5);
        }
    }
}
