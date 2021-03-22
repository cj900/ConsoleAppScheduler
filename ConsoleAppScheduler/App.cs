using System;
using System.Diagnostics;
using System.Text;
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
                var process = Process.Start(processStartInfo);
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
                    // Logs.WriteCurrent(consoleString);
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