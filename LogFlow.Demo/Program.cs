using System;

namespace LogFlow.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var writer = new RollingFileWriter("Logs",
                typeof(Program).Assembly.GetName().Name, RollOn.Hour);

            var logger = new Logger(writer);

            for (var i = 0; i < writer.BatchSize + 3; i++)
                logger.Log(LogLevel.Info, "Test{0:000}", i);

            Console.ReadKey();
        }
    }
}
