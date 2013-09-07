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

        public Logger(params AbstractWriter[] writers)
        {
            Contract.Requires(writers != null);

            this.writers = writers.ToList();

            cts = new CancellationTokenSource();

            foreach (var writer in this.writers)
            {
                writer.Setup();

                var writerBlock = new ActionBlock<LogItem[]>(
                    async logItems =>
                    {
                        await writer.WriteAsync(logItems);


                        //if (autoFlush)
                        //await writer.FlushAsync();
                    },
                    new ExecutionDataflowBlockOptions()
                    {
                        MaxDegreeOfParallelism = 1
                    });

                writerBlock.Completion.ContinueWith(
                    task => writer.Teardown());
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

