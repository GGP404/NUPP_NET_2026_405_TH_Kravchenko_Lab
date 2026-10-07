using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShop.ConsoleApp
{
    public static class SynchronizationExamples
    {
        // Lock example
        public static void LockExample()
        {
            // Synchronization object
            object syncObject = new object();

            int counter = 0;

            // Parallel counter incrementing
            Parallel.For(
                0,
                1000,
                _ =>
                {
                    // Thread safety via lock
                    lock (syncObject)
                    {
                        counter++;
                    }
                });

            Console.WriteLine(
                $"Lock result: {counter}");
        }

        // SemaphoreSlim example
        public static async Task SemaphoreExampleAsync()
        {
            // Limit concurrent access to 2 threads
            using SemaphoreSlim semaphore = new SemaphoreSlim(2, 2);

            // Creating 5 asynchronous tasks
            Task[] tasks = Enumerable
                .Range(1, 5)
                .Select(async number =>
                {
                    // Wait for available slot
                    await semaphore.WaitAsync();

                    try
                    {
                        Console.WriteLine(
                            $"Semaphore -> Task {number} отримала доступ");

                        await Task.Delay(100);
                    }
                    finally
                    {
                        // Release slot
                        semaphore.Release();
                    }
                })
                .ToArray();

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);
        }

        // AutoResetEvent example
        public static void AutoResetEventExample()
        {
            // Initial state set to non-signaled (false)
            using AutoResetEvent autoResetEvent =
                new AutoResetEvent(false);

            // Background task execution
            Task task = Task.Run(() =>
            {
                Thread.Sleep(200);

                Console.WriteLine(
                    "AutoResetEvent -> сигнал");

                // Send signal to unblock main thread
                autoResetEvent.Set();
            });

            // Wait for signal from background task
            autoResetEvent.WaitOne();

            Console.WriteLine(
                "AutoResetEvent -> продовження");

            // Ensure task completion
            task.GetAwaiter().GetResult();
        }
    }
}