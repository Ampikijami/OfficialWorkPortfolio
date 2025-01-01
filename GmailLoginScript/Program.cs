using System.Threading.Tasks;

namespace GmailLoginScript
{
    class Program
    {
        static void Main(string[] args)
        {

            var task1 = Task.Factory.StartNew(() => { new Worker().ScriptExecutor(); });

            // 6/25/2023 i put
            // <TargetFramework>net5.0</TargetFramework>
            // instead of net472
            //net5.0
            // Wait for ALL tasks to finish
            
            // Control will block here until all 3 finish in parallel
            //4.7.2 dotnetframework version 4.7.2 is needed to be installed to get this to run.
            Task.WaitAll(new[] { task1 });


            //var task1 = Task.Factory.StartNew(() => { /*do something*/ });
            //var task2 = Task.Factory.StartNew(() => { /*do something*/ });
            //var task3 = Task.Factory.StartNew(() => { /*do something*/ });

            //// Wait for ALL tasks to finish
            //// Control will block here until all 3 finish in parallel
            //Task.WaitAll(new[] { task1, task2, task3 });
        }
    }
}