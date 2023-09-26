using System;
using System.IO;
using System.Text;

namespace Uhost.Core.Extensions
{
    /// <summary>
    /// Provides base type and stream IO extensions
    /// </summary>
    public static class BinaryExtensions
    {
        private static readonly Encoding _defaultEncoding = Encoding.UTF8;

        #region Read
        /// <summary>
        /// Reads byte from stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns>Byte value.</returns>
        public static byte ReadUInt8(this Stream stream)
        {
            if (stream.Position == stream.Length)
            {
                throw new EndOfStreamException();
            }

            return (byte)stream.ReadByte();
        }

        /// <summary>
        /// Reads 16-bit unsigned int from stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns>16-bit unsigned int value.</returns>
        public static ushort ReadUInt16(this Stream stream)
        {
            if (stream.Position == stream.Length)
            {
                throw new EndOfStreamException();
            }

            var buffer = new byte[2];
            _ = stream.Read(buffer, 0, 2);
            return BitConverter.ToUInt16(buffer, 0);
        }

        /// <summary>
        /// Reads 24-bit unsigned int from stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns>24-bit unsigned int value.</returns>
        public static int ReadUInt24(this Stream stream)
        {
            if (stream.Position == stream.Length)
            {
                throw new EndOfStreamException();
            }

            var buffer = new byte[4];
            _ = stream.Read(buffer, 0, 3);
            return BitConverter.ToInt32(buffer, 0);
        }

        /// <summary>
        /// Reads 32-bit unsigned int from stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns>32-bit unsigned int value.</returns>
        public static uint ReadUInt32(this Stream stream)
        {
            if (stream.Position == stream.Length)
            {
                throw new EndOfStreamException();
            }

            var buffer = new byte[4];
            _ = stream.Read(buffer, 0, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }

        /// <summary>
        /// Reads 32-bit signed int from stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns>32-bit signed int value.</returns>
        public static int ReadInt32(this Stream stream)
        {
            if (stream.Position == stream.Length)
            {
                throw new EndOfStreamException();
            }

            var buffer = new byte[4];
            _ = stream.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        /// <summary>
        /// Reads 48-bit unsigned int from stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns>32-bit unsigned int value.</returns>
        public static long ReadUInt48(this Stream stream)
        {
            if (stream.Position == stream.Length)
            {
                throw new EndOfStreamException();
            }

            var buffer = new byte[8];
            _ = stream.Read(buffer, 0, 6);
            buffer[6] = buffer[7] = 0;
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// Reads 64-bit unsigned int from stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns>64-bit unsigned int value.</returns>
        public static ulong ReadUInt64(this Stream stream)
        {
            if (stream.Position == stream.Length)
            {
                throw new EndOfStreamException();
            }

            var buffer = new byte[8];
            _ = stream.Read(buffer, 0, 8);
            return BitConverter.ToUInt64(buffer, 0);
        }

        /// <summary>
        /// Reads 64-bit signed int from stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns>64-bit signed int value.</returns>
        public static long ReadInt64(this Stream stream)
        {
            if (stream.Position == stream.Length)
            {
                throw new EndOfStreamException();
            }

            var buffer = new byte[8];
            _ = stream.Read(buffer, 0, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// Reads 8-bit length string from stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns>8-bit length string value.</returns>
        public static string ReadString(this Stream stream)
        {
            byte length = stream.ReadUInt8(), c;

            if (stream.Position + length > stream.Length)
            {
                throw new EndOfStreamException();
            }

            var buffer = new byte[length];
            c = (byte)stream.Read(buffer, 0, length);
            return _defaultEncoding.GetString(buffer, 0, c);
        }

        /// <summary>
        /// Reads 8-bit buffer from stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns>8-bit buffer value.</returns>
        public static byte[] ReadBuffer8(this Stream stream)
        {
            var length = stream.ReadUInt8();

            if (stream.Position + length > stream.Length)
            {
                throw new EndOfStreamException();
            }

            var buffer = new byte[length];

            if (stream.Read(buffer, 0, length) < length)
            {
                throw new EndOfStreamException();
            }

            return buffer;
        }

        /// <summary>
        /// Reads 16-bit buffer from stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns>16-bit buffer value.</returns>
        public static byte[] ReadBuffer16(this Stream stream)
        {
            var length = stream.ReadUInt16();

            if (stream.Position + length > stream.Length)
            {
                throw new EndOfStreamException();
            }

            var buffer = new byte[length];

            if (stream.Read(buffer, 0, length) < length)
            {
                throw new EndOfStreamException();
            }

            return buffer;
        }
        #endregion

        #region Write
        /// <summary>
        /// Writes byte to stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="value">Byte.</param>
        public static void WriteUInt8(this Stream stream, byte value) => stream.WriteByte(value);

        /// <summary>
        /// Writes 16-bit unsigned int to stream.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">16-bit unsigned int.</param>
        public static void WriteUInt16(this Stream stream, ushort value)
        {
            var buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, 2);
        }

        /// <summary>
        /// Writes 24-bit unsigned int to stream.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">24-bit unsigned int.</param>
        public static void WriteUInt24(this Stream stream, int value)
        {
            var buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, 3);
        }

        /// <summary>
        /// Writes 32-bit unsigned int to stream.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">32-bit unsigned int.</param>
        public static void WriteUInt32(this Stream stream, uint value)
        {
            var buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, 4);
        }

        /// <summary>
        /// Writes 32-bit signed int to stream.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">32-bit signed int.</param>
        /// <exception cref="NotImplementedException"></exception>
        public static void WriteInt32(this Stream stream, int value)
        {
            var buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, 4);
        }

        /// <summary>
        /// Writes 48-bit unsigned int to stream.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">64-bit signed int.</param>
        public static void WriteUInt48(this Stream stream, long value)
        {
            var buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, 6);
        }

        /// <summary>
        /// Writes 64-bit unsigned int to stream.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="Value">64-bit unsigned int.</param>
        public static void WriteUInt64(this Stream stream, ulong Value)
        {
            var buffer = BitConverter.GetBytes(Value);
            stream.Write(buffer, 0, 8);
        }

        /// <summary>
        /// Writes 64-bit signed int to stream.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="value">64-bit signed int.</param>
        public static void WriteInt64(this Stream stream, long value)
        {
            var buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, 8);
        }

        /// <summary>
        /// Writes 8-bit length string to stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="value">8-bit length string.</param>
        public static void WriteString(this Stream stream, string value)
        {
            var buffer = _defaultEncoding.GetBytes(value);

            if (buffer.Length > byte.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"String length might be below or equal { byte.MaxValue } bytes.");
            }

            byte length = (byte)buffer.Length;
            stream.WriteUInt8(length);
            stream.Write(buffer, 0, length);
        }

        /// <summary>
        /// Writes 8-bit buffer to stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="buffer">8-bit buffer.</param>
        public static void WriteBuffer8(this Stream stream, byte[] buffer)
        {
            if (buffer.Length > byte.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(buffer), $"Buffer length might be below or equal { byte.MaxValue } bytes.");
            }

            var length = (byte)buffer.Length;
            stream.WriteUInt8(length);
            stream.Write(buffer, 0, length);
        }

        /// <summary>
        /// Writes 16-bit buffer to stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="buffer">16-bit buffer.</param>
        public static void WriteBuffer16(this Stream stream, byte[] buffer)
        {
            if (buffer.Length > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(buffer), $"Buffer length might be below or equal { ushort.MaxValue } bytes.");
            }

            var length = (ushort)buffer.Length;
            stream.WriteUInt16(length);
            stream.Write(buffer, 0, length);
        }
        #endregion
    }
}
