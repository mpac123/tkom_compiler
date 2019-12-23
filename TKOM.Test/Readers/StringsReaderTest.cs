using TKOM.Readers;
using Xunit;

namespace TKOM.Test.Readers
{
    public class StringsReaderTest
    {
        [Fact]
        public void StringWithContent_stringReaderReadsFirstSignOnInitialization_CurrentSignCorrect()
        {
            // prepare
            var str = "example 123";

            // act
            var stringReader = new StringsReader(str);

            // validate
            Assert.Equal('e', stringReader.CurrentSign);
        }

        [Fact]
        public void EmptyFile_stringReaderReadsFirstSignOnInitialization_CurrentSignMinusOne()
        {
            // prepare
            var str = "";

            // act
            var stringReader = new StringsReader(str);

            // validate
            Assert.Equal(-1, stringReader.CurrentSign);
        }

        [Fact]
        public void FileWithContent_stringReaderReadsTwoSigns_CurrentSignCorrect()
        {
            // prepare
            var str = "example 123";

            // act
            var stringReader = new StringsReader(str);
            stringReader.Read();
            stringReader.Read();

            // validate
            Assert.Equal('a', stringReader.CurrentSign);
        }

        [Fact]
        public void FileWithContent_stringReaderReadsTwoSignsRewindOne_CurrentSignCorrect()
        {
            // prepare
            var str = "example 123";

            // act
            var stringReader = new StringsReader(str);
            stringReader.Read();
            stringReader.Read();
            stringReader.Rewind(1);

            // validate
            Assert.Equal('x', stringReader.CurrentSign);
        }

        [Fact]
        public void FileWithContent_stringReaderReadsTwoSignsRewindTwo_CurrentSignCorrect()
        {
            // prepare
            var str = "example 123";

            // act
            var stringReader = new StringsReader(str);
            stringReader.Read();
            stringReader.Read();
            stringReader.Rewind(2);

            // validate
            Assert.Equal('e', stringReader.CurrentSign);
        }
    }
}