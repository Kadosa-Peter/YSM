using System;
using System.Text;

namespace Ysm.Core
{
    public class Error
    {
        public string ExceptionType { get; set; }

        public string AssemblyName { get; set; }

        public string ClassName { get; set; }

        public string MethodName { get; set; }

        public string Message { get; set; }

        public string InnerException { get; set; }

        public string Trace { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(Environment.NewLine);
            stringBuilder.AppendLine($"AssemblyName: {AssemblyName}");
            stringBuilder.AppendLine($"ClassName: {ClassName}");
            stringBuilder.AppendLine($"MethodName: {MethodName}");
            stringBuilder.AppendLine($"ExceptionType: {ExceptionType}");
            stringBuilder.AppendLine($"Message: {Message}");
            stringBuilder.AppendLine($"InnerException: {InnerException}");
            stringBuilder.AppendLine("Trace: " + Trace);
            stringBuilder.AppendLine(Environment.NewLine);

            return stringBuilder.ToString();
        }
    }
}
