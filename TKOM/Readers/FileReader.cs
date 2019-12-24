using System;
using System.IO;

namespace TKOM.Readers
{
    public class FileReader : IReader, IDisposable
    {
        bool disposed = false;
        private FileStream _fstream;

        public int CurrentSign {get; private set;}

        public int Line {get; private set;} 

        public int Column {get; private set;} 

        public FileReader(string path)
        {
            Line = 0;
            Column = 0;
            _fstream = File.OpenRead(path);
            Read();
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                _fstream.Dispose();
            }
            disposed = true;
        }

        public void Read()
        {
            CurrentSign = _fstream.ReadByte();
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
            _fstream.Seek(-(numSigns + 1), SeekOrigin.Current);
            Read();
        }
    }
}