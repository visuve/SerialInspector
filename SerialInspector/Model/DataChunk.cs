using System.Globalization;

namespace SerialInspector.Model
{
    internal class DataChunk
    {
        public uint First
        {
            get;
            private set;
        }

        public uint Second
        {
            get;
            private set;
        }

        internal DataChunk(string raw)
        {
            string[] chunks = raw.Split('-');
            First = uint.Parse(chunks[0], NumberStyles.HexNumber);
            Second = uint.Parse(chunks[1], NumberStyles.HexNumber);
        }

        public override string ToString()
        {
            return $"{First}-{Second}";
        }
    }
}