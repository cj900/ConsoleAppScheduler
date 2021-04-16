using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ConsoleAppScheduler
{
    public sealed class Config
    {
        private static readonly IConfigurationBuilder Builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        private static readonly IConfigurationRoot AppConfig = Builder.Build();
        private static Config _instance = null;
        public static Config Instance => _instance ??= new Config();
        public string UpdateTime { get; set; }
        public string LogFile { get; set; }
        public List<AppEntity> AppEntities { get;set; }
        private Config()
        {
            InitConfig();
        }
        private void InitConfig()
        {
            UpdateTime = AppConfig["System:GoabalUpdateTime"];
            LogFile = AppConfig["System:LogFile"];
            AppEntities = AppConfig.GetSection("AppEntities").Get<List<AppEntity>>();
        }

        public static void Reload()
        {
            AppConfig.Reload();
            _instance = new Config();
        }
    }
}
