using System.IO;
using TKOM.Readers;
using Xunit;

namespace TKOM.Test.Readers
{
    public class FileReaderTest
    {
        [Fact]
        public void FileWithContent_FileReaderReadsFirstSignOnInitialization_CurrentSignCorrect()
        {
            // prepare
            using (var file = new StreamWriter("file.txt"))
            {
                file.WriteLine("example 123");
            }

            // act
            var fileReader = new FileReader("file.txt");

            // validate
            Assert.Equal('e', fileReader.CurrentSign);
        }

        [Fact]
        public void EmptyFile_FileReaderReadsFirstSignOnInitialization_CurrentSignMinusOne()
        {
            // prepare
            using (var file = new StreamWriter("empty_file.txt"))
            {
            }

            // act
            var fileReader = new FileReader("empty_file.txt");

            // validate
            Assert.Equal(-1, fileReader.CurrentSign);
        }

        [Fact]
        public void FileWithContent_FileReaderReadsTwoSigns_CurrentSignCorrect()
        {
            // prepare
            using (var file = new StreamWriter("file.txt"))
            {
                file.WriteLine("example 123");
            }

            // act
            var fileReader = new FileReader("file.txt");
            fileReader.Read();
            fileReader.Read();

            // validate
            Assert.Equal('a', fileReader.CurrentSign);
        }

        [Fact]
        public void FileWithContent_FileReaderReadsTwoSignsRewindOne_CurrentSignCorrect()
        {
            // prepare
            using (var file = new StreamWriter("file.txt"))
            {
                file.WriteLine("example 123");
            }

            // act
            var fileReader = new FileReader("file.txt");
            fileReader.Read();
            fileReader.Read();
            fileReader.Rewind(1);

            // validate
            Assert.Equal('x', fileReader.CurrentSign);
        }

        [Fact]
        public void FileWithContent_FileReaderReadsTwoSignsRewindTwo_CurrentSignCorrect()
        {
            // prepare
            using (var file = new StreamWriter("file.txt"))
            {
                file.WriteLine("example 123");
            }

            // act
            var fileReader = new FileReader("file.txt");
            fileReader.Read();
            fileReader.Read();
            fileReader.Rewind(2);

            // validate
            Assert.Equal('e', fileReader.CurrentSign);
        }
    }
}