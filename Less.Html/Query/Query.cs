﻿//bibaoke.com

using Less.Collection;
using Less.Text;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Less.Html
{
    /// <summary>
    /// 查询器
    /// </summary>
    public partial class Query : IEnumerable<Element>
    {
        private Selector Selector
        {
            get;
            set;
        }

        /// <summary>
        /// 获取指定索引的元素
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public Element this[int index]
        {
            get
            {
                IEnumerable<Element> elements = this.Select();

                return elements.ToArray()[index];
            }
        }

        /// <summary>
        /// 查询到的元素数
        /// </summary>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public int length
        {
            get
            {
                return this.Select().ToArray().Length;
            }
        }

        internal Query(Selector selector)
        {
            this.Selector = selector;
        }

        /// <summary>
        /// 设置元素的样式
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        /// <exception cref="ArgumentNullException">name 不能为 null，value 不能为 null</exception>
        public Query css(string name, string value)
        {
            if (name.IsNull())
            {
                throw new ArgumentNullException("name");
            }

            if (value.IsNull())
            {
                throw new ArgumentNullException("value");
            }

            string style = this.attr("style");

            if (style.IsNull())
            {
                return this.attr("style", name + ":" + value);
            }
            else
            {
                Dictionary<string, string> dic = this.GetDic(style);

                List<string> list = new List<string>();

                string key = name.Trim();

                bool exists = false;

                foreach (KeyValuePair<string, string> i in dic)
                {
                    if (i.Key.CompareIgnoreCase(key))
                    {
                        list.Add(name + ":" + value);

                        exists = true;
                    }
                    else
                    {
                        list.Add(i.Key + ":" + i.Value);
                    }
                }

                if (!exists)
                {
                    list.Add(name + ":" + value);
                }

                this.attr("style", list.Join("; "));

                return this;
            }
        }

        /// <summary>
        /// 获取元素的样式
        /// </summary>
        /// <param name="name">样式名称</param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        /// <exception cref="ArgumentNullException">name 不能为 null</exception>
        public string css(string name)
        {
            if (name.IsNull())
            {
                throw new ArgumentNullException("name");
            }

            string style = this.attr("style");

            if (style.IsNull())
            {
                return null;
            }
            else
            {
                Dictionary<string, string> dic = this.GetDic(style);

                string value;

                if (dic.TryGetValue(name.Trim(), out value))
                {
                    return value;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 从元素移除一个或多个类
        /// </summary>
        /// <param name="classes"></param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        /// <exception cref="ArgumentNullException">classes 不能为 null</exception>
        public Query removeClass(string classes)
        {
            IEnumerable<Element> elements = this.Select();

            string[] removeArray = classes.SplitByWhiteSpace();

            foreach (Element i in elements)
            {
                string[] oriArray = new string[0];

                string name = "";

                if (i.className.IsNotNull())
                {
                    oriArray = i.className.SplitByWhiteSpace();

                    foreach (string j in oriArray)
                    {
                        if (!removeArray.Any(k => k.CompareIgnoreCase(j)))
                        {
                            name += j + Symbol.Space;
                        }
                    }
                }

                i.className = name.TrimEnd();
            }

            return this;
        }

        /// <summary>
        /// 向元素添加一个或多个类
        /// </summary>
        /// <param name="classes"></param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        /// <exception cref="ArgumentNullException">classes 不能为 null</exception>
        public Query addClass(string classes)
        {
            IEnumerable<Element> elements = this.Select();

            string[] addArray = classes.SplitByWhiteSpace();

            foreach (Element i in elements)
            {
                string[] oriArray = new string[0];

                string className = "";

                if (i.className.IsNotNull())
                {
                    oriArray = i.className.SplitByWhiteSpace();

                    className = i.className + Symbol.Space;
                }

                foreach (string j in addArray)
                {
                    if (!oriArray.Any(k => k.CompareIgnoreCase(j)))
                    {
                        className += j + Symbol.Space;
                    }
                }

                i.className = className.TrimEnd();
            }

            return this;
        }

        /// <summary>
        /// 返回元素的直接父元素
        /// </summary>
        /// <returns></returns>
        public Query parent()
        {
            var parent = this.Copy();

            parent.Selector.ExtFilterList.Add(selected => selected.Select(i => i.parentNode).GetElements());

            return parent;
        }

        /// <summary>
        /// 创建元素的副本
        /// </summary>
        /// <returns></returns>
        public Query clone()
        {
            IEnumerable<Element> elements = this.Select();

            List<Node> clone = new List<Node>();

            foreach (Element i in elements)
            {
                clone.Add(i.cloneNode(true));
            }

            var q = this.Selector.Rebind();

            return q(clone.ToArray());
        }

        /// <summary>
        /// 移除元素
        /// </summary>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public Query remove()
        {
            IEnumerable<Element> elements = this.Select();

            List<Node> list = new List<Node>();

            foreach (Element i in elements)
            {
                list.Add(i.parentNode.removeChild(i));
            }

            var q = this.Selector.Rebind();

            return q(list.ToArray());
        }

        /// <summary>
        /// 获取元素的 value 属性
        /// </summary>
        /// <returns></returns>
        public string val()
        {
            return this.attr("value");
        }

        /// <summary>
        /// 设置元素的 value 属性
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public Query val(object value)
        {
            this.val(value.ToString());

            return this;
        }

        /// <summary>
        /// 设置元素的 value 属性
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public Query val(string value)
        {
            this.attr("value", value);

            return this;
        }

        /// <summary>
        /// 移除元素的属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public Query removeAttr(string name)
        {
            IEnumerable<Element> elements = this.Select();

            foreach (Element i in elements)
            {
                i.removeAttribute(name);
            }

            return this;
        }

        /// <summary>
        /// 获取元素的属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public string attr(string name)
        {
            IEnumerable<Element> elements = this.Select();

            foreach (Element i in elements)
            {
                return i.getAttribute(name);
            }

            return null;
        }

        /// <summary>
        /// 设置元素的属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="value">属性值</param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public Query attr(string name, object value)
        {
            return this.attr(name, value.IsNull() ? "" : value.ToString());
        }

        /// <summary>
        /// 设置元素的属性
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="value">属性值</param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public Query attr(string name, string value)
        {
            IEnumerable<Element> elements = this.Select();

            foreach (Element i in elements)
            {
                i.setAttribute(name, value);
            }

            return this;
        }

        /// <summary>
        /// 根据表达式查找元素
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Query find(string expression)
        {
            var find = this.Copy();

            find.Selector.ExtFilterList.Add(selected =>
            {
                IEnumerable<Element> source = selected.GetChildElements();

                Document document = source.GetOwnerDocument();

                return find.Selector.Select(document, source, expression);
            });

            return find;
        }

        /// <summary>
        /// 在元素之后插入内容
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public Query after(params SelectorParam[] parameters)
        {
            foreach (SelectorParam i in parameters)
            {
                this.Operate(i,
                    (element, nodes) =>
                    {
                        Node after = element.nextSibling;

                        if (after.IsNotNull())
                        {
                            foreach (Node j in nodes)
                            {
                                element.parentNode.insertBefore(j, after);
                            }
                        }
                        else
                        {
                            foreach (Node j in nodes)
                            {
                                element.parentNode.appendChild(j);
                            }
                        }
                    },

                    (element, html) =>
                    {
                        Node after = element.nextSibling;

                        Node[] nodes = element.ownerDocument.Parse(html).childNodes;

                        if (after.IsNotNull())
                        {
                            foreach (Node j in nodes)
                            {
                                element.parentNode.insertBefore(j, after);
                            }
                        }
                        else
                        {
                            foreach (Node j in nodes)
                            {
                                element.parentNode.appendChild(j);
                            }
                        }
                    },

                    (element, query) =>
                    {
                        Node after = element.nextSibling;

                        IEnumerable<Element> elements = query.Select();

                        if (after.IsNotNull())
                        {
                            foreach (Element j in elements)
                            {
                                element.parentNode.insertBefore(j, after);
                            }
                        }
                        else
                        {
                            foreach (Element j in elements)
                            {
                                element.parentNode.appendChild(j);
                            }
                        }
                    });
            }

            return this;
        }

        /// <summary>
        /// 在元素之前插入内容
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public Query before(params SelectorParam[] parameters)
        {
            foreach (SelectorParam i in parameters)
            {
                this.Operate(i,
                    (element, nodes) =>
                    {
                        foreach (Node j in nodes)
                        {
                            element.parentNode.insertBefore(j, element);
                        }
                    },

                    (element, html) =>
                    {
                        foreach (Node j in element.ownerDocument.Parse(html).childNodes)
                        {
                            element.parentNode.insertBefore(j, element);
                        }
                    },

                    (element, query) =>
                    {
                        IEnumerable<Element> elements = query.Select();

                        foreach (Element j in elements)
                        {
                            element.parentNode.insertBefore(j, element);
                        }
                    });
            }

            return this;
        }

        /// <summary>
        /// 在元素的开头插入内容
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public Query prepend(params SelectorParam[] parameters)
        {
            foreach (SelectorParam i in parameters)
            {
                this.Operate(i,
                    (element, nodes) =>
                    {
                        nodes.EachDesc(j =>
                        {
                            if (element.hasChildNodes())
                            {
                                element.insertBefore(j, element.ChildNodeList[0]);
                            }
                            else
                            {
                                element.appendChild(j);
                            }
                        });
                    },

                    (element, html) =>
                    {
                        element.innerHTML = element.innerHTML.Insert(0, html);
                    },

                    (element, query) =>
                    {
                        IEnumerable<Element> elements = query.Select();

                        foreach (Element j in elements.Reverse())
                        {
                            if (element.hasChildNodes())
                            {
                                element.insertBefore(j, element.ChildNodeList[0]);
                            }
                            else
                            {
                                element.appendChild(j);
                            }
                        }
                    });
            }

            return this;
        }

        /// <summary>
        /// 在元素的结尾插入内容
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public Query append(params SelectorParam[] parameters)
        {
            foreach (SelectorParam i in parameters)
            {
                this.Operate(i,
                    (element, nodes) =>
                    {
                        foreach (Node j in nodes)
                        {
                            element.appendChild(j);
                        }
                    },

                    (element, html) =>
                    {
                        element.innerHTML += html;
                    },

                    (element, query) =>
                    {
                        IEnumerable<Element> elements = query.Select();

                        foreach (Element j in elements)
                        {
                            element.appendChild(j);
                        }
                    });
            }

            return this;
        }

        /// <summary>
        /// 删除元素的子元素
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public Query empty()
        {
            return this.html("");
        }

        /// <summary>
        /// 返回元素的 textContent 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public string text()
        {
            IEnumerable<Element> elements = this.Select();

            if (elements.IsEmpty())
            {
                return null;
            }
            else
            {
                string text = "";

                foreach (Element i in elements)
                {
                    text += i.textContent;
                }

                return text;
            }
        }

        /// <summary>
        /// 设置元素的 textContent
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public Query text(object content)
        {
            string text = content.IsNull("").ToString();

            IEnumerable<Element> elements = this.Select();

            foreach (Element i in elements)
            {
                i.textContent = text;
            }

            return this;
        }

        /// <summary>
        /// 设置元素的 textContent
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public Query text(string content)
        {
            if (content.IsNull())
            {
                content = "";
            }

            IEnumerable<Element> elements = this.Select();

            foreach (Element i in elements)
            {
                i.textContent = content;
            }

            return this;
        }

        /// <summary>
        /// 返回元素的 innerHTML
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public string html()
        {
            IEnumerable<Element> elements = this.Select();

            if (elements.IsEmpty())
            {
                return null;
            }
            else
            {
                string html = "";

                foreach (Element i in elements)
                {
                    html += i.innerHTML;
                }

                return html;
            }
        }

        /// <summary>
        /// 设置元素的 innerHTML
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public Query html(object html)
        {
            if (html.IsNull())
            {
                return this.html("");
            }
            else
            {
                return this.html(html.ToString());
            }
        }

        /// <summary>
        /// 设置元素的 innerHTML
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public Query html(SelectorParam param)
        {
            this.Operate(param,
                (element, nodes) =>
                {
                    element.childNodes = nodes;
                },

                (element, html) =>
                {
                    element.innerHTML = html;
                },

                (element, query) =>
                {
                    IEnumerable<Element> elements = query.Select();

                    element.childNodes = elements.ToArray();
                });

            return this;
        }

        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SelectorParamException">选择器参数错误</exception>
        public IEnumerator<Element> GetEnumerator()
        {
            return this.Select().GetEnumerator();
        }

        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Select().GetEnumerator();
        }

        internal IEnumerable<Element> Select()
        {
            return this.Selector.Select();
        }

        private Dictionary<string, string> GetDic(string style)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            string[] styleArray = style.Split(';');

            foreach (string i in styleArray)
            {
                int index = i.IndexOf(':');

                if (index > 0)
                {
                    if (index < i.Length)
                    {
                        string sName = i.Substring(0, index).Trim();
                        string sValue = i.Substring(index + 1).Trim();

                        if (sName.IsNotEmpty() && sValue.IsNotEmpty())
                        {
                            dic.Remove(sName);

                            dic.Add(sName, sValue);
                        }
                    }
                }
            }

            return dic;
        }

        private Query Copy()
        {
            var q = this.Selector.Rebind();

            var copy = q(this.Selector.Param);

            copy.Selector.ExtFilterList.AddRange(this.Selector.ExtFilterList);

            return copy;
        }

        private void Operate(
            SelectorParam param,
            Action<Element, Node[]> nodesAction, Action<Element, string> stringAction, Action<Element, Query> queryAction)
        {
            if (param.IsNotNull())
            {
                IEnumerable<Element> elements = this.Select();

                if (param.NodesValue.IsNotNull())
                {
                    foreach (Element i in elements)
                    {
                        nodesAction(i, param.NodesValue);
                    }
                }
                else if (param.StringValue.IsNotNull())
                {
                    foreach (Element i in elements)
                    {
                        stringAction(i, param.StringValue);
                    }
                }
                else
                {
                    foreach (Element i in elements)
                    {
                        queryAction(i, param.QueryValue);
                    }
                }
            }
        }
    }
}
