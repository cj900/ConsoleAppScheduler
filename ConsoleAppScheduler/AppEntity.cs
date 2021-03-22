using System;


namespace ConsoleAppScheduler
{
    public class AppEntity
    {
        public string AppLocation { get; set; }
        public string AppArguments { get; set; }
        public string WorkingDirectory { get; set; }
        public string CronJob { get; set; }
        public string LogPrefix { get; set; }
        public bool ErrorOutput { get; set; }
        public bool StandardOutput { get; set; }

        public void Log(string msg)
        {
            if (!String.IsNullOrWhiteSpace(msg))
            {
                Logs.WriteCurrent($"[{LogPrefix}] {msg}");
            }
        }

        public void Log(Exception e)
        {
            Logs.WriteCurrent($"[{LogPrefix}][Error] "+e.GetExceptionFootprints());

        }
    }
}