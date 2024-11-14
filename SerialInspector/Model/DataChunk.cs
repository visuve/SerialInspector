using System;
using System.Data;
using System.Linq;

namespace SerialInspector.Model
{
    internal class DataChunk : IEquatable<DataChunk>
    {
        private readonly DataTable dataTable = new DataTable();

        public double FirstChunkSum
        {
            get;
            private set;
        }

        public double SecondChunkSum
        {
            get;
            private set;
        }

        public byte[] Bytes
        {
            get;
            private set;
        }

        internal DataChunk(string raw)
        {
            Bytes = raw.Split('-').Select(s => Convert.ToByte(s, 16)).ToArray();

            if (Bytes.Length != 8)
            {
                Bytes = null;
                throw new ArgumentException("Amount of chunks inequal to 8");
            }
        }

        private double ComputeMath(string math)
        {
            object calculus = dataTable.Compute(math, null);
            return Convert.ToDouble(calculus);
        }

        public bool Equals(DataChunk other)
        {
            return Bytes.SequenceEqual(other.Bytes);
        }

        public override string ToString()
        {
            return BitConverter.ToString(Bytes, 0, 8);
        }

        internal DataChunk(string raw, string firstChunkFormula, string secondChunkFormula) :
            this(raw)
        {
            var firstChunkMath = firstChunkFormula
                .Replace("$A", Bytes[0].ToString())
                .Replace("$B", Bytes[1].ToString())
                .Replace("$C", Bytes[2].ToString())
                .Replace("$D", Bytes[3].ToString());

            FirstChunkSum = ComputeMath(firstChunkMath);

            var secondChunkMath = secondChunkFormula
                .Replace("$E", Bytes[4].ToString())
                .Replace("$F", Bytes[5].ToString())
                .Replace("$G", Bytes[6].ToString())
                .Replace("$H", Bytes[7].ToString());

            SecondChunkSum = ComputeMath(secondChunkMath);
        }
    }
}