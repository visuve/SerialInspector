using Microsoft.VisualStudio.TestTools.UnitTesting;
using SerialInspector;
using SerialInspector.Model;
using System.IO.Ports;
using System.Text;

namespace SerialInspectorTests
{
    [TestClass]
    public class SettingsUtilityTests
    {
        [TestMethod]
        public void SettingsPathNotNull()
        {
            Assert.IsFalse(string.IsNullOrEmpty(SettingsUtility.SettingsFilePath));
        }

        [TestMethod]
        public void IsValidSettings()
        {
            var options = new SerialConnectionOptions(new string[] { "COM1" });

            {
                var settings = new SerialConnectionSettings()
                {
                    Port = "COM1",
                    BaudRate = 38400,
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One
                };

                Assert.IsTrue(SettingsUtility.IsValidSettings(options, settings));
            }
            {
                var settings = new SerialConnectionSettings()
                {
                    Port = "COM1",
                    BaudRate = 38400,
                    Parity = Parity.None,
                    DataBits = -1,
                    StopBits = StopBits.One
                };

                Assert.IsFalse(SettingsUtility.IsValidSettings(options, settings));
            }
        }

        [TestMethod]
        public void DeserializeSuccess()
        {
            string json = "{ \"port\": \"COM1\", \"baud_rate\": 38400, \"parity\": 0, \"data_bits\": 8, \"stop_bits\": 1 }";
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            SerialConnectionSettings settings = SettingsUtility.FromBytes(bytes);

            Assert.AreEqual(settings.Port, "COM1");
            Assert.AreEqual(settings.BaudRate, 38400);
            Assert.AreEqual(settings.Parity, Parity.None);
            Assert.AreEqual(settings.DataBits, 8);
            Assert.AreEqual(settings.StopBits, StopBits.One);
        }

        [TestMethod]
        public void SerializeSuccess()
        {
            var settings = new SerialConnectionSettings()
            {
                Port = "COM1",
                BaudRate = 38400,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One
            };

            byte[] bytes = SettingsUtility.ToBytes(settings, false);
            string json = Encoding.UTF8.GetString(bytes);
            Assert.IsTrue(json == "{\"port\":\"COM1\",\"baud_rate\":38400,\"parity\":0,\"data_bits\":8,\"stop_bits\":1}");
        }
    }
}