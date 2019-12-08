using System.IO;
using TKOM.Parser;
using TKOM.Test.Tools.StreamGenerator;
using Xunit;

namespace TKOM.Test.Parser
{
    public class ReaderTest
    {
        [Fact]
        public void PeeksFromStreamIfBufferEmpty()
        {
            var s = "abcdefghijk";
            using (var stream = StreamGenerator.GenerateStreamFromString(s))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var reader = new Reader(stream_reader);

                // act
                var peeked_first = reader.Peek();
                var peeked_second = reader.Peek();

                // validate
                Assert.Equal('a', peeked_first);
                Assert.Equal('a', peeked_second);
            }
        }

        [Fact]
        public void ReadsFromStreamIfBufferEmpty()
        {
            var s = "abcdefghijk";
            using (var stream = StreamGenerator.GenerateStreamFromString(s))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var reader = new Reader(stream_reader);

                // act
                var read = reader.Read();
                var peeked = reader.Peek();
                var read_second = reader.Read();

                // validate
                Assert.Equal('a', read);
                Assert.Equal('b', peeked);
                Assert.Equal('b', read_second);
            }
        }

        [Fact]
        public void ReadsFromBuffeAfterRewind()
        {
            var s = "abcdefghijk";
            using (var stream = StreamGenerator.GenerateStreamFromString(s))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var reader = new Reader(stream_reader);

                // act
                reader.Read();
                reader.Read();
                reader.Read();
                var first_read = reader.Read();
                reader.Rewind(2);
                var second_read = reader.Read();
                var peeked = reader.Peek();
                var third_read = reader.Read();
                var fourth_read = reader.Read();


                // validate
                Assert.Equal('d', first_read);
                Assert.Equal('c', second_read);
                Assert.Equal('d', peeked);
                Assert.Equal('d', third_read);
                Assert.Equal('e', fourth_read);
            }
        }

        [Fact]
        public void PeeksFromEnfOfStrwam()
        {
            var s = "abc";
            using (var stream = StreamGenerator.GenerateStreamFromString(s))
            {
                // prepare
                var stream_reader = new StreamReader(stream);
                var reader = new Reader(stream_reader);

                // act
                reader.Read();
                reader.Read();
                reader.Read();
                var first_peak = reader.Peek();


                // validate
                Assert.Equal(-1, first_peak);
            }
        }
    }
}