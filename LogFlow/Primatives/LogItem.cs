using System;
using System.Threading;

namespace LogFlow
{
    public class LogItem
    {
        internal LogItem(LogLevel level, string message)
        {
            LoggedOn = DateTime.UtcNow;
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            Level = level;
            Message = message;
        }

        public DateTime LoggedOn { get; private set; }
        public int ThreadId { get; private set; }
        public LogLevel Level { get; private set; }
        public string Message { get; private set; }
    }
}
