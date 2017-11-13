using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Markdown
{
    public class StrongTag : Tag, ITagable
    {
        public StrongTag() : base("strong", "__")
        {
        }

        public string FindAndReplace(string markdown)
        {
            var isOpenTag = true;
            line = new StringBuilder(markdown);

            for (var i = 0; i < line.Length; i++)
            {
                if (IsScreened(line, i)) line = line.Remove(i++, 1);

                if (!IsDoubleUnderscore(line, i)) continue;

                if (IsSpaceBeforeUndescore(line, i, isOpenTag))
                {
                    i += 2;
                    continue;
                }

                if (IsSpaceAfterUndescore(line, i, isOpenTag))
                {
                    i += 2;
                    continue;
                }

                if (IsInsideDigit(line, i))
                {
                    i += 2;
                    continue;
                }

                stack.Push(i);
                isOpenTag = !isOpenTag;
                i += 1;
            }

            if (stack.Count % 2 != 0)
                stack.Pop();

            //CheckDoubleInsideSingleUnderscores();

            return Replace();
        }

        private void CheckDoubleInsideSingleUnderscores(StringBuilder line)
        {
            throw new System.NotImplementedException();
        }

        private static bool IsSpaceBeforeUndescore(StringBuilder line, int pos, bool isOpen) =>
            !isOpen && line[pos - 1] == ' ';

        private static bool IsSpaceAfterUndescore(StringBuilder line, int pos, bool isOpen) => 
            isOpen && pos + 2 < line.Length && line[pos + 2] == ' ';

        private static bool IsScreened(StringBuilder line, int pos) =>
            pos + 2 < line.Length && line[pos] == '\\' && line[pos + 1] == '_' && line[pos + 2] == '_';

        private static bool IsDoubleUnderscore(StringBuilder line, int pos) =>
            pos + 1 < line.Length && line[pos] == '_' && line[pos + 1] == '_';

        private static bool IsInsideDigit(StringBuilder line, int pos) =>
            pos > 0 && char.IsDigit(line[pos - 1]) || pos + 2 < line.Length && char.IsDigit(line[pos + 2]);
    }
}