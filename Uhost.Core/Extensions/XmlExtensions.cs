using System;
using System.Collections.Generic;
using System.Xml;

namespace Uhost.Core.Extensions
{
    internal static class XmlExtensions
    {
        /// <summary>
        /// Implementation of Linq FirstOrDefault for <see cref="XmlNodeList"/>.
        /// </summary>
        /// <param name="xmlNodeList">Source node list.</param>
        /// <param name="predicate">Predicate.</param>
        /// <returns></returns>
        public static XmlNode FirstOrDefault(this XmlNodeList xmlNodeList, Func<XmlNode, bool> predicate = null)
        {
            if (xmlNodeList == null)
            {
                return null;
            }
            if (predicate == null)
            {
                return xmlNodeList.Count == 0 ? null : xmlNodeList[0];
            }

            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                if (predicate.Invoke(xmlNodeList[i]))
                {
                    return xmlNodeList[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Implementation of Linq LastOrDefault for <see cref="XmlNodeList"/>.
        /// </summary>
        /// <param name="xmlNodeList">Source node list.</param>
        /// <param name="predicate">Predicate.</param>
        /// <returns></returns>
        public static XmlNode LastOrDefault(this XmlNodeList xmlNodeList, Func<XmlNode, bool> predicate = null)
        {
            if (xmlNodeList == null)
            {
                return null;
            }
            if (predicate == null)
            {
                return xmlNodeList.Count == 0 ? null : xmlNodeList[^1];
            }

            for (int i = xmlNodeList.Count - 1; i >= 0; i--)
            {
                if (predicate.Invoke(xmlNodeList[i]))
                {
                    return xmlNodeList[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Implementation of Linq Where for <see cref="XmlNodeList"/>.
        /// </summary>
        /// <param name="xmlNodeList">Source node list.</param>
        /// <param name="predicate">Predicate.</param>
        /// <returns></returns>
        public static IEnumerable<XmlNode> Where(this XmlNodeList xmlNodeList, Func<XmlNode, bool> predicate)
        {
            if (xmlNodeList == null)
            {
                yield break;
            }
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                if (predicate.Invoke(xmlNodeList[i]))
                {
                    yield return xmlNodeList[i];
                }
            }
        }

        public static XmlNode FirstOrDefault(this XmlNode xmlNode, Func<XmlNode, bool> predicate = null) =>
            (xmlNode?.ChildNodes).FirstOrDefault(predicate);

        public static XmlNode LastOrDefault(this XmlNode xmlNode, Func<XmlNode, bool> predicate = null) =>
            (xmlNode?.ChildNodes).LastOrDefault(predicate);

        public static IEnumerable<XmlNode> Where(this XmlNode xmlNode, Func<XmlNode, bool> predicate) =>
            (xmlNode?.ChildNodes).Where(predicate);
    }
}
