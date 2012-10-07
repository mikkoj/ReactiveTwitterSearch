using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace TwitterSearch
{
    public static class ReactiveHelpers
    {
        public static IObservable<T> ReplayLastByKey<T, TKey>(this IObservable<T> source, Func<T, TKey> keySelector)
        {
            var cache = new Dictionary<TKey, T>();
            return Observable.Defer(() => { lock (cache) return cache.Values.ToList().ToObservable(); })
                             .Concat(source.Do(s => { lock (cache) cache[keySelector(s)] = s; }));
        }

        public static IDisposable MergeInsertAtTop<T, TKey>(this ObservableCollection<T> col, IObservable<T> stream, Func<T, TKey> keySelector)
        {
            col.Clear();
            var lookupTable = new Dictionary<TKey, int>();
            return stream.ObserveOn(SynchronizationContext.Current).Subscribe(item =>
            {
                var key = keySelector(item);
                int index;
                if (!lookupTable.TryGetValue(key, out index))
                {
                    lookupTable[key] = col.Count;
                    col.Insert(0, item);
                }
                else
                {
                    col[index] = item;
                }
            }, e =>
            {
                Console.WriteLine(e.Message);
            });
        }

        public static IDisposable MergeInsertAtBottom<T, TKey>(this ObservableCollection<T> col, IObservable<T> stream, Func<T, TKey> keySelector)
        {
            col.Clear();
            var lookupTable = new Dictionary<TKey, int>();
            return stream.ObserveOn(SynchronizationContext.Current).Subscribe(item =>
            {
                var key = keySelector(item);
                int index;
                if (!lookupTable.TryGetValue(key, out index))
                {
                    lookupTable[key] = col.Count;
                    col.Add(item);
                }
                else
                {
                    col[index] = item;
                }
            }, e =>
            {
                Console.WriteLine(e.Message);
            });
        }
    }
}