using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFlow.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileWriter = new RollingFileWriter("Logs", true);

            var logger = new Logger(null);
        }
    }
}
