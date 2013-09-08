using System;
using System.Reflection;

namespace LogFlow.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // When {BATCHSIZE} log items are logged, the log items
            // will be persisted to the underlying data store
            const int RFWBATCHSIZE = 10;

            // If "timeout" elapses before {BATCHSIZE} log items are 
            // logged, the data will be persisted to the underlying 
            // data store
            var RFWTIMEOUTSECONDS = 5;

            // The log-filename prefix could be set to anything, but 
            // the entry-assembly name is usually best
            var prefix = Assembly.GetEntryAssembly().GetName().Name;

            var rollingFileWriter = new RollingFileWriter("Logs", prefix, 
                RollOn.PerRun, RFWBATCHSIZE, TimeSpan.FromSeconds(RFWTIMEOUTSECONDS));

            var logger = new Logger(rollingFileWriter);

            // Let's log (rollingFileWriter.BatchSize + 2) items to 
            // ensure that we have two batches
            for (var i = 0; i < rollingFileWriter.BatchSize + 2; i++)
                logger.Log(LogLevel.Info, "Test{0:000}", i);

            Console.Write("Press any key to terminate (Note: if this is done before the {0}-second timeout interval elapses, then the second batch of log item's will not be persisted)...", RFWTIMEOUTSECONDS);

            Console.ReadKey();

            // You'd typically call "logger.Dispose()" when terminating, 
            // in order to ensure that all of the log items become 
            // persisted.  In this example, though, the method call has
            //  been commented out to demonstrate batch timeout.

            //logger.Dispose();
        }
    }
}
