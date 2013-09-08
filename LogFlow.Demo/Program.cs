using System;
using System.Reflection;

namespace LogFlow.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // When {BATCHSIZE} log items are logged, the log items will be 
            // persisted to the underlying data store
            const int BATCHSIZE = 10;

            // If "timeout" elapses before {BATCHSIZE} log items are logged, 
            // the data will be persisted to the underlying data store
            var TIMEOUTSECONDS = 5;

            // The log-filename prefix could be set to anything, but the entry 
            // assembly name is usually best
            var assemblyName = Assembly.GetEntryAssembly().GetName().Name;

            // A ConsoleWriter may be specified with a non-default BATCHSIZE 
            // and TIMEOUT, but you'll typically want to use the default 
            // constructor to write out log items to the Console immediately
            var consoleWriter = new ConsoleWriter();

            // A RollingLogWriter persists log items to files, with the actual 
            // name of the file varying as per the Prefix and RollOn parameters
            var rollingFileWriter = new RollingFileWriter("Logs", assemblyName, 
                RollOn.PerRun, BATCHSIZE, TimeSpan.FromSeconds(TIMEOUTSECONDS));

            // An EventLogWriter may raise a Security error the first time it is 
            // run if the application isn't run with Adminstrator priviledges or 
            // if the EventLog "source" wasn't previously created by a program 
            // such as an installer that had Administrator priviledges
            //
            // var eventLogWriter = new EventLogWriter(assemblyName);

            // Log items are persisted to one or more "writers"
            var logger = new Logger(consoleWriter, rollingFileWriter);

            // Let's log (rollingFileWriter.BatchSize + 2) items to ensure that 
            // we have two batches
            for (var i = 0; i < rollingFileWriter.BatchSize + 2; i++)
                logger.Log(LogLevel.Info, "Test{0:000}", i);

            Console.WriteLine("Press any key to terminate (NOTE: if this is done before the {0}-second timeout,", TIMEOUTSECONDS);
            Console.WriteLine("elapses, the second batch of log items will NOT be persisted to disk.  All of");
            Console.WriteLine("the log items will, however, be written to the Console right away, since the");
            Console.WriteLine("default ConsoleWriter ignores batching!)...");

            Console.WriteLine();

            Console.ReadKey();

            // You'd typically call "logger.Dispose()", when terminating, to ensure 
            // that all of the log items become persisted.  In this example, though, 
            // the method call has been commented out to demonstrate batch timeout
            //
            // logger.Dispose();
        }
    }
}
