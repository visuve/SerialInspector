using System;
using System.Collections.Generic;

namespace SerialInspector.Model
{
    internal class SerialMessage : IEquatable<SerialMessage>
    {
        private string identifier;

        public string Identifier
        { 
            get
            {
                return identifier;
            }
            set
            {
                if (value.Length != 8)
                {
                    throw new ArgumentException("Identifier must be 8 characters long!");
                }

                identifier = value;
            }
        }
        public DataChunk Data { get; private set; }

        internal SerialMessage(string identifier, DataChunk data)
        {
            Identifier = identifier;
            Data = data;
        }

        internal static SerialMessage Parse(string text, string firstChunkFormula, string secondChunkFormula)
        {
            string identifier = text.Substring(0, 8);
            string chunks = text.Substring(9, 23);
            return new SerialMessage(identifier, new DataChunk(chunks, firstChunkFormula, secondChunkFormula));
        }

        internal static SerialMessage Parse(string text)
        {
            string identifier = text.Substring(0, 8);
            string chunks = text.Substring(9, 23);
            return new SerialMessage(identifier, new DataChunk(chunks));
        }

        public bool Equals(SerialMessage other)
        {
            return Identifier.Equals(other.Identifier) && Data.Equals(other.Data);
        }
        public override string ToString()
        {
            return Identifier + '|' + Data.ToString();
        }
    }

    class SerialMessageSameIdentifier : EqualityComparer<SerialMessage>
    {
        public override bool Equals(SerialMessage lhs, SerialMessage rhs)
        {
            if (lhs == null && rhs == null)
            {
                return true;
            }

            if (lhs == null || rhs == null)
            {
                return false;
            }

            return lhs.Identifier == rhs.Identifier;
        }

        public override int GetHashCode(SerialMessage message)
        {
            return message.Identifier.GetHashCode() ^ message.Data.GetHashCode();
        }
    }
}
