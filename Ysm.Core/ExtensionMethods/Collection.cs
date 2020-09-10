using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Ysm.Core
{
    public static partial class ExtensionMethods
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<T>(this ObservableCollection<T> @this, IEnumerable<T> source)
        {
            foreach (T t in source)
            {
                @this.Add(t);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<T>(this ConcurrentBag<T> @this, IEnumerable<T> source)
        {
            if (source == null) return;

            foreach (T element in source)
            {
                @this.Add(element);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> Wrap<T>(this T @this)
        {
            return new List<T> { @this };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Empty<T>(this IList<T> @this)
        {
            if (@this == null)
                return true;

            return !@this.Any();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEmpty<T>(this IList<T> @this)
        {
            if (@this == null)
                return false;

            return @this.Any();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConcurrentBag<T> ToConcurrent<T>(this IEnumerable<T> @this)
        {
            ConcurrentBag<T> target = new ConcurrentBag<T>(@this);

            return target;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEmpty<T>(this IEnumerable<T> @this)
        {
            if (@this == null)
                return false;

            return @this.Any();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddOrThrow<T>(this IList<T> @this, T t)
        {
            if (@this.Contains(t) == false)
                @this.Add(t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyToList<T>(this IEnumerable<T> @this, List<T> target)
        {
            if (@this == null)
                return;

            foreach (T t in @this)
            {
                target.Add(t);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T LastOrDefault<T>(this List<T> list)
        {
            if (list.Count == 0)
                return default(T);

            return list.Last();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveLast<T>(this List<T> list)
        {
            if (list.Count == 0)
                return;

            T item = list.Last();

            list.Remove(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TakeLast<T>(this List<T> list)
        {
            if (list.Count == 0)
                return default(T);

            return list.Last();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddNotNull<T>(this IList<T> list, T item)
        {
            if (item != null)
            {
                list.Add(item);
            }
        }

        public static void Sort<TSource, TKey>(this ObservableCollection<TSource> @this, Func<TSource, TKey> keySelector)
        {
            List<TSource> collection = @this.OrderBy(keySelector).ToList();
            @this.Clear();
            foreach (TSource t in collection)
            {
                @this.Add(t);
            }
        }

        public static void SortByDescending<TSource, TKey>(this ObservableCollection<TSource> @this, Func<TSource, TKey> keySelector)
        {
            List<TSource> collection = @this.OrderByDescending(keySelector).ToList();
            @this.Clear();
            foreach (TSource t in collection)
            {
                @this.Add(t);
            }
        }
    }
}
