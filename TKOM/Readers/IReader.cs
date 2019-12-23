namespace TKOM.Readers 
{
    public interface IReader
    {
        int CurrentSign {get;}
        void Read();
        void Rewind(int numSigns);
    }
}