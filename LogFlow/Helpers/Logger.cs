using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace LogFlow
{
    public sealed class Logger : IDisposable
    {
        private readonly ActionBlock<LogItem> writerBlocks;
        private readonly List<AbstractWriter> writers;

        private CancellationTokenSource cts;

        private bool disposed = false;

        public Logger(IEnumerable<AbstractWriter> appenders)
        {
            Contract.Requires(appenders != null);

            this.writers = appenders.ToList();

            cts = new CancellationTokenSource();

            var buffer = new BufferBlock<LogItem>(
                new DataflowBlockOptions()
                {
                    CancellationToken = cts.Token
                });

            foreach(var writer in this.writers)
            {
                writer.Setup();

                var writerBlock = new ActionBlock<LogItem>(
                    async logItem =>
                    {
                        //await writer.WriteLineAsync(s);

                        //if (autoFlush)
                        //    await writer.FlushAsync();
                    },
                    new ExecutionDataflowBlockOptions()
                    {
                        MaxDegreeOfParallelism = 1
                    });

                //writerBlock.Completion.ContinueWith(
                //    task => writer.Dispose());
            }
        }

        ~Logger()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                //writerBlock.Complete();

                //writerBlock.Completion.Wait();
            }

            disposed = true;
        }

        public void Log(LogLevel status, string format, params object[] args)
        {
            var logItem = new LogItem(
                status, string.Format(format, args));

            //if (!writerBlock.Post(logItem.ToCsvString()))
            //    throw new ObjectDisposedException("Logger");
        }
    }
}

