using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace crontest
{
    class Program
    {
        static void Main(string[] args)
        {
            // trigger async evaluation
            RunProgram().GetAwaiter().GetResult();
            Console.WriteLine("Hello World!");
            // Console.ReadKey();
        }

        private static async Task RunProgram()
        {
            try
            {
                // 建立 scheduler
                StdSchedulerFactory factory = new StdSchedulerFactory();
                IScheduler scheduler = await factory.GetScheduler();

    
                // 建立 Job
                IJobDetail job = JobBuilder.Create<ShowDataTimeJob>()
                    .WithIdentity("job1", "group1")
                    .Build();
    
                // 建立 Trigger，每秒跑一次
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartNow()
                    // .WithSimpleSchedule(x => x
                    //     .WithIntervalInSeconds(5)
                    //     .RepeatForever())
                    .WithCronSchedule("0 45 15 * * ?") // 每秒重複
                    .Build();
    
                // 加入 ScheduleJob 中
                await scheduler.ScheduleJob(job, trigger);
    
                // 啟動
                await scheduler.Start();
    
                // 執行 10 秒
                await Task.Delay(TimeSpan.FromSeconds(100));
    
                // say goodbye
                await scheduler.Shutdown();
            }
            catch (SchedulerException se)
            {
                await Console.Error.WriteLineAsync(se.ToString());
            }
        }
    }

    public class ShowDataTimeJob :IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync($"現在時間 {DateTime.Now}");
            // await Task.Run(()=> {              
            //     Console.WriteLine("你好啊！");
            // });
        }

    }

    public class EricSimpleJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Hello Eric, Job executed.");
            return Task.CompletedTask;
        }
    }
}
