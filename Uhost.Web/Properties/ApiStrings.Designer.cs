﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Uhost.Web.Properties {
    using System;
    
    
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
    internal class ApiStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ApiStrings() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Uhost.Web.Properties.ApiStrings", typeof(ApiStrings).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Пользователь {0} заблокирован {1} пользователем {2} по причине &quot;{3}&quot;.
        /// </summary>
        internal static string Auth_Error_BlockFmt {
            get {
                return ResourceManager.GetString("Auth_Error_BlockFmt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Неверный логин или пароль.
        /// </summary>
        internal static string Auth_Error_InvalidCredentials {
            get {
                return ResourceManager.GetString("Auth_Error_InvalidCredentials", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Пользователь не авторизован.
        /// </summary>
        internal static string Auth_Error_Unauthorized {
            get {
                return ResourceManager.GetString("Auth_Error_Unauthorized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Неопознанная ошибка.
        /// </summary>
        internal static string Common_Error_Common {
            get {
                return ResourceManager.GetString("Common_Error_Common", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Некорректное значение.
        /// </summary>
        internal static string Common_Error_Invalid {
            get {
                return ResourceManager.GetString("Common_Error_Invalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Слишком много запросов.
        /// </summary>
        internal static string Common_Error_TooManyRequests {
            get {
                return ResourceManager.GetString("Common_Error_TooManyRequests", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Пользователь не авторизован.
        /// </summary>
        internal static string Common_Error_Unauthorized {
            get {
                return ResourceManager.GetString("Common_Error_Unauthorized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Файл с таким ид. не найден.
        /// </summary>
        internal static string File_Error_NotFoundById {
            get {
                return ResourceManager.GetString("File_Error_NotFoundById", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на &lt;head&gt;&lt;meta charset=&quot;UTF-8&quot;&gt;&lt;style&gt;*{font-family:-apple-system,BlinkMacSystemFont,&apos;Segoe UI&apos;,Roboto,Oxygen,Ubuntu,Cantarell,&apos;Open Sans&apos;,&apos;Helvetica Neue&apos;,sans-serif;color:#173647;}div.button-wrapper{display:flex;justify-content:center;}a.swagger-link{font-size:48px;line-height:48px;font-weight:900;width:96px;height:96px;border-radius:96px;background-color:#85ea2d;padding:16px;position:relative;overflow:hidden;transition:all 0.3s ease-out;user-select:none;box-shadow:2px 2px 8px 2px #17364788;cursor:pointer;}a [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string Home_Index {
            get {
                return ResourceManager.GetString("Home_Index", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Право с ид. {0} не найдено.
        /// </summary>
        internal static string Right_Error_NotFoundByIdFmt {
            get {
                return ResourceManager.GetString("Right_Error_NotFoundByIdFmt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Пользователь должен иметь следующие права: {0}.
        /// </summary>
        internal static string Right_Error_ShouldHaveAll {
            get {
                return ResourceManager.GetString("Right_Error_ShouldHaveAll", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Пользователь должен иметь хотя бы одно из следующих прав: {0}.
        /// </summary>
        internal static string Right_Error_ShouldHaveAny {
            get {
                return ResourceManager.GetString("Right_Error_ShouldHaveAny", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Роль с таким именем уже существует.
        /// </summary>
        internal static string Role_Error_AlreadyExists {
            get {
                return ResourceManager.GetString("Role_Error_AlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Роль с даким ид. не найдена.
        /// </summary>
        internal static string Role_Error_NotFoundById {
            get {
                return ResourceManager.GetString("Role_Error_NotFoundById", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Роль с ид. {0} не найдена.
        /// </summary>
        internal static string Role_Error_NotFoundByIdFmt {
            get {
                return ResourceManager.GetString("Role_Error_NotFoundByIdFmt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Пользователь с таким логином уже существует.
        /// </summary>
        internal static string User_Error_AlreadyExists {
            get {
                return ResourceManager.GetString("User_Error_AlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Пользователь с таким ид. не найден.
        /// </summary>
        internal static string User_Error_NotFoundById {
            get {
                return ResourceManager.GetString("User_Error_NotFoundById", resourceCulture);
            }
        }
    }
}
