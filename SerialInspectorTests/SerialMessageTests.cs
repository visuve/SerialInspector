using Microsoft.VisualStudio.TestTools.UnitTesting;
using SerialInspector.Model;
using System;
using System.Linq;

namespace SerialInspectorTests
{
    [TestClass]
    public class SerialMessageTests
    {
        [TestMethod]
        public void ParseSuccess()
        {
            SerialMessage message = SerialMessage.Parse("DEADBEEF|01-02-03-04-05-06-07-08");
            Assert.AreEqual("DEADBEEF", message.Identifier);
            Assert.AreEqual(1, message.Data.Bytes.First());
            Assert.AreEqual(8, message.Data.Bytes.Last());
        }

        [TestMethod]
        public void ParseTooLong()
        {
            SerialMessage message = SerialMessage.Parse("DEADBEEF|01-02-03-04-05-06-07-08-09");
            Assert.AreEqual("DEADBEEF", message.Identifier);
            Assert.AreEqual(1, message.Data.Bytes.First());
            Assert.AreEqual(8, message.Data.Bytes.Last());
        }

        [TestMethod]
        public void AreEqual()
        {
            var a = SerialMessage.Parse("DEADBEEF|01-02-03-04-05-06-07-08");
            var b = SerialMessage.Parse("DEADBEEF|01-02-03-04-05-06-07-08");
            Assert.AreEqual(a, b);
        }
    }
}
