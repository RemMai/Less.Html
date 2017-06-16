﻿//bibaoke.com

using System.Collections.Generic;
using Less.Collection;

namespace Less.Html
{
    /// <summary>
    /// 节点扩展方法
    /// </summary>
    internal static class NodeExtensions
    {
        /// <summary>
        /// 获取节点中的元素
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        internal static IEnumerable<Element> GetElements(this IEnumerable<Node> nodes)
        {
            return nodes.Select<Element>();
        }
    }
}
