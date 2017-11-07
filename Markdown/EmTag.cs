using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace Markdown
{
    public class EmTag : Tag, ITagable
    {
        public EmTag() : base("em", "_")
        {
        }

        public string FindAndReplace(string markdown)
        {
            var isOpenTag = true;
            line = new StringBuilder(markdown);

            for (var i = 0; i < line.Length; i++)
            {
                if (IsScrenned(line, i)) line = line.Remove(i++, 1);
                if (!IsUnderscore(line, i)) continue;

                if (IsDoubleUnderscore(line, i))
                {
                    i++;
                    continue;
                }

                if (IsSpaceBeforeUndescore(line, i, isOpenTag))
                    continue;

                if (IsSpaceAfterUndescore(line, i, isOpenTag))
                    continue;

                if (IsInsideDigit(line, i)) continue;

                stack.Push(i);
                isOpenTag = !isOpenTag;
            }

            if (stack.Count % 2 != 0) stack.Pop();

            return Replace();
        }      

        private static bool IsUnderscore(StringBuilder line, int pos) =>
            line[pos] == '_';

        private static bool IsScrenned(StringBuilder line, int pos) =>
            line[pos] == '\\' && (pos + 2 == line.Length || line[pos + 2] != '_') && line[pos + 1] == '_';

        private static bool IsDoubleUnderscore(StringBuilder line, int pos) => 
            pos + 1 < line.Length && line[pos + 1] == '_';

        private static bool IsSpaceBeforeUndescore(StringBuilder line, int pos, bool isOpen) =>
            !isOpen && line[pos - 1] == ' ';

        private static bool IsSpaceAfterUndescore(StringBuilder line, int pos, bool isOpen) =>
            isOpen && pos + 1 < line.Length && line[pos + 1] == ' ';

        private static bool IsInsideDigit(StringBuilder line, int pos) =>
            pos > 0 && char.IsDigit(line[pos - 1]) || pos + 1 < line.Length && char.IsDigit(line[pos + 1]);

    }
}