using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net;
using Xunit;

namespace Uhost.Tests.Extensions
{
    public static class AssertationExtensions
    {/// <summary>
     /// Performs common response assertations
     /// </summary>
     /// <param name="response">Controller response object</param>
     /// <returns>Response root JSON node</returns>
        internal static JObject CommonResponseAssertation(this IActionResult response, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            // Test for null
            Assert.NotNull(response);
            var viewResult = Assert.IsType<ContentResult>(response);

            // Test for MIME
            Assert.Equal("application/json;charset=utf-8", viewResult.ContentType);
            Assert.NotNull(viewResult.Content);

            // Test for HTTP code
            Assert.Equal((int)statusCode, viewResult.StatusCode);

            // Test for object content, getting root node
            var content = JObject.Parse(viewResult.Content);
            Assert.NotNull(content);
            return content;
        }

        /// <summary>
        /// Performs successful response assertaion
        /// </summary>
        /// <param name="jObject">Response root JSON node</param>
        /// <returns>Response "Result" JSON node</returns>
        internal static JToken SuccessResponseAssertation(this JObject jObject)
        {
            // Test for "Success" field
            var valueSuccess = jObject.Value<bool>("success");
            Assert.True(valueSuccess);

            // Test for "Result" field
            var nodeResult = jObject["result"];
            Assert.NotNull(nodeResult);
            return nodeResult;
        }

        /// <summary>
        /// Performs successful deletion response assertation.
        /// </summary>
        /// <param name="jObject">Response result JSON node</param>
        internal static void DeletedResponseAssertion(this JToken jObject)
        {
            var valueDeleted = jObject.Value<bool>("deleted");
            Assert.True(valueDeleted);
        }

        /// <summary>
        /// Performs successful response assertaion
        /// </summary>
        /// <param name="jObject">Response root JSON node</param>
        /// <returns>Response "Result" JSON node</returns>
        internal static JToken ErrorResponseAssertation(this JObject jObject)
        {
            // Test for "Success" field
            var valueSuccess = jObject.Value<bool>("success");
            Assert.False(valueSuccess);

            // Test for "Result" field
            var nodeResult = jObject["errors"];
            Assert.NotNull(nodeResult);
            return nodeResult;
        }

        /// <summary>
        /// Performs JSON array assertaion by key name.
        /// </summary>
        /// <param name="jToken">Parent node.</param>
        /// <param name="arrayKey">Name of child array node.</param>
        /// <param name="empty">Determines if JArray node might be empty.</param>
        /// <returns>JArray node.</returns>
        internal static JArray JArrayByKeyNameAssertation(this JToken jToken, string arrayKey, bool empty = false)
        {
            // Test for "arrayKey" field
            var nodeArray = jToken[arrayKey];
            Assert.NotNull(nodeArray);
            var items = Assert.IsType<JArray>(nodeArray);

            // Test for items count
            if (!empty)
            {
                Assert.NotEmpty(items);
            }

            return items;
        }

        /// <summary>
        /// Performs string assertation with not null and trim.
        /// </summary>
        /// <param name="jToken">Parent node.</param>
        /// <param name="key">Target node keyname.</param>
        /// <param name="expected">Expected string value.</param>
        /// <param name="trimChar">Char to be trimmed.</param>
        internal static void StringAssertation(this JToken jToken, string key, string expected, char trimChar = ' ') =>
            StringAssertation(jToken.Value<string>(key), expected, trimChar);

        /// <summary>
        /// Performs string assertation with not null and trim.
        /// </summary>
        /// <param name="actual">Actual value.</param>
        /// <param name="expected">Expected string value.</param>
        /// <param name="trimChar">Char to be trimmed.</param>
        internal static void StringAssertation(this string actual, string expected, char trimChar = ' ')
        {
            if (expected == null)
            {
                Assert.Null(actual);
            }
            else
            {
                Assert.NotNull(actual);
                Assert.Equal(expected, actual.Trim(trimChar));
            }
        }

        /// <summary>
        /// Performs string assertation with not null and trim.
        /// </summary>
        /// <param name="jToken">Parent node.</param>
        /// <param name="key">Target node keyname.</param>
        /// <param name="expected">Expected string value.</param>
        /// <param name="trimChar">Char to be trimmed.</param>
        internal static void StringAssertation(this JToken jToken, string expected, char trimChar = ' ') =>
            StringAssertation(jToken.Value<string>(), expected, trimChar);

        /// <summary>
        /// Performs string assertation with not null and trim.
        /// </summary>
        /// <param name="jToken">Parent node.</param>
        /// <param name="key">Target node keyname.</param>
        /// <param name="expected">Expected int value.</param>
        internal static void IntAssertation(this JToken jToken, string key, int expected)
        {
            var value = jToken.Value<int>(key);
            Assert.Equal(expected, value);
        }

        /// <summary>
        /// Performs string assertation with not null and trim.
        /// </summary>
        /// <param name="jToken">Parent node.</param>
        /// <param name="key">Target node keyname.</param>
        /// <param name="expected">Expected int value.</param>
        internal static void IntAssertation(this JToken jToken, int expected)
        {
            var value = jToken.Value<int>();
            Assert.Equal(expected, value);
        }

        /// <summary>
        /// Валидация на не null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jToken"></param>
        /// <param name="key"></param>
        internal static void TypeNotNullAssertation<T>(this JToken jToken, string key)
        {
            var raw = jToken.Value<T>(key);
            Assert.NotNull(raw);
            var obj = Assert.IsType<T>(raw);
            Assert.NotNull(obj);
        }
    }
}
