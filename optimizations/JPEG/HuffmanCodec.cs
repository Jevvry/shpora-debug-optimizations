using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JPEG.Utilities;

namespace JPEG
{
    public class HuffmanNode
    {
        public byte? LeafLabel { get; set; }
        public int Frequency { get; set; }
        public HuffmanNode Left { get; set; }
        public HuffmanNode Right { get; set; }
    }

    public class BitsWithLength : IEquatable<BitsWithLength>
    {
        public int Bits { get; set; }
        public int BitsCount { get; set; }

        public bool Equals(BitsWithLength other)
        {
            return BitsCount == other.BitsCount && Bits == other.Bits;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is BitsWithLength && Equals((BitsWithLength)obj);
        }
        //public class Comparer : IEqualityComparer<BitsWithLength>
        //{
        //    public bool Equals(BitsWithLength x, BitsWithLength y)
        //    {
        //        if (x == y) return true;
        //        if (x == null || y == null)
        //            return false;
        //        return x.BitsCount == y.BitsCount && x.Bits == y.Bits;
        //    }

        //    public int GetHashCode(BitsWithLength obj)
        //    {
        //        if (obj == null)
        //            return 0;
        //        return ((397 * obj.Bits) << 5) ^ (17 * obj.BitsCount);
        //    }
        //}

        public override int GetHashCode()
        {
            return HashCode.Combine(Bits, BitsCount);
        }
    }

    class BitsBuffer
    {
        private List<byte> buffer = new List<byte>();
        private BitsWithLength unfinishedBits = new BitsWithLength();

        public void Add(BitsWithLength bitsWithLength)
        {
            var bitsCount = bitsWithLength.BitsCount;
            var bits = bitsWithLength.Bits;

            int neededBits = 8 - unfinishedBits.BitsCount;
            while (bitsCount >= neededBits)
            {
                bitsCount -= neededBits;
                buffer.Add((byte)((unfinishedBits.Bits << neededBits) + (bits >> bitsCount)));

                bits = bits & ((1 << bitsCount) - 1);

                unfinishedBits.Bits = 0;
                unfinishedBits.BitsCount = 0;

                neededBits = 8;
            }

            unfinishedBits.BitsCount += bitsCount;
            unfinishedBits.Bits = (unfinishedBits.Bits << bitsCount) + bits;
        }

        public byte[] ToArray(out long bitsCount)
        {
            bitsCount = buffer.Count * 8L + unfinishedBits.BitsCount;
            var result = new byte[bitsCount / 8 + (bitsCount % 8 > 0 ? 1 : 0)];
            buffer.CopyTo(result);
            if (unfinishedBits.BitsCount > 0)
                result[buffer.Count] = (byte)(unfinishedBits.Bits << (8 - unfinishedBits.BitsCount));
            return result;
        }
    }

    public class HuffmanCodec
    {
        public static byte[] Encode(
            byte[] data,
            out Dictionary<BitsWithLength, byte> decodeTable,
            out long bitsCount)
        {
            var frequences = CalcFrequences(data);

            var root = BuildHuffmanTree(frequences);
            var encodeTable = new BitsWithLength[byte.MaxValue + 1];
            FillEncodeTable(root, encodeTable);

            var bitsBuffer = new BitsBuffer();
            foreach (var b in data)
                bitsBuffer.Add(encodeTable[b]);

            decodeTable = CreateDecodeTable(encodeTable);

            return bitsBuffer.ToArray(out bitsCount);
        }

        public static byte[] Decode(byte[] encodedData, Dictionary<BitsWithLength, byte> decodeTable, long bitsCount)
        {
            var result = new List<byte>();

            byte decodedByte;
            var sample = new BitsWithLength { Bits = 0, BitsCount = 0 };
            for (var byteNum = 0; byteNum < encodedData.Length; byteNum++)
            {
                var b = encodedData[byteNum];
                for (var bitNum = 0; bitNum < 8 && byteNum * 8 + bitNum < bitsCount; bitNum++)
                {
                    sample.Bits = (sample.Bits << 1) + ((b & (1 << (8 - bitNum - 1))) != 0 ? 1 : 0);
                    sample.BitsCount++;

                    if (decodeTable.TryGetValue(sample, out decodedByte))
                    {
                        result.Add(decodedByte);

                        sample.BitsCount = 0;
                        sample.Bits = 0;
                    }
                }
            }

            return result.ToArray();
        }

        private static Dictionary<BitsWithLength, byte> CreateDecodeTable(BitsWithLength[] encodeTable)
        {
            var result = new Dictionary<BitsWithLength, byte>();
            for (int b = 0; b < encodeTable.Length; b++)
            {
                var bitsWithLength = encodeTable[b];
                if (bitsWithLength == null)
                    continue;

                result[bitsWithLength] = (byte)b;
            }

            return result;
        }

        private static void FillEncodeTable(HuffmanNode node, BitsWithLength[] encodeSubstitutionTable,
            int bitvector = 0, int depth = 0)
        {
            if (node.LeafLabel != null)
                encodeSubstitutionTable[node.LeafLabel.Value] =
                    new BitsWithLength { Bits = bitvector, BitsCount = depth };
            else
            {
                if (node.Left != null)
                {
                    FillEncodeTable(node.Left, encodeSubstitutionTable, (bitvector << 1) + 1, depth + 1);
                    FillEncodeTable(node.Right, encodeSubstitutionTable, (bitvector << 1) + 0, depth + 1);
                }
            }
        }

        public static HuffmanNode SlowTreeBuild(int[] frequences)
        {
            var nodes = GetNodes(frequences);

            while (nodes.Count() > 1)
            {
                var firstMin = nodes.MinOrDefault(node => node.Frequency);
                nodes = nodes.Without(firstMin);
                var secondMin = nodes.MinOrDefault(node => node.Frequency);
                nodes = nodes.Without(secondMin);
                nodes = nodes.Concat(new HuffmanNode
                {
                    Frequency = firstMin.Frequency + secondMin.Frequency,
                    Left = secondMin,
                    Right = firstMin
                }.ToEnumerable());
            }

            return nodes.First();
        }

        private static HuffmanNode GetMinFromTwoQueues(Queue<HuffmanNode> firstQueue, Queue<HuffmanNode> secondQueue)
        {
            HuffmanNode min;
            if (firstQueue.TryPeek(out var v1))
                if (secondQueue.TryPeek(out var v2))
                    min = v1.Frequency <= v2.Frequency
                        ? firstQueue.Dequeue()
                        : secondQueue.Dequeue();
                else
                    min = firstQueue.Dequeue();
            else
                min = secondQueue.Dequeue();

            return min;
        }

        public static HuffmanNode BuildHuffmanTree(int[] frequences)
        {
            var nodes = new List<HuffmanNode>();
            for (var i = 0; i < frequences.Length; i++)
            {
                if (frequences[i] <= 0)
                    continue;
                nodes.Add(new HuffmanNode { Frequency = frequences[i], LeafLabel = (byte)i });
            }

            nodes.Sort((x, y) => x.Frequency.CompareTo(y.Frequency));

            var firstQueue = new Queue<HuffmanNode>(nodes);
            var secondQueue = new Queue<HuffmanNode>();
            while (firstQueue.Count + secondQueue.Count > 1)
            {
                var firstMin = GetMinFromTwoQueues(firstQueue, secondQueue);
                var secondMin = GetMinFromTwoQueues(firstQueue, secondQueue);

                secondQueue.Enqueue(new HuffmanNode
                {
                    Frequency = firstMin.Frequency + secondMin.Frequency,
                    Left = secondMin,
                    Right = firstMin
                });
            }

            return firstQueue.Count == 0 ? secondQueue.Dequeue() : firstQueue.Dequeue();
        }

        private static IEnumerable<HuffmanNode> GetNodes(int[] frequences)
        {
            return Enumerable.Range(0, byte.MaxValue + 1)
                .Select(num => new HuffmanNode { Frequency = frequences[num], LeafLabel = (byte)num })
                .Where(node => node.Frequency > 0)
                .ToArray();
        }

        private static int[] CalcFrequences(byte[] data)
        {
            var result = new int[byte.MaxValue + 1];
            Parallel.ForEach(data, b => Interlocked.Increment(ref result[b]));
            return result;
        }
    }
}