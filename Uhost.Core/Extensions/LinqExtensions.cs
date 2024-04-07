﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Uhost.Core.Extensions
{
    public static class LinqExtensions
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Implementation of LinQ Sum for enumeration of <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="spans"></param>
        /// <returns></returns>
        public static TimeSpan Sum(this IEnumerable<TimeSpan> spans)
        {
            var value = new TimeSpan(0);

            return spans.Aggregate(value, (current, span) => current + span);
        }

        /// <summary>
        /// Implementation of LinQ Sum for enumeration of <see cref="TimeSpan"/>
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="enumerable">Object enumerable</param>
        /// <param name="selector">Selector function</param>
        /// <returns></returns>
        public static TimeSpan Sum<T>(this IEnumerable<T> enumerable, Func<T, TimeSpan> selector)
        {
            return enumerable.Select(e => selector.Invoke(e)).Sum();
        }

        /// <summary>
        /// Уничтожает коллекцию <typeparamref name="T"/>
        /// </summary>
        /// <param name="disposables">Коллекция <typeparamref name="T"/></param>
        public static void Dispose<T>(this IEnumerable<T> disposables) where T : IDisposable
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Выполняет действие <paramref name="action"/> для всех элементов <paramref name="enumerable"/> типа <typeparamref name="T"/>, для которх выполняется условие <paramref name="predicate"/>
        /// </summary>
        /// <typeparam name="T">Целевой тип перечисления</typeparam>
        /// <param name="enumerable">Элементы перечисления</param>
        /// <param name="action">Действие</param>
        /// <param name="predicate">Условие отбора</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action, Func<T, bool> predicate = null)
        {
            predicate ??= e => true;

            foreach (var item in enumerable.Where(predicate))
            {
                action.Invoke(item);
            }
        }

        /// <summary>
        /// Добавление множества элементов в коллекцию
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="items"></param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            items.ForEach(item => collection.Add(item));
        }

        /// <summary>
        /// Хотя бы одно значение из коллекции есть в коллекции
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="containable"></param>
        /// <returns></returns>
        public static bool ContainsAny<T>(this IEnumerable<T> container, IEnumerable<T> containable)
        {
            return containable.Any(e => container.Contains(e));
        }

        /// <summary>
        /// Хотя бы одно значение из коллекции есть в коллекции
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="containable"></param>
        /// <returns></returns>
        public static bool ContainsAny<T>(this IEnumerable<T> container, params T[] containable)
        {
            return container.ContainsAny(containable.AsEnumerable());
        }

        /// <summary>
        /// Все значения коллекции есть в коллекции
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="containable"></param>
        /// <returns></returns>
        public static bool ContainsAll<T>(this IEnumerable<T> container, IEnumerable<T> containable)
        {
            return containable.All(e => container.Contains(e));
        }

        /// <summary>
        /// Все значения коллекции есть в коллекции
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="containable"></param>
        /// <returns></returns>
        public static bool ContainsAll<T>(this IEnumerable<T> container, params T[] containable)
        {
            return container.ContainsAll(containable.AsEnumerable());
        }

        /// <summary>
        /// Фильтрует элементы перечисления, оставляя уникальные значения
        /// </summary>
        /// <typeparam name="TObject">Объект</typeparam>
        /// <typeparam name="TProperty">Свойство</typeparam>
        /// <param name="input">Исходное перечисление</param>
        /// <param name="selector">Выражение уникальности</param>
        /// <returns></returns>
        public static IEnumerable<TObject> DistinctBy<TObject, TProperty>(this IEnumerable<TObject> input, Func<TObject, TProperty> selector)
        {
            var tracker = new HashSet<TProperty>();

            foreach (var obj in input)
            {
                var value = selector.Invoke(obj);

                if (!tracker.Contains(value))
                {
                    tracker.Add(value);

                    yield return obj;
                }
            }

            tracker.Clear();
        }

        /// <summary>
        /// Добавление только уникальных объектов в коллекцию из перечисления
        /// </summary>
        /// <typeparam name="TObject">Объект</typeparam>
        /// <typeparam name="TProperty">Свойство</typeparam>
        /// <param name="collection">Исходная коллекция</param>
        /// <param name="values">Добавляемые элементы</param>
        /// <param name="distinstSelector">Выражение уникальности</param>
        public static void AddRangeDistinctBy<TObject, TProperty>(this ICollection<TObject> collection, IEnumerable<TObject> values, Func<TObject, TProperty> distinstSelector)
        {
            var toAdd = values.Where(e => !collection.Any(i => distinstSelector.Invoke(i).Equals(distinstSelector.Invoke(e))));

            collection.AddRange(toAdd);
        }

        /// <summary>
        /// Добавляет индекс к элементам перечисления
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static IEnumerable<(int Index, T Value)> AsIndexValueEnumerable<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Select((e, i) => (i, e));
        }

        /// <summary>
        /// Параллельная выборка по двум перечислениям. Выборка заканчивается с концом меньшего из двух перечислений, если они разной длины
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IEnumerable<(T1 Left, T2 Right)> ParallelSelect<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right)
        {
            var leftEnum = left.GetEnumerator();
            var rightEnum = right.GetEnumerator();

            try
            {
                while (leftEnum.MoveNext() && rightEnum.MoveNext())
                {
                    yield return (leftEnum.Current, rightEnum.Current);
                }
            }
            finally
            {
                leftEnum.Dispose();
                rightEnum.Dispose();
            }
        }

        public static IEnumerable<T> AsSingleElementEnumerable<T>(this T element)
        {
            yield return element;
        }

        public static void AddRange<T>(this ICollection<T> collection, params T[] items)
        {
            collection.AddRange(items.AsEnumerable());
        }

        public static IEnumerable<IEnumerable<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> source)
        {
            var enumerators = source.Select(i => i.GetEnumerator()).ToList();

            try
            {
                while (enumerators.All(e => e.MoveNext()))
                {
                    yield return enumerators.Select(e => e.Current).ToList();
                }
            }
            finally
            {
                foreach (var enumerator in enumerators)
                {
                    enumerator.Dispose();
                }
            }
        }

        /// <summary>
        /// Возвращает первый случайный элемент перечисления или значение по умолчанию, если перечисление пустое
        /// </summary>
        /// <typeparam name="T">Тип элемента</typeparam>
        /// <param name="enumerable">Перечисление</param>
        /// <param name="predicate">Доп. условие выборки</param>
        public static T RandomOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate = null)
        {
            predicate ??= e => true;

            return enumerable
                .OrderBy(e => _random.NextDouble())
                .FirstOrDefault(predicate);
        }

        /// <summary>
        /// Поворачивает коллекцию на указанное кол-во элементов
        /// </summary>
        /// <typeparam name="T">Тип элемента</typeparam>
        /// <param name="collection">Коллекция</param>
        /// <param name="count">Кол-во элементов</param>
        public static void Rotate<T>(this IList<T> collection, int count = 1)
        {
            if (collection.Count > 1)
            {
                for (var i = 0; i < count; i++)
                {
                    var item = collection[0];
                    collection.RemoveAt(0);
                    collection.Add(item);
                }
            }
        }

        /// <summary>
        /// Безопасная функция Max()
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static TResult SafeMax<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> selector)
        {
            if (enumerable.Any())
            {
                return enumerable.Max(selector);
            }

            return default;
        }
    }
}
