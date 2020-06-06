using Microsoft.VisualStudio.TestTools.UnitTesting;
using SerialInspector.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialInspectorTests
{
    [TestClass]
    public class ObservableSetTests
    {
        [TestMethod]
        public void AddSameIdentifier()
        {
            var set = new ObservableSet<SerialMessage>(new SerialMessageSameIdentifier())
            {
                SerialMessage.Parse("DEADBEEF|01-02-03-04-05-06-07-08"),
                SerialMessage.Parse("DEADBEEF|00-00-00-00-00-00-00-00"),
                SerialMessage.Parse("DEADBEEF|FF-FF-FF-FF-AA-AA-AA-AA")
            };

            Assert.AreEqual(1, set.Count);
            Assert.AreEqual(set[0].Data.Bytes.First(), 0xFF);
            Assert.AreEqual(set[0].Data.Bytes.Last(), 0xAA);
        }

        [TestMethod]
        public void AddDifferentIdentifier()
        {
            var set = new ObservableSet<SerialMessage>(new SerialMessageSameIdentifier())
            {
                SerialMessage.Parse("DEADBEED|01-02-03-04-05-06-07-08"),
                SerialMessage.Parse("DEADBEEE|00-00-00-00-00-00-00-00"),
                SerialMessage.Parse("DEADBEEF|FF-FF-FF-FF-FF-FF-FF-FF")
            };

            Assert.AreEqual(3, set.Count);
        }

        [TestMethod]
        public void AddMultiple()
        {
            var set = new ObservableSet<SerialMessage>(new SerialMessageSameIdentifier())
            {
                SerialMessage.Parse("DEADBEEF|01-02-03-04-05-06-07-08"),
                SerialMessage.Parse("DEADBEEC|01-02-03-04-05-06-07-08"),
                SerialMessage.Parse("DEADBEED|00-00-00-00-00-00-00-00"),
                SerialMessage.Parse("DEADBEEF|FF-FF-FF-FF-FF-FF-FF-FF")
            };

            Assert.AreEqual(3, set.Count);
            Assert.IsFalse(set.Any(x => x.Equals(SerialMessage.Parse("DEADBEEF|01-02-03-04-05-06-07-08"))));
            Assert.IsTrue(set.Any(x => x.Equals(SerialMessage.Parse("DEADBEEF|FF-FF-FF-FF-FF-FF-FF-FF"))));
        }
    }
}
