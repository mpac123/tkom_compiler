using System.IO;

namespace TKOM.Readers
{
    public class StringsReader : IReader
    {
        private string _string;
        private int _position;

        public StringsReader(string s)
        {
            Line = 0;
            Column = 0;
            _string = s;
            _position = 0;
            Read();
        }

        public int CurrentSign { get; private set; }

        public int Line { get; private set; }

        public int Column { get; private set; }

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
            if (CurrentSign == '\n')
            {
                Line += 1;
            }
            else
            {
                Column += 1;
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