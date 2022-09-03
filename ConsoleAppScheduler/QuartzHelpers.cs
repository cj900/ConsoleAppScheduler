using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace ConsoleAppScheduler
{
    public static class QuartzHelpers
    {
        public static async Task StartAsync<T>(TimeSpan ts, string jobName, string triggerName, string groupName,
            AppEntity appEntity) where T : IJob
        {
            JobDataMap jobDataMap = new JobDataMap();
            jobDataMap.Put("AppEntity", appEntity);
            DateTimeOffset runTime = DateBuilder.EvenSecondDate(DateTime.Now);
            IJobDetail job = JobBuilder.Create<T>().WithIdentity(jobName, groupName).UsingJobData(jobDataMap).Build();
            ITrigger trigger = TriggerBuilder.Create().WithIdentity(triggerName, groupName).StartAt(runTime)
                .WithSimpleSchedule(x => x.WithInterval(ts).RepeatForever()).Build();
            ISchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();
            scheduler.Context.Put("AppEntity", appEntity);
            await scheduler.ScheduleJob(job, trigger);
            await scheduler.Start();
        }

        public static async Task StartAsync<T>(string cronExp, string jobName, string triggerName, string groupName,
            AppEntity appEntity) where T : IJob
        {
            JobDataMap jobDataMap = new JobDataMap();
            jobDataMap.Put("AppEntity", appEntity);
            DateTimeOffset runTime = DateBuilder.EvenSecondDate(DateTime.Now);
            IJobDetail job = JobBuilder.Create<T>().WithIdentity(jobName, groupName).UsingJobData(jobDataMap).Build();
            ITrigger trigger = TriggerBuilder.Create().WithIdentity(triggerName, groupName).WithCronSchedule(cronExp)
                .StartAt(runTime).Build();
            ISchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();
            await scheduler.ScheduleJob(job, trigger);
            await scheduler.Start();
        }

        public static bool removeJob(String jobName, String triggerName, String GroupName)
        {
            var scheduler = new StdSchedulerFactory().GetScheduler().Result;
            var triggerKey = new TriggerKey(triggerName, GroupName);
            var jobKey = JobKey.Create(jobName, GroupName);
            try
            {
                scheduler.PauseJob(jobKey);
                scheduler.PauseTrigger(triggerKey); // 停止触发器
                var unscheduledJob = scheduler.UnscheduleJob(triggerKey).Result; // 移除触发器
                var deleteJob = scheduler.DeleteJob(jobKey).Result;// 删除任务
                return deleteJob || unscheduledJob;
            }
            catch (Exception e)
            {
                Logs.PrintLog(e);
                //  throw new RuntimeException(e);
            }
            return false;
        }
        
        public static bool StartJob(String jobName, String triggerName, String GroupName)
        {
            var scheduler = new StdSchedulerFactory().GetScheduler().Result;
          //  var triggerKey = new TriggerKey(triggerName, GroupName);
            var jobKey = JobKey.Create(jobName, GroupName);
            try
            {
                return scheduler.TriggerJob(jobKey).IsCompleted;
            }
            catch (Exception e)
            {
                Logs.PrintLog(e);
                //  throw new RuntimeException(e);
            }
            return false;
        }
    }
}