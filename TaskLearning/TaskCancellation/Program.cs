using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCancellation
{
    class Program
    {
        static void Main(string[] args)
        {
            TestTaskCancellation();
        }

        static CancellationTokenSource Cts = new CancellationTokenSource();

        static void TestTaskCancellation()
        {
            CancellationToken token = Cts.Token;

            Task<int> t1 = Task.Run<int>(async () => await TaskRunFunc(0, token), token);
            Task<int> t2 = Task.Run<int>(async () => await TaskRunFunc(100, token), token);
            Task.Run(() => {
                Thread.Sleep(3000);
                CancelTasks();
            });


            //Very import
            try
            {
                Task.WaitAll(new Task<int>[] { t1, t2 });

                Console.WriteLine("Task1 result: " + t1.Result);
                Console.WriteLine("Task2 result: " + t2.Result);
            }
            catch (AggregateException ae)
            {
                if(ae.InnerException is OperationCanceledException)
                    Console.WriteLine("Tasks are canceled!");
                else
                    Console.WriteLine("Unknown Error!");
            }
        }

        static async Task<int> TaskRunFunc(int value, CancellationToken token)
        {
            int times = 10;
            while (times-- > 0)
            {
                await Task.Delay(1000);
                Console.WriteLine(++value);

                //Very important
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
            }
            return value;
        }

        static void CancelTasks()
        {
            Cts.Cancel();
            Cts = new CancellationTokenSource(); //For next tasks
        }
    }
}
