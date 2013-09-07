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
        private StreamWriter writer = null;

        private string path;
        private string prefix;
        private RollOn rollOn;

        public RollingFileWriter(string path, string prefix, RollOn rollOn) :
            base(LogTarget.RollingFile, 100, TimeSpan.FromSeconds(5))
        {
            Contract.Requires(path.IsPath());
            Contract.Requires(!string.IsNullOrWhiteSpace(prefix));
            Contract.Requires(path.IndexOfAny(Path.GetInvalidFileNameChars()) == -1);
            Contract.Requires(Enum.IsDefined(typeof(RollOn), rollOn));

            this.path = path;
            this.prefix = prefix;
            this.rollOn = rollOn;
        }

        public bool AutoFlush { get; set; }

        internal override void Setup()
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
            }

            var fileName = Path.Combine(path, string.Format("{0}_{1}.log", prefix, date));

            writer = new StreamWriter(fileName);

            writer.AutoFlush = AutoFlush;
        }

        internal override void Teardown()
        {
            writer.Dispose();
        }

        internal override async Task WriteAsync(IEnumerable<LogItem> logItems)
        {
            await Task.Run(() =>
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                });

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
    }
}
