using System.IO;

namespace TKOM.Readers
{
    public class StringsReader : IReader
    {
        private string _string;
        private int _position;

        public StringsReader(string s)
        {
            _string = s;
            _position = 0;
            Read();
        }

        public int CurrentSign { get; private set; }

        public void Read()
        {
            if (_string.Length <= _position)
            {
                CurrentSign = -1;
            }
            else
            {
                CurrentSign = _string[_position];
                _position++;
            }
        }

        public void Rewind(int numSigns)
        {
            _position -= numSigns;
            if (_position < 1)
            {
                throw new System.Exception("Attempt to rewind before the beginning of the stream");
            }
            CurrentSign = _string[_position - 1];
        }
    }
}