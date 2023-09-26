using System;
using System.Collections.Generic;
using System.Linq;

namespace Uhost.Core.Extensions
{
    public static class LinqExtensions
    {
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
            foreach (var value in containable)
            {
                if (container.Contains(value))
                {
                    return true;
                }
            }

            return false;
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
            foreach (var value in containable)
            {
                if (!container.Contains(value))
                {
                    return false;
                }
            }

            return true;
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
            var tracker = new List<TProperty>();

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
    }
}
