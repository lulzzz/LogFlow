using System.Threading.Tasks.Dataflow;

namespace LogFlow
{
    public static class TaskExtenders
    {
        public static void HandleCompletion(
            this IDataflowBlock source, IDataflowBlock target)
        {
            source.Completion.ContinueWith(
                task =>
                {
                    if (task.IsFaulted)
                        target.Fault(task.Exception);
                    else
                        target.Complete();
                });
        }
    }
}
