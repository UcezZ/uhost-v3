﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Uhost.Core.Properties
{


    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class CoreStrings
    {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CoreStrings()
        {
        }

        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Uhost.Core.Properties.CoreStrings", typeof(CoreStrings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        /// <summary>
        ///   Ищет локализованную строку, похожую на Неизвестная ошибка.
        /// </summary>
        internal static string Common_Error_Common
        {
            get
            {
                return ResourceManager.GetString("Common_Error_Common", resourceCulture);
            }
        }

        /// <summary>
        ///   Ищет локализованную строку, похожую на Некорректное значение.
        /// </summary>
        internal static string Common_Error_Invalid
        {
            get
            {
                return ResourceManager.GetString("Common_Error_Invalid", resourceCulture);
            }
        }

        /// <summary>
        ///   Ищет локализованную строку, похожую на Заполните поле &quot;{0}&quot;.
        /// </summary>
        internal static string Common_Error_RequiredFmt
        {
            get
            {
                return ResourceManager.GetString("Common_Error_RequiredFmt", resourceCulture);
            }
        }

        /// <summary>
        ///   Ищет локализованную строку, похожую на Поле сортировки указано неверно, допустимые значения: {0}.
        /// </summary>
        internal static string Common_Error_SortBy
        {
            get
            {
                return ResourceManager.GetString("Common_Error_SortBy", resourceCulture);
            }
        }

        /// <summary>
        ///   Ищет локализованную строку, похожую на Направление сортировки указано неверно, допустимые значения: {0}.
        /// </summary>
        internal static string Common_Error_SortDirection
        {
            get
            {
                return ResourceManager.GetString("Common_Error_SortDirection", resourceCulture);
            }
        }

        /// <summary>
        ///   Ищет локализованную строку, похожую на Поле &quot;{0}&quot; должно быть не длиннее {1} символов.
        /// </summary>
        internal static string Common_Error_StringTooLongFmt
        {
            get
            {
                return ResourceManager.GetString("Common_Error_StringTooLongFmt", resourceCulture);
            }
        }

        /// <summary>
        ///   Ищет локализованную строку, похожую на Поле &quot;{0}&quot; должно быть не короче {1} символов.
        /// </summary>
        internal static string Common_Error_StringTooShortFmt
        {
            get
            {
                return ResourceManager.GetString("Common_Error_StringTooShortFmt", resourceCulture);
            }
        }

        /// <summary>
        ///   Ищет локализованную строку, похожую на Допустимые расширения: {0}.
        /// </summary>
        internal static string File_Error_InvalidExtension
        {
            get
            {
                return ResourceManager.GetString("File_Error_InvalidExtension", resourceCulture);
            }
        }

        /// <summary>
        ///   Ищет локализованную строку, похожую на Допустимые MIME: {0}.
        /// </summary>
        internal static string File_Error_InvalidMime
        {
            get
            {
                return ResourceManager.GetString("File_Error_InvalidMime", resourceCulture);
            }
        }

        /// <summary>
        ///   Ищет локализованную строку, похожую на Файл слишком большой, максимально разрешённый размер: {0}.
        /// </summary>
        internal static string File_Error_TooLarge
        {
            get
            {
                return ResourceManager.GetString("File_Error_TooLarge", resourceCulture);
            }
        }

        /// <summary>
        ///   Ищет локализованную строку, похожую на Тема указана неверно, допустимые значения: {0}.
        /// </summary>
        internal static string User_Error_ThemeFail
        {
            get
            {
                return ResourceManager.GetString("User_Error_ThemeFail", resourceCulture);
            }
        }
    }
}
