using System;
using System.Threading;


namespace ConsoleAppScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            Logs.WriteCurrent("[Main] App Start");
            Logs.WriteCurrent($"[Main] {Config.Instance.AppEntities.Count} Apps Will Run");
            var i = 0;
            foreach (var instanceAppEntity in Config.Instance.AppEntities)
            {
                var job = QuartzHelpers.StartAsync<App>(instanceAppEntity.CronJob, 
                        "job"+i, "triggerName"+i, "groupName",instanceAppEntity);
                    i++;
            }
            Console.Read();
        }
    }
}