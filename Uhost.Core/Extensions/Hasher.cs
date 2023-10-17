using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Uhost.Core.Extensions;

namespace Uhost.Core.Extensions
{
    public static class HasherExtensions
    {
        /// <summary>
        /// Предоставляет выбор алгоритма хэширования
        /// </summary>
        public enum EncryptionMethod
        {
            SHA256,
            MD5
        }

        /// <summary>
        /// Кодировка строки
        /// </summary>
        private static readonly Encoding _encoding = Encoding.UTF8;

        /// <summary>
        /// Считает хэш буфера в массив байтов
        /// </summary>
        /// <param name="input">Буфер</param>
        /// <param name="method">Алгоритм хэширования</param>
        /// <returns></returns>
        public static byte[] ComputeHash(this byte[] input, EncryptionMethod method)
        {
            switch (method)
            {
                case EncryptionMethod.SHA256:
                    using (var sha256 = SHA256.Create())
                    {
                        return sha256.ComputeHash(input);
                    }
                case EncryptionMethod.MD5:
                    using (var md5 = MD5.Create())
                    {
                        return md5.ComputeHash(input);
                    }
                default:
                    throw new ArgumentException("Bad encryption method specified", nameof(method));
            }
        }

        /// <summary>
        /// Считает хэш потока в массив байтов
        /// </summary>
        /// <param name="input">Входной поток</param>
        /// <param name="method">Алгоритм хэширования</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] ComputeHash(this Stream input, EncryptionMethod method)
        {
            switch (method)
            {
                case EncryptionMethod.SHA256:
                    using (var sha256 = SHA256.Create())
                    {
                        return sha256.ComputeHash(input);
                    }
                case EncryptionMethod.MD5:
                    using (var md5 = MD5.Create())
                    {
                        return md5.ComputeHash(input);
                    }
                default:
                    throw new ArgumentException("Bad encryption method specified", nameof(method));
            }
        }

        /// <summary>
        /// Считает хэш строки в строку
        /// </summary>
        /// <param name="input">Входная строка</param>
        /// <param name="method">Алгоритм хэширования</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string ComputeHash(this string input, EncryptionMethod method)
        {
            var buffer = _encoding.GetBytes(input);
            var result = ComputeHash(buffer, method);

            return result.ToHexString();
        }
    }
}
