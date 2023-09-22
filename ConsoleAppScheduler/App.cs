using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace ConsoleAppScheduler
{
    public class App : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var appEntity = (AppEntity) context.JobDetail.JobDataMap.Get("AppEntity");
            try
            {
                var processStartInfo = new ProcessStartInfo();
                if (appEntity.WorkingDirectory != null)
                {
                    processStartInfo.WorkingDirectory = appEntity.WorkingDirectory;
                }

                processStartInfo.FileName = appEntity.AppLocation;
                processStartInfo.Arguments = appEntity.AppArguments;
                processStartInfo.RedirectStandardOutput = appEntity.StandardOutput;
                processStartInfo.RedirectStandardError = appEntity.ErrorOutput;
                // processStartInfo.StandardOutputEncoding =  Encoding.Default;
                processStartInfo.UseShellExecute = false;
                Process process =null;
//              Console.WriteLine((DateTimeOffset.Now.ToUniversalTime()-context.ScheduledFireTimeUtc).Value.TotalSeconds);
                if (appEntity.RandomStartupTime > 0)
                {
                    Random rnd = new Random();
                    int second  = rnd.Next(1, appEntity.RandomStartupTime );
                    Logs.WriteCurrent($"Wait {second} seconds to start");
                    Thread.Sleep(second*1000);
                    process= Process.Start(processStartInfo);
                }
                else
                {
                    process = Process.Start(processStartInfo);
                }
                if (process == null)
                {
                    Logs.WriteCurrent("Proccess App Cannot Start!");
                }
                else
                {
                    if (appEntity.StandardOutput)
                    {
                        process.OutputDataReceived +=
                            (s, dataReceivedEventArgs) => appEntity.Log(dataReceivedEventArgs.Data);
                        process.BeginOutputReadLine();
                    }
                    if (appEntity.ErrorOutput)
                    {
                        process.ErrorDataReceived += (s, dataReceivedEventArgs) =>
                            appEntity.Log("E: " + dataReceivedEventArgs.Data);
                        process.BeginErrorReadLine();
                    }
                    process.WaitForExit();
                    process.Dispose();
                    if (context.NextFireTimeUtc != null)
                        Logs.WriteCurrent(
                            $"[{appEntity.LogPrefix}] Next Run Time : {context.NextFireTimeUtc.Value.ToLocalTime()}");
                }
            }
            catch (Exception e)
            {
                appEntity.Log(e);
                // throw;
            }

            return Task.CompletedTask;
        }
    }
}