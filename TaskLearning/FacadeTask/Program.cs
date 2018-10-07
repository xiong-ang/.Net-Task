using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FacadeTask
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestFacadeTask(GetTaskStatus);
            TestControlTaskResult(GetTaskStatus);

            Thread.Sleep(100);
        }

        static void TestFacadeTask(Action<Task> action)
        {
            TaskCompletionSource<int> getRandom = new TaskCompletionSource<int>();
            Task<int> facadeTask = getRandom.Task;

            //Task.Factory.StartNew = new Task + Task.Start
            Task.Factory.StartNew(() =>
            {
                //执行操作或task
                int value = (new Random()).Next(100);
                Console.WriteLine("Value: " + value);

                //控制facade的执行结果
                if (value < 33)
                    getRandom.SetException(new Exception("Less than 30"));
                else if (value < 66)
                    getRandom.SetCanceled();
                else
                    getRandom.SetResult(value);
            })
            //facade的执行结果表现在facadeTask
            .ContinueWith(t => {
                action(facadeTask);
            });
        }

        static void TestControlTaskResult(Action<Task> action)
        {
            Task.Factory.StartNew(async() =>
            {
                int value = (new Random()).Next(100);
                Console.WriteLine("Value: " + value);

                //控制facade的执行结果
                if (value < 33)
                    await Task.FromException(new Exception("Less than 33"));
                else if (value < 66)
                    await Task.FromCanceled(new CancellationToken());
            })
            .ContinueWith(action);
        }

        static void GetTaskStatus(Task t)
        {
            if (t.IsCanceled)
                Console.WriteLine("Canceled");
            else if (t.IsFaulted)
                Console.WriteLine("Faulted");
            else
                Console.WriteLine("Succeed");
        }
    }
}
