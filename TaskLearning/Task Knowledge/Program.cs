using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Knowledge
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestTaskWait();
            TestTaskResult();
        }

        static void TestTaskWait()
        {
            string value = "Ready";
            Task t = Task.Run(async() => {
                await Task.Delay(5000);
                value = "Done";
            });

            Task.Run(async() => {
                await Task.Delay(5000);
                t.Wait();
                Console.WriteLine(value);
            })
            .Wait();
        }

        static void TestTaskResult()
        {
            Task<string> t = Task.Run<string>(async() => {
                await Task.Delay(5000);
                return "Done";
            });
            
            Task.Run(async () =>
            {
                await Task.Delay(5000);

                //Implicitly wait
                Console.WriteLine(t.Result);
            })
            .Wait();
        }
    }
}
