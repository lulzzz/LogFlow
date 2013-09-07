using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LogFlow
{
    public abstract class AbstractWriter
    {
        private LogLevel minLevel;

        public AbstractWriter(LogTarget target, int batchSize, TimeSpan timeout)
        {
            Contract.Requires(Enum.IsDefined(typeof(LogTarget), target));
            Contract.Requires(batchSize >= 1);
            Contract.Requires(timeout >= TimeSpan.Zero);

            Target = target;
            BatchSize = batchSize;
            Timeout = timeout;

            MinLevel = LogLevel.Error;
        }

        public LogTarget Target { get; private set; }
        public int BatchSize { get; private set; }
        public TimeSpan Timeout { get; private set; }

        public LogLevel MinLevel
        {
            get
            {
                return minLevel;
            }
            set
            {
                Contract.Requires(Enum.IsDefined(typeof(LogLevel), value));

                minLevel = value;
            }
        }

        internal abstract void Setup();
        internal abstract void Teardown();

        internal abstract Task WriteAsync(IEnumerable<LogItem> logItems);
    }
}
