namespace Markdown
{
    public class SpecialSymbol
    {
        public readonly char MainSymbol;
        public readonly bool IsCloseTag;
        public readonly int Position;
        public readonly int Size;

        public SpecialSymbol(char mainSymbol, bool isCloseTag, int position, int size)
        {
            MainSymbol = mainSymbol;
            IsCloseTag = isCloseTag;
            Position = position;
            Size = size;
        }
    }
}