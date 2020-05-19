using Microsoft.VisualStudio.TestTools.UnitTesting;

using SerialInspector.Model;
using System;
using System.Linq;

namespace SerialInspectorTests
{
    [TestClass]
    public class DataChunkTests
    {
        [TestMethod]
        public void ParseBytes()
        {
            string data = "0x00-0x00-0x0A-0x40-0x00-0x00-0x0D-0xCC";

            var dataChunk = new DataChunk(data);

            var expected = new byte[]
            {
                0x00,
                0x00,
                0x0A,
                0x40,
                0x00,
                0x00,
                0x0D,
                0xCC
            };

            Assert.IsTrue(dataChunk.Bytes.SequenceEqual<byte>(expected));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TooShort()
        {
            new DataChunk("0x00-0x00-0x0A-0x40-0x00-0x00-0x0D");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TooLong()
        {
            new DataChunk("0x00-0x00-0x0A-0x40-0x00-0x00-0x0D-0xCC-0xEE");
        }


        [TestMethod]
        public void ParseMath()
        {
            string data = "0x01-0x02-0x03-0x04-0x05-0x06-0x07-0x08";

            var dataChunk = new DataChunk(data, "$A + $B + $C + $D", "$E + $F + $G + $H");

            var expected = new byte[]
            {
                0x01,
                0x02,
                0x03,
                0x04,
                0x05,
                0x06,
                0x07,
                0x08
            };

            Assert.IsTrue(dataChunk.Bytes.SequenceEqual<byte>(expected));
            Assert.AreEqual(dataChunk.FirstChunkSum, 10.0d);
            Assert.AreEqual(dataChunk.SecondChunkSum, 26.0d);
        }

        [TestMethod]
        public void ParseMathPartial()
        {
            string data = "0x01-0x02-0x03-0x04-0x05-0x06-0x07-0x08";

            var dataChunk = new DataChunk(data, "$A", "$E");

            var expected = new byte[]
            {
                0x01,
                0x02,
                0x03,
                0x04,
                0x05,
                0x06,
                0x07,
                0x08
            };

            Assert.IsTrue(dataChunk.Bytes.SequenceEqual<byte>(expected));
            Assert.AreEqual(dataChunk.FirstChunkSum, 1.0d);
            Assert.AreEqual(dataChunk.SecondChunkSum, 5.0d);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void ParseMathFail()
        {
            string data = "0x01-0x02-0x03-0x04-0x05-0x06-0x07-0x08";

            var dataChunk = new DataChunk(data, "", "");

            var expected = new byte[]
            {
                0x01,
                0x02,
                0x03,
                0x04,
                0x05,
                0x06,
                0x07,
                0x08
            };

            Assert.IsTrue(dataChunk.Bytes.SequenceEqual<byte>(expected));
            Assert.AreEqual(dataChunk.FirstChunkSum, 1.0d);
            Assert.AreEqual(dataChunk.SecondChunkSum, 5.0d);
        }
    }
}