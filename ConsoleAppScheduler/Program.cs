using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;


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

                var cmds = input.ToLower().Split(" ");
                // switch (@input)
                switch (cmds[0])
                {
                    case "exit":
                        Logs.WriteCurrent("[Main] User Exit");
                        Environment.Exit(-1);
                        break;
                    case "cancel":
                        Logs.WriteCurrent("[Main] Stop All");
                        foreach (var instanceAppEntity in Config.Instance.AppEntities)
                        {
                            if (instanceAppEntity.Enable)
                            {
                                instanceAppEntity.StopJob();
                            }
                        }

                        break;
                    case "help":
                        Console.WriteLine("help - Show Help Menu");
                        Console.WriteLine("exit - Exit Program");
                        Console.WriteLine("cancel - Remove All Job");
                        Console.WriteLine("reload - Reload Config File And Start All Job");
                        Console.WriteLine("show - Show The Running Job");
                        break;
                    case "reload":
                        foreach (var instanceAppEntity in Config.Instance.AppEntities)
                        {
                            if (instanceAppEntity.Enable)
                            {
                                instanceAppEntity.StopJob();
                            }
                        }

                        SchedulerList.Clear();
                        Config.Reload();
                        init();
                        //Console.WriteLine(Config.Instance.AppEntities.Count);
                        break;
                    case "show":
                        foreach (var instanceAppEntity in Config.Instance.AppEntities)
                        {
                            if (instanceAppEntity.Enable)
                            {
                                Logs.WriteCurrent(
                                    $"{Config.Instance.AppEntities.IndexOf(instanceAppEntity) + 1}. {instanceAppEntity.LogPrefix} is Running.");
                            }
                        }

                        break;
                    case "run":
                       // Logs.WriteCurrent("Please Enter The Index of The Application To Run");
                        //var position = Console.ReadLine();
                        if (cmds.Length > 1)
                        {
                            try
                            {
                                //Config.Instance.AppEntities[Convert.ToInt32(position) - 1].StartJob();
                                Config.Instance.AppEntities[Convert.ToInt32(cmds[1]) - 1].StartJob();
                            }
                            catch (Exception e)
                            {
                                Logs.WriteCurrent("Enter Integer Error!");
                            }
                        }

                        /*foreach (var instanceAppEntity in Config.Instance.AppEntities)
                        {1
                            if (instanceAppEntity.Enable)
                            {
                                Logs.WriteCurrent(instanceAppEntity.LogPrefix +" is Running.");
                            }
                        }*/
                        break;
                    default:
                        Logs.WriteCurrent("Command not Find!"); 
                        break;
                }
            }
            // Console.Read();
        }

        public static void init()
        {
            Logs.WriteCurrent("[Main] App Start");

            var i = 0;
            foreach (var instanceAppEntity in Config.Instance.AppEntities)
            {
                if (instanceAppEntity.Enable)
                {
                    Logs.WriteCurrent($"[Main] {instanceAppEntity.LogPrefix} Will Run");
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

            Logs.WriteCurrent($"[Main] {Config.Instance.AppEntities.Count(x => x.Enable)} Apps Will Run");
        }
    }
}