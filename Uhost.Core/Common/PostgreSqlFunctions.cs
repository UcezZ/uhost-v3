using System;

#pragma warning disable IDE0060 // Удалите неиспользуемый параметр
namespace Uhost.Core.Common
{
    public static class PostgreSqlFunctions
    {
        /// <summary>
        /// Преобразуется в SQL-выражение <c><paramref name="source"/> % <paramref name="target"/></c>.
        /// </summary>
        public static bool TrgmAreSimilar(string source, string target) => throw new NotImplementedException();

        /// <summary>
        /// Преобразуется в SQL-выражение <c>word_similarity(<paramref name="source"/>, <paramref name="target"/>)</c>.
        /// </summary>
        public static double TrgmWordSimilarity(string source, string target) => throw new NotImplementedException();

        /// <summary>
        /// Преобразуется в SQL-вырашение <c>RANDOM()</c>
        /// </summary>
        /// <returns></returns>
        public static double Random() => throw new NotImplementedException();
    }
}
#pragma warning restore IDE0060 // Удалите неиспользуемый параметр
