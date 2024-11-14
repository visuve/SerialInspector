using Microsoft.VisualStudio.TestTools.UnitTesting;
using SerialInspector.View;
using System.Globalization;

namespace SerialInspectorTests
{
    [TestClass]
    public class ConverterTests
    {
        HexConverter converter = new HexConverter();

        [TestMethod]
        public void ConvertHexString()
        {
            object result = converter.Convert("DEADBEEF", typeof(string), null, CultureInfo.InvariantCulture);
            Assert.AreEqual(result.ToString(), "0xDEADBEEF");

            result = converter.Convert("DEAD", typeof(string), null, CultureInfo.InvariantCulture);
            Assert.AreEqual(result.ToString(), "0xDEAD0000");

            result = converter.Convert("DEADBE", typeof(string), null, CultureInfo.InvariantCulture);
            Assert.AreEqual(result.ToString(), "0xDEADBE00");
        }

        [TestMethod]
        public void ConvertBackHexString()
        {
            object result = converter.ConvertBack("0xDEADBEEF", typeof(string), null, CultureInfo.InvariantCulture);
            Assert.AreEqual(result, "DEADBEEF");

            result = converter.ConvertBack("0xDEAD", typeof(string), null, CultureInfo.InvariantCulture);
            Assert.AreEqual(result, "DEAD0000");

            result = converter.ConvertBack("0x00DEAD", typeof(string), null, CultureInfo.InvariantCulture);
            Assert.AreEqual(result, "00DEAD00");

            result = converter.ConvertBack("0x0000DEAD", typeof(string), null, CultureInfo.InvariantCulture);
            Assert.AreEqual(result, "0000DEAD");
        }


        [TestMethod]
        public void ConvertHexByte()
        {
            byte value = 0xFF;
            var result = converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);
            Assert.AreEqual(result, "0xFF");

            value = 0x01;
            result = converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);
            Assert.AreEqual(result, "0x01");
        }

        [TestMethod]
        public void ConvertBackHexByte()
        {
            var result = converter.ConvertBack("0xFF", typeof(byte), null, CultureInfo.InvariantCulture);
            byte expected = 0xFF;
            Assert.AreEqual(result, expected);

            result = converter.ConvertBack("0x01", typeof(byte), null, CultureInfo.InvariantCulture);
            expected = 0x01;
            Assert.AreEqual(result, expected);
        }
    }
}