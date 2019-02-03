using System;
using System.Diagnostics;
using System.Text;

namespace WendigoJaeger.TranslationTool
{
    public enum Category
    {
        Info,
        Warning,
        Error,
        Debug
    }

    public class Reporter
    {
        public event Action<string> OnLineOutput;

        public int Warnings { get; private set; }
        public int Errors { get; private set; }

        public bool HasErrors
        {
            get
            {
                return Errors > 0;
            }
        }

        public void Info(string line, params object[] args)
        {
            WriteLine(Category.Info, line, args);
        }

        public void Warning(string line, params object[] args)
        {
            WriteLine(Category.Warning, line, args);
        }

        public void Debug(string line, params object[] args)
        {
            WriteLine(Category.Debug, line, args);
        }

        public void Error(string line, params object[] args)
        {
            WriteLine(Category.Error, line, args);
        }

        public void WriteLine(Category category, string line, params object[] args)
    {
            switch (category)
            {
                case Category.Warning:
                    Warnings++;
                    break;
                case Category.Error:
                    Errors++;
                    break;
                default:
                    break;
            }

            StringBuilder builder = new StringBuilder();

            DateTime now = DateTime.Now;

            builder.Append(now.ToString("yyyy-MM-dd HH:mm:ss"));
            builder.AppendFormat(" [{0}] ", category.ToString().ToUpper());
            builder.AppendFormat(line, args);
            builder.AppendLine();

            OnLineOutput?.Invoke(builder.ToString());
        }

        public void ProcessOutputHandler(object sender, DataReceivedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                OnLineOutput?.Invoke(args.Data + '\n');
            }
        }
    }
}
