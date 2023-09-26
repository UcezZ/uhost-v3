using System;

namespace Uhost.Web.Attributes
{
    /// <summary>
    /// Ограничение количества запросов за промежуток времени
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ThrottleAttribute : Attribute
    {
        private int _count = 60;
        private int _spanSeconds = 15;

        /// <summary>
        /// Количество запросов
        /// </summary>
        public int Count
        {
            get => _count;
            set => _count = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(Count));
        }

        /// <summary>
        /// Промежуток времени в секундах
        /// </summary>
        public int SpanSeconds
        {
            get => _spanSeconds;
            set => _spanSeconds = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(SpanSeconds));
        }

        /// <summary>
        /// Промежуток времени
        /// </summary>
        public TimeSpan Span => TimeSpan.FromSeconds(SpanSeconds);
    }
}
