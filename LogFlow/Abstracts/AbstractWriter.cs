using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace LogFlow
{
    public abstract class AbstractWriter
    {
        public AbstractWriter(Target target, 
            LogLevel minLevel = LogLevel.Error)
        {
            Contract.Requires(Enum.IsDefined(typeof(Target), target));
            Contract.Requires(Enum.IsDefined(typeof(LogLevel), target));

            Target = target;
            MinLevel = minLevel;
        }

        public Target Target { get; private set; }

        public LogLevel MinLevel { get; set; }

        internal abstract void Setup();
        internal abstract void Teardown();

        protected abstract Task WriteAsync(LogItem logItem);
    }
}
