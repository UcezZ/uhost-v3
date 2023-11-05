using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Uhost.Core.Extensions
{
    public static class DrawingExtensions
    {
        /// <summary>
        /// Пережимает картинку из потока в JPEG с указанным качеством <paramref name="quality"/>, в случае получения меньшего размера файла пишет данные в тот же поток и возвращает true
        /// </summary>
        /// <param name="stream">Поток изображения</param>
        /// <param name="quality">Качество JPEG [0..100]</param>
        public static bool CompressImage(this Stream stream, byte quality)
        {
            if (!stream.CanRead || !stream.CanSeek || !stream.CanWrite)
            {
                throw new ArgumentException("Stream is not accessible", nameof(stream));
            }

            Image img;

            try
            {
                stream.Position = 0;
                img = Image.FromStream(stream);
            }
            catch
            {
                return false;
            }

            using (var temp = new MemoryStream())
            using (img)
            {
                img.CompressImage(quality, temp);

                if (temp.Length < stream.Length)
                {
                    temp.Position = 0;
                    stream.SetLength(0);
                    temp.CopyTo(stream);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Пережимает картинку в JPEG с указанным качеством <paramref name="quality"/>
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="quality">Качество JPEG [0..100]</param>
        /// <param name="output">Выходной поток</param>
        public static void CompressImage(this Image image, byte quality, Stream output)
        {
            if (!output.CanSeek || !output.CanWrite)
            {
                throw new ArgumentException("Stream is inaccessible", nameof(output));
            }
            if (quality > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(quality));
            }

            var enc = new EncoderParameters
            {
                Param = new[]
                {
                    new EncoderParameter(Encoder.Quality, (long)quality)
                }
            };

            output.Position = 0;

            using (var bitmap = new Bitmap(image))
            {
                bitmap.Save(output, ImageFormat.Jpeg.GetEncoder(), enc);
            }
        }

        /// <summary>
        /// Получает <see cref="ImageCodecInfo"/>, соответствующий <see cref="ImageCodecInfo"/>.
        /// </summary>
        public static ImageCodecInfo GetEncoder(this ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();

            return codecs.FirstOrDefault(e => e.FormatID == format.Guid);
        }

        public static Size FitToMin(this Size size, int minSide)
        {
            if (size.Width > size.Height)
            {
                return size.FitTo(height: minSide);
            }
            else
            {
                return size.FitTo(width: minSide);
            }
        }

        public static Size FitTo(this Size size, int width = 0, int height = 0)
        {
            if (width > 0 && size.Width > width)
            {
                var mul = width / (double)size.Width;

                size.Width = (int)(size.Width * mul);
                size.Height = (int)(size.Height * mul);
            }
            if (height > 0 && size.Height > height)
            {
                var mul = height / (double)size.Height;

                size.Width = (int)(size.Width * mul);
                size.Height = (int)(size.Height * mul);
            }

            return size;
        }
    }
}
