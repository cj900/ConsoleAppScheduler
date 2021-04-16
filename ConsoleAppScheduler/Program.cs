using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace ConsoleAppScheduler
{
    static class Program
    {
        static List<Task> SchedulerList = new List<Task>();
        
        static void Main(string[] args)
        {
            init();
            while (true)
            {
                var input = Console.ReadLine();
                switch (@input)
                {
                    case "exit":
                        Logs.WriteCurrent("[Main] User Exit");
                        Environment.Exit(-1);
                        break;
                    case "cancel":
                        Logs.WriteCurrent("[Main] Stop All");
                        foreach (var instanceAppEntity in Config.Instance.AppEntities)
                        {
                            instanceAppEntity.StopJob();
                        }
                        break;
                    case "help":
                        Console.WriteLine("help - Show Help Menu");
                        Console.WriteLine("exit - Exit Program");
                        Console.WriteLine("cancel - Remove All Job");
                        Console.WriteLine("reload - Reload Config File And Start All Job");
                        break;
                    case "reload":
                        foreach (var instanceAppEntity in Config.Instance.AppEntities)
                        {
                            instanceAppEntity.StopJob();
                        }
                        SchedulerList.Clear();
                        Config.Reload();
                        init();
                        //Console.WriteLine(Config.Instance.AppEntities.Count);
                        break;
                    default:
                      //  Logs.WriteCurrent("[Main] User Exit");
                        break;
                }
            }
            // Console.Read();
        }

        public static void init()
        {
            Logs.WriteCurrent("[Main] App Start");
            Logs.WriteCurrent($"[Main] {Config.Instance.AppEntities.Count} Apps Will Run");
            var i = 0;
            foreach (var instanceAppEntity in Config.Instance.AppEntities)
            {
                instanceAppEntity.JobName = $"Job{i}";
                instanceAppEntity.TriggerName = $"TriggerName{i}";
                instanceAppEntity.GroupName = "group0";
                var job = QuartzHelpers.StartAsync<App>(instanceAppEntity.CronJob,
                    instanceAppEntity.JobName, instanceAppEntity.TriggerName, 
                    instanceAppEntity.GroupName, instanceAppEntity);
                SchedulerList.Add(job);
                i++;
            }
        }
    }
}