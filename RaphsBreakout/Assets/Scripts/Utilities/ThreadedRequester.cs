using System;
using System.Collections.Generic;
using System.Threading;

namespace Utilities
{
    public class ThreadedRequester : MonoSingleton<ThreadedRequester>
    {
        private Queue<ThreadInfo> _queue = new();

        public static void Request(Func<object> function, Action<object> callback)
        {
            Instance.Request_Internal(function, callback);
        }

        private void Request_Internal(Func<object> function, Action<object> callback)
        {
            ThreadStart threadStart = delegate
            {
                Thread(function, callback);
            };
            new Thread(threadStart).Start();
        }

        private void Thread(Func<object> function, Action<object> callback)
        {
            var result = function();
            lock (_queue)
            {
                _queue.Enqueue(new(callback, result));
            }
        }

        private void Update()
        {
            if (_queue.Count > 0)
            {
                for (var i = 0; i < _queue.Count; i++)
                {
                    var threadInfo = _queue.Dequeue();
                    threadInfo.Callback(threadInfo.Parameter);
                }
            }
        }

        private struct ThreadInfo
        {
            public readonly Action<object> Callback;
            public readonly object Parameter;

            public ThreadInfo(Action<object> callback, object parameter)
            {
                Callback = callback;
                Parameter = parameter;
            }
        }
    }
}