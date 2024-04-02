using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Uhost.Core.Extensions;
using Uhost.Core.Properties;

namespace Uhost.Core.Attributes.Validation
{
    /// <summary>
    /// Валидация загружаемоого файла. Поддерживает коллекцию файлов
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class FormFileValidationAttribute : ValidationAttribute
    {
        private const int _defaultMaxFileSize = 5242880;
        private readonly long _maxFileSize;
        private readonly bool _allowNull;
        private readonly List<string> _mimes;
        private readonly List<string> _exts;

        /// <summary>
        /// <inheritdoc cref="FormFileValidationAttribute"/>
        /// </summary>
        /// <param name="maxFileSize">Максимальный размер файла в байтах</param>
        /// <param name="mime">Список допустимых MIME</param>
        /// <param name="ext">Список допустимых расширений</param>
        /// <param name="allowNull">Разрешить пустое значение</param>
        public FormFileValidationAttribute(long maxFileSize = _defaultMaxFileSize, string[] mime = null, string[] ext = null, bool allowNull = false)
        {
            _mimes = new List<string>();
            _exts = new List<string>();
            _allowNull = allowNull;
            _maxFileSize = maxFileSize;

            if (mime != null && mime.Any())
            {
                _mimes.AddRange(mime.Select(e => e.ToLower()));
            }
            if (ext != null && ext.Any())
            {
                _exts.AddRange(ext.Select(e => e.ToLower()));
            }
        }

        /// <summary>
        /// Валидация одного файла
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns></returns>
        private ValidationResult ValidateOne(IFormFile file)
        {
            if (file.Length > _maxFileSize)
            {
                return new ValidationResult(CoreStrings.File_Error_TooLarge.Format(_maxFileSize.ToHumanSize()));
            }

            if (_mimes.Any() && !_mimes.Contains(file.ContentType?.ToLower() ?? string.Empty))
            {
                return new ValidationResult(CoreStrings.File_Error_InvalidMime.Format(string.Join(", ", _mimes)));
            }

            if (_exts.Any() && !_exts.Contains(Path.GetExtension(file.FileName.ToLower())?[1..]))
            {
                return new ValidationResult(CoreStrings.File_Error_InvalidExtension.Format(string.Join(", ", _exts)));
            }

            return ValidationResult.Success;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null && _allowNull)
            {
                return ValidationResult.Success;
            }

            if (value is IFormFile file)
            {
                return ValidateOne(file);
            }
            else if (value is IFormFileCollection files)
            {
                if (!files.Any() && _allowNull)
                {
                    return ValidationResult.Success;
                }

                var results = files.Select(e => ValidateOne(e));

                var bad = results.FirstOrDefault(e => e != ValidationResult.Success);

                if (bad != null)
                {
                    return bad;
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                return new ValidationResult(CoreStrings.Common_Error_Common);
            }
        }
    }
}
