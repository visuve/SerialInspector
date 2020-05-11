using System;
using System.Data;
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

        private uint ComputeMath(string math)
        {
            try
            {
                object calculus = new DataTable().Compute(math, null);
                return Convert.ToUInt32(calculus);
            }
            catch // Intentional exception swallowing
            {
                return 0;
            }
        }

        internal DataChunk(string raw, string firstChunkMath, string secondChunkMath) :
            this(raw)
        {
            firstChunkMath = firstChunkMath.Replace("%FIRST_CHUNK%", First.ToString());
            First = ComputeMath(firstChunkMath);

            secondChunkMath = secondChunkMath.Replace("%SECOND_CHUNK%", Second.ToString());
            Second = ComputeMath(secondChunkMath);
        }

        public override string ToString()
        {
            return $"{First}-{Second}";
        }
    }
}