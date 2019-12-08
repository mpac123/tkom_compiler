using System;
using System.IO;

namespace TKOM.Parser
{
    public class Reader
    {
        private readonly StreamReader _streamReader;
        private string _buffer;
        private int _position;

        public Reader(StreamReader streamReader)
        {
            _streamReader = streamReader;
            _buffer = "";
            _position = -1;
        }

        public bool IsEndOfStream()
        {
            if (_position == _buffer.Length - 1
             && _streamReader.Peek() == -1)
             {
                 return true;
             }
             return false;
        }

        public int Peek()
        {
            if (_position > -2 && _buffer.Length > _position+1)
            {
                return _buffer[_position + 1];
            }
            return _streamReader.Peek();
        }

        public char Read()
        {
            char sign;
            if (_buffer.Length == 0 || _position == _buffer.Length - 1)
            {
                sign = (char) _streamReader.Read();
                _buffer += sign;
            }
            else
            {
                sign = _buffer[_position + 1];
            }
            _position += 1;
            return sign;
        }

        public void Rewind(int length)
        {
            _position -= length;
            if (_position < -1)
            {
                throw new Exception("Cannot rewind that much, buffer too short.");
            }
        }

        public void ClearBuffer()
        {
            if (_position == _buffer.Length - 1)
            {
                _buffer = "";
                _position = -1;
            }
        }
        
    }
}