﻿//bibaoke.com

using System.Collections.Generic;
using System.Net;

namespace Less.Html
{
    /// <summary>
    /// 元素
    /// </summary>
    public class Element : Node
    {
        private static HashSet<string> SingleElements;

        internal string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 是否单标签元素
        /// </summary>
        internal bool IsSingle
        {
            get;
            set;
        }

        internal int InnerBegin
        {
            get;
            set;
        }

        internal int InnerEnd
        {
            get;
            set;
        }

        internal int Index
        {
            get
            {
                return this.ownerDocument.all.IndexOf(this);
            }
        }

        /// <summary>
        /// 元素名称
        /// </summary>
        public override string nodeName
        {
            get
            {
                return this.Name.ToUpper();
            }
        }

        /// <summary>
        /// style 属性
        /// </summary>
        public string style
        {
            get
            {
                return this.getAttribute("style");
            }
            set
            {
                this.setAttribute("style", value);
            }
        }

        /// <summary>
        /// 元素 id
        /// </summary>
        public string id
        {
            get
            {
                return this.getAttribute("id");
            }
            set
            {
                this.setAttribute("id", value);
            }
        }

        /// <summary>
        /// class 属性
        /// </summary>
        public string className
        {
            get
            {
                return this.getAttribute("class");
            }
            set
            {
                this.setAttribute("class", value);
            }
        }

        /// <summary>
        /// 设置或返回节点及其后代的文本内容
        /// </summary>
        public string textContent
        {
            get
            {
                List<Node> list = this.GetAllNodes();

                DynamicString text = new DynamicString();

                foreach (Node i in list)
                {
                    if (i is Text)
                        text.Append(i.nodeValue);
                }

                return text;
            }
            set
            {
                this.innerHTML = WebUtility.HtmlEncode(value);
            }
        }

        /// <summary>
        /// 元素内容
        /// </summary>
        public string innerHTML
        {
            get
            {
                if (!this.IsSingle)
                    return this.ownerDocument.Content.Substring(this.InnerBegin, this.InnerEnd - this.InnerBegin + 1);

                return null;
            }
            set
            {
                this.childNodes = this.ownerDocument.Parse(value).childNodes;
            }
        }

        /// <summary>
        /// 属性集合
        /// </summary>
        public NamedNodeMap<Attr> attributes
        {
            get;
            protected set;
        }

        /// <summary>
        /// 元素是否拥有属性
        /// </summary>
        /// <returns></returns>
        public bool hasAttributes()
        {
            return this.attributes.Count > 0;
        }

        /// <summary>
        /// 返回元素节点的指定属性值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string getAttribute(string name)
        {
            Attr attr = this.getAttributeNode(name);

            return attr.IsNotNull() ? attr.value : null;
        }

        /// <summary>
        /// 返回指定的属性节点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Attr getAttributeNode(string name)
        {
            return this.attributes.getNamedItem(name);
        }

        /// <summary>
        /// 把指定属性设置或更改为指定值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void setAttribute(string name, string value)
        {
            this.setAttributeNode(new Attr(name, value, this.ownerDocument.Parse));
        }

        /// <summary>
        /// 设置或更改指定属性节点
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        public Attr setAttributeNode(Attr attr)
        {
            return this.attributes.setNamedItem(attr);
        }

        /// <summary>
        /// 从元素中移除指定属性
        /// </summary>
        /// <param name="name"></param>
        public void removeAttribute(string name)
        {
            this.attributes.removeNamedItem(name);
        }

        /// <summary>
        /// 移除指定的属性节点
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        public Attr removeAttributeNode(Attr attr)
        {
            return this.attributes.removeNamedItem(attr.name);
        }

        static Element()
        {
            Element.SingleElements = new HashSet<string>(new string[]
            {
                "!doctype", "meta", "link", "img"
            });
        }

        internal Element(string name)
        {
            this.attributes = new NamedNodeMap<Attr>(this);

            this.Name = name.ToLower();

            if (Element.SingleElements.Contains(name))
                this.IsSingle = true;
        }

        /// <summary>
        /// 克隆节点
        /// </summary>
        /// <param name="deep"></param>
        /// <returns></returns>
        public override Node cloneNode(bool deep)
        {
            Document document = this.ownerDocument.Parse(this.Content);

            Node element = document.firstChild;

            if (!deep)
            {
                foreach (Node i in element.childNodes)
                    element.removeChild(i);
            }

            return element;
        }

        internal void OnAttributesChange(int offset, bool inOpenTag)
        {
            this.End += offset;

            if (!this.IsSingle && inOpenTag)
            {
                this.InnerBegin += offset;
                this.InnerEnd += offset;
            }

            foreach (Node i in this.ChildNodeList)
                i.SetIndex(offset);

            this.ShiftNext(offset);

            this.ShiftParent(offset);
        }

        /// <summary>
        /// 设置元素的所属文档
        /// </summary>
        /// <param name="document"></param>
        protected override void OnSetOwnerDocument(Document document)
        {
            foreach (Attr i in this.attributes)
                i.ownerDocument = document;
        }

        /// <summary>
        /// 自检函数
        /// </summary>
        protected override void OnSelfCheck()
        {
            foreach (Attr i in this.attributes)
                i.SelfCheck();
        }

        /// <summary>
        /// 设置新的索引
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected override void OnSetIndex(int offset)
        {
            if (!this.IsSingle)
            {
                this.InnerBegin += offset;
                this.InnerEnd += offset;
            }

            foreach (Attr i in this.attributes)
                i.SetIndex(offset);
        }

        /// <summary>
        /// 作为下一个节点偏移索引
        /// </summary>
        /// <param name="offset"></param>
        protected override void OnShiftNext(int offset)
        {
            if (!this.IsSingle)
            {
                this.InnerBegin += offset;
                this.InnerEnd += offset;
            }

            if (this.hasAttributes())
                this.attributes[0].Shift(offset);

            //偏移本实例的子节点
            foreach (Node i in this.ChildNodeList)
                i.SetIndex(offset);
        }

        /// <summary>
        /// 作为父节点偏移索引
        /// </summary>
        /// <param name="offset"></param>
        protected override void OnShiftParent(int offset)
        {
            if (!this.IsSingle)
                this.InnerEnd += offset;

            this.ShiftAttributesInCloseTag(offset);
        }

        private void ShiftAttributesInCloseTag(int offset)
        {
            foreach (Attr i in this.attributes)
            {
                if (!i.InOpenTag)
                {
                    i.Shift(offset);

                    break;
                }
            }
        }
    }
}
