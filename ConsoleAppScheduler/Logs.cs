using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Threading;


namespace  ConsoleAppScheduler
{
    public static class Logs
    {
        // private static Logs instance = null;
        public static String LogFilePath=Config.Instance.LogFile;
        private static ReaderWriterLockSlim lock_ = new();

        public static void PrintLog(string Msg)
        {
            Console.Out.WriteLineAsync(FormatMessage(Msg));
        }
        public static void PrintLog(Exception e)
        {
            Console.Out.WriteLineAsync(FormatMessage(GetExceptionFootprints(e)));
        }
        public static String FormatMessage(String Msg)
        {
            return String.Format("[{0}] {1}", DateTime.Now.ToString("MM/dd/yyyy H:mm:ss"),Msg);
        }
        public static void Write(String FilePath, string Msg)
        {
            PrintLog(Msg);
            lock_.EnterWriteLock();
            try
            {
                using (StreamWriter outStream = File.AppendText(FilePath))
                {
                    outStream.WriteLine(FormatMessage(Msg));
                    outStream.Flush();
                    outStream.Close();
                }
            }
            finally
            {
                lock_.ExitWriteLock();
            }
            // new FileWriter().WriteData(Msg, Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"SampleFile.txt"));
            //fileWriter.WriteData(FormatMessage(Msg), FilePath);
            // using (StreamWriter outputFile = new StreamWriter(FilePath, true))
            // {
            //     outputFile.WriteLine(String.Format("[{0}] {1};", DateTime.Now.ToString("MM/dd/yyyy H:mm:ss"), Msg));
            //     outputFile.Flush();
            //     outputFile.Close();
            // }
        }
        // public static void WriteCurrent(String FileName, string Msg)
        // {
        //     Write(Environment.CurrentDirectory + "\\" + FileName, Msg);
        // }
        public static void WriteCurrent(Exception e)
        {
            Write(LogFilePath, GetExceptionFootprints(e));
        }
        public static void WriteCurrent(String msg)
        {
            Write(LogFilePath, msg);
        }

        public static void WriteCurrent(String msg, params String[] strAry)
        {
            Write(LogFilePath, String.Format(msg, strAry));
        }
        /// <summary>
        ///     Gets the entire stack trace consisting of exception's footprints (File, Method, LineNumber)
        /// </summary>
        /// <param name="exception">Source <see cref="Exception" /></param>
        /// <returns>
        ///     <see cref="string" /> that represents the entire stack trace consisting of exception's footprints (File,
        ///     Method, LineNumber)
        /// </returns>
        public static string GetExceptionFootprints(this Exception exception)
        {
            StackTrace stackTrace = new StackTrace(exception, true);
            StackFrame[] frames = stackTrace.GetFrames();

            var traceStringBuilder = new StringBuilder();

            for (var i = 0; i < frames.Length; i++)
            {
                StackFrame frame = frames[i];

                if (frame.GetFileLineNumber() < 1)
                    continue;

                traceStringBuilder.AppendLine($"File: {frame.GetFileName()}");
                traceStringBuilder.AppendLine($"Method: {frame.GetMethod().Name}");
                traceStringBuilder.AppendLine($"LineNumber: {frame.GetFileLineNumber()}");

                if (i == frames.Length - 1)
                    break;

                traceStringBuilder.AppendLine(" ---> ");
            }

            string stackTraceFootprints = traceStringBuilder.ToString();

            if (string.IsNullOrWhiteSpace(stackTraceFootprints))
                return "NO DETECTED FOOTPRINTS";

            return stackTraceFootprints;
        }


        // <summary>
        // Equivalent to Exception.ToString();
        // </summary>
        //public static string FlattenException(this Exception exception)
        //{
        //    var stringBuilder = new StringBuilder();

        //    while (exception != null)
        //    {
        //        stringBuilder.AppendLine(exception.Message);
        //        stringBuilder.AppendLine(exception.StackTrace);

        //        exception = exception.InnerException;
        //    }

        //    return stringBuilder.ToString();
        //}

    }

}