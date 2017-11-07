using System.Collections.Generic;
using System.Text;

namespace Markdown
{
    public abstract class Tag
    {
        protected string tag;
        protected string symbol;
        protected Stack<int> stack;
        protected StringBuilder line;

        protected Tag(string tag, string symbol)
        {
            this.tag = tag;
            this.symbol = symbol;
            this.stack = new Stack<int>();
        }

        public string Wrap(string input) => $"<{tag}>{input}</{tag}>";

        protected string Replace()
        {
            var isOpenTag = false;
            foreach (var element in stack)
            {
                line.Remove(element, symbol.Length);
                line.Insert(element, isOpenTag ? $"<{tag}>" : $"</{tag}>");

                isOpenTag = !isOpenTag;
            }
            return line.ToString();
        }
    }
}