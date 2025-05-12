using System;
using System.Threading.Tasks;

namespace Core.Utilities
{
    public static class EventAwaiter
    {
        public static Task AwaitEvent(Action<Action> subscribe, Action<Action> unsubscribe)
        {
            var tcs = new TaskCompletionSource<bool>(); 
            Action handler = null;
            handler = () =>
            {
                unsubscribe(handler);
                tcs.SetResult(true);
            };

            subscribe(handler);

            return tcs.Task;
        }

        public static Task<T> AwaitEvent<T>(Action<Action<T>> subscribe, Action<Action<T>> unsubscribe)
        {
            var tcs = new TaskCompletionSource<T>();

            Action<T> handler = null;
            handler = (arg) =>
            {
                unsubscribe(handler);
                tcs.SetResult(arg);
            };

            subscribe(handler);

            return tcs.Task;
        }
    }
}