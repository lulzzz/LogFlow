using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LogFlow
{
    public class FileWriter : AbstractWriter
    {
        private StreamWriter writer = null;

        private string fileName;
        private bool append;

        public FileWriter(string fileName, bool append) :
            base(Target.File)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(fileName));

            this.fileName = fileName;
            this.append = append;
        }

        internal override void Setup()
        {
            writer = new StreamWriter(fileName);
        }

        internal override void Teardown()
        {
            writer.Dispose();
        }

        protected override async Task WriteAsync(LogItem logItem)
        {
            var sb = new StringBuilder();

            sb.Append(logItem.LoggedOn.ToString("o"));
            sb.Append(',');
            sb.Append(logItem.ThreadId);
            sb.Append(',');
            sb.Append(logItem.Level);
            sb.Append(',');
            sb.Append(logItem.Message);

            await writer.WriteLineAsync(sb.ToString());
        }
    }
}
