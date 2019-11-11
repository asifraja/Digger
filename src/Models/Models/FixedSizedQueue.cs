using System.Collections.Concurrent;

namespace Common.Models
{
    public class FixedSizedQueue<T>
    {
        ConcurrentQueue<T> q = new ConcurrentQueue<T>();
        private readonly object lockObject = new object();

        public FixedSizedQueue(int limit)
        {
            Limit = limit;
        }

        private int Limit { get; set; }
        public void Enqueue(T obj)
        {
            q.Enqueue(obj);
            lock (lockObject)
            {
                while (q.Count > Limit && q.TryDequeue(out T overflow)) ;
            }
        }

        public T[] ToArray() { return q.ToArray(); }
    }
}
