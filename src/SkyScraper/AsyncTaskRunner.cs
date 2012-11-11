using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace SkyScraper
{
    public class AsyncTaskRunner : ITaskRunner
    {
        readonly ConcurrentDictionary<Guid, Task> tasks = new ConcurrentDictionary<Guid, Task>();

        public void Run(Action action)
        {
            var task = Task.Factory.StartNew(action);
            Run(task);
        }

        public async Task Run(Task task)
        {
            var guid = Guid.NewGuid();
            tasks.AddOrUpdate(guid, task, (g, t) => t);
            await task;
            Task outTask;
            tasks.TryRemove(guid, out outTask);
        }

        public void WaitForAllTasks()
        {
            Task.WaitAll(tasks.Values.ToArray());
        }
    }
}