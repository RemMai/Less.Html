﻿//bibaoke.com

using System.Collections.Generic;
using System.Linq;

namespace Less.Html
{
    internal class FilterByAll : ElementFilter
    {
        protected override IEnumerable<Element> EvalThis(IEnumerable<Element> source)
        {
            return source.SelectMany(i => i.GetAllElements());
        }
    }
}
