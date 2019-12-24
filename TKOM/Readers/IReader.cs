namespace TKOM.Readers
{
    public interface IReader
    {
        int Line { get; }
        int Column { get; }
        int CurrentSign { get; }
        void Read();
        void Rewind(int numSigns);
    }
}