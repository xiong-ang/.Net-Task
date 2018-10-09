using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskExceptionHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            TestUnobseredTaskExceptionHandler();
            //TestExceptionAttriHandler();
            //TestWaitExcettionHandler();
            //TestResultExcettionHandler();
        }

        //https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskscheduler.unobservedtaskexception?redirectedfrom=MSDN&view=netframework-4.7.2
        static void TestUnobseredTaskExceptionHandler()
        {
            TaskScheduler.UnobservedTaskException += (object sender, UnobservedTaskExceptionEventArgs e) =>{
                PrintException(e.Exception.InnerException);
                e.SetObserved();
            };

            Task<int> t = Task.Run<int>(async () => await ExceptionTaskFunc());
            Thread.Sleep(5000);//Wait task to complete
        }

        static int TestExceptionAttriHandler()
        {
            Task<int> t1 = Task.Run<int>(async () => await ExceptionTaskFunc());
            Task<int> t2 = Task.Run<int>(async () => await ExceptionTaskFunc());
            Task<int> t3 = Task.Run<int>(async () => await ExceptionTaskFunc());

            Task<int>[] tasks = new Task<int>[] { t1, t2, t3 };

            #region test
            { 
                int index = Task.WaitAny(tasks);
                var firstTask = tasks[index];

                if (firstTask.Exception != null)
                    PrintException(firstTask.Exception.InnerException);
                return 0;
            }
            #endregion test

            #region normal workflow
#if false
            while (tasks.Length > 0)
            {
                int index = Task.WaitAny(tasks);
                var firstTask = tasks[index];

                if (firstTask.Exception == null)
                    return firstTask.Result;

                tasks = tasks.Where(t => t != firstTask).ToArray();
            }
            throw new ApplicationException("All Tasks failed!");
#endif
            #endregion normal workflow

        }

        static void TestWaitExcettionHandler()
        {
            Task<int> t = Task.Run<int>(async () => await ExceptionTaskFunc());

            try
            {
                //t.Wait();
                //Or
                Task.WaitAll(new Task<int>[] { t });
            }
            catch (AggregateException ae)
            {
                PrintException(ae.InnerException);
            }

        }

        static void TestResultExcettionHandler()
        {
            Task<int> t = Task.Run<int>(async()=> await ExceptionTaskFunc());

            try
            {
                var v = t.Result;///
            }
            catch (AggregateException ae)
            {
                PrintException(ae.InnerException);
            }

        }
       
        static async Task<int> ExceptionTaskFunc()
        {
            await Task.Delay(1000);

            int zero = 0;
            return 1 / zero;
        }

        static void PrintException(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
