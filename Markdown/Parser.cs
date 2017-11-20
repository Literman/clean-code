using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Markdown
{
    public class Parser
    {
        private static readonly HashSet<char> register = new HashSet<char> { '_', '*' };
        private static readonly Dictionary<string, string> tags = new Dictionary<string, string>
        {
            ["_"] = "em",
            ["__"] = "strong",
            ["*"] = "bold"
        };

        private static readonly LinkedList<SpecialSymbol> specialSymbols = new LinkedList<SpecialSymbol>();
        private static readonly Stack<SpecialSymbol> openedSymbols = new Stack<SpecialSymbol>();

        private static StringBuilder result;

        public Parser()
        {
            result = new StringBuilder();
            specialSymbols.Clear();
            openedSymbols.Clear();
        }

        public Parser(params (string, string)[] options) : this()
        {
            register.Clear();
            tags.Clear();
            RegisterSpecialSymbols(options);
        }

        private static void RegisterSpecialSymbols(params (string Symbol, string Tag)[] options)
        {
            foreach (var pair in options)
            {
                tags[pair.Symbol] = pair.Tag;
                register.Add(pair.Symbol[0]);
            }
        }

        public string RenderToHtml(string line)
        {
            SetSpecialSymbols(line);
            UpdateSpecialSymbols();
            return GetRendedText(line);
        }

        private static void SetSpecialSymbols(string line)
        {
            var pos = 0;
            var isCloseTag = false;

            while (pos < line.Length)
            {
                var currentSymbol = line[pos];

                if (currentSymbol == '\\')
                {
                    pos += 2;
                    continue;
                }

                if (register.Contains(currentSymbol))
                {
                    pos += AddSpecialSymbolAndGetOffset(currentSymbol, isCloseTag, pos, line);
                    continue;
                }

                isCloseTag = !char.IsWhiteSpace(currentSymbol);
                pos++;
            }
        }

        private void UpdateSpecialSymbols()
        {
            var isSingle = false;
            foreach (var symbol in specialSymbols.ToArray())
            {
                if (symbol.MainSymbol == '_' && symbol.Size == 1)
                    isSingle = !symbol.IsCloseTag;
                if (symbol.MainSymbol == '_' && symbol.Size == 2 && isSingle)
                    specialSymbols.Remove(symbol);
            }
        }

        private static string GetRendedText(string line)
        {
            var pos = 0;
            while (pos < line.Length)
            {
                var prepos = pos;
                if (!(specialSymbols.Count > 0 && prepos == specialSymbols.First.Value.Position))
                {
                    if (line[pos] == '\\' && pos + 1 != line.Length)
                        pos++;

                    result.Append(line[pos]);
                    pos++;
                    continue;
                }

                while (specialSymbols.Count > 0 && prepos == specialSymbols.First.Value.Position)
                {
                    PastTag(specialSymbols.First.Value);
                    pos += specialSymbols.First.Value.Size;
                    specialSymbols.RemoveFirst();
                }
            }
            return result.ToString();
        }

        private static void AddValidSymbol(SpecialSymbol opened, SpecialSymbol closed)
        {
            if (specialSymbols.Count != 0 && specialSymbols.First.Value.Position >= opened.Position)
                specialSymbols.AddFirst(opened);
            else
                specialSymbols.AddLast(opened);

            specialSymbols.AddLast(closed);
        }

        private static void AddOpenedSymbol(char mainSymbol, bool isCloseTag, int position, int size)
        {
            var data = new SpecialSymbol(mainSymbol, isCloseTag, position, size);

            if (openedSymbols.Count > 0 && IsValidClosing(isCloseTag, mainSymbol, size))
                AddValidSymbol(openedSymbols.Pop(), data);
            else openedSymbols.Push(data);
        }

        private static int AddSpecialSymbolAndGetOffset(char currentSymbol, bool isCloseTag, int pos, string line)
        {
            var size = 1;
            while (pos + size < line.Length && currentSymbol == line[pos + size])
                size++;

            if (IsOpenTag(pos + size, line) != isCloseTag)
                AddOpenedSymbol(currentSymbol, isCloseTag, pos, size);
            return size;
        }

        private static void PastTag(SpecialSymbol specialSymbol)
        {
            var key = new string(specialSymbol.MainSymbol, specialSymbol.Size);
            if (!tags.ContainsKey(key))
                result.Append(key);
            else
                result.AppendFormat(specialSymbol.IsCloseTag ? "</{0}>" : "<{0}>", tags[key]);
        }

        private static bool IsOpenTag(int pos, string line)
        {
            while (pos < line.Length)
            {
                if (!register.Contains(line[pos]))
                    return !char.IsWhiteSpace(line[pos]);
                pos++;
            }
            return false;
        }

        private static bool IsValidClosing(bool isClosed, char mainSymbol, int size)
        {
            var symbol = openedSymbols.Peek();
            return !symbol.IsCloseTag && isClosed
                && symbol.MainSymbol == mainSymbol
                && symbol.Size == size;
        }
    }
}