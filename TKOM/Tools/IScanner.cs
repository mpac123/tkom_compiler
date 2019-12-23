namespace TKOM.Tools
{
    public interface IScanner
    {
        void ReadNextToken();
        bool TryReadText();
        bool TryReadString();
    }
}