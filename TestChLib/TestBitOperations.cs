using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ChLib;
using NUnit.Framework;

namespace TestChLib
{
    [TestFixture, Ignore]
    public class TestBitOperations
    {
        private readonly uint[] _tests =
            {
                0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x80,
                0x100, 0x200, 0x400, 0x800, 0x1000, 0x2000, 0x4000, 0x8000,
                0x10000, 0x20000, 0x40000, 0x80000, 0x100000, 0x200000, 0x400000, 0x800000,
                0x1000000, 0x2000000, 0x4000000, 0x8000000, 0x10000000, 0x20000000, 0x40000000, 0x80000000,
            };

        [Test]
        public void TestBitScanForward()
        {
            //setupDeBruijn32();
            int expected = 0;
            foreach (var t in _tests)
            {
                var value = bitScanForward32(t); //t.BitScanForward();
                Console.WriteLine("{0:X}({0}): {1}", t, value);
                Assert.AreEqual(expected, value);
                expected++;
            }

            Console.Write("{");
            foreach (var i in _index32)
            {
                Console.Write(i + ", ");
            }
            Console.Write("}");
        }

        private readonly Random _r = new Random();

        private const int _numTimes = (1 << 25);

        [Test]
        public void TestBitScanForwardPerformance()
        {

            for (int i = 0; i < _numTimes - 1; i++)
            {
                var x = _r.Next().BitScanForward();
            }
        }

        [Test]
        public void TestBitScanForward64Performance()
        {

            for (int i = 0; i < _numTimes - 1; i++)
            {
                var x = bitScanForward64((uint)_r.Next());
            }
        }

        [Test]
        public void TestBitScanForward32Performance()
        {

            for (int i = 0; i < _numTimes - 1; i++)
            {
                var x = bitScanForward32((uint)_r.Next());
            }
        }

        [Test]
        public void TestLookupBitCountPerformance()
        {
            for (int i = 0; i < _numTimes - 1; i++)
            {
                var x = _r.Next().BitCount();
            }
        }

        [Test]
        public void TestKernighanBitCountPerformance()
        {
            for (int i = 0; i < _numTimes - 1; i++)
            {
                var x = kernighanBitCount((uint)_r.Next());
            }
        }

        [Test]
        public void TestBitCount64Performance()
        {
            for (int i = 0; i < _numTimes - 1; i++)
            {
                var x = bitCount64((uint)_r.Next());
            }
        }

        // Can be faster than lookup for sparse bit populations
        private static uint kernighanBitCount(uint x)
        {
            uint count = 0;
            while (x != 0)
            {
                count++;
                x &= x - 1; // reset LS1B
            }
            return count;
        }

        private readonly int[] _index64 = {
                                              63, 0, 58, 1, 59, 47, 53, 2,
                                              60, 39, 48, 27, 54, 33, 42, 3,
                                              61, 51, 37, 40, 49, 18, 28, 20,
                                              55, 30, 34, 11, 43, 14, 22, 4,
                                              62, 57, 46, 52, 38, 26, 32, 41,
                                              50, 36, 17, 19, 29, 10, 13, 21,
                                              56, 45, 25, 31, 35, 16, 9, 12,
                                              44, 24, 15, 8, 23, 7, 6, 5
                                          };

        //const uint _debruijn32 = 0x077CB531;
        private const uint _debruijn32 = 0x076be629;
        //private static readonly int[] _index32 = new int[32];
        private static readonly int[] _index32 = {
                                                     0, 1, 23, 2, 29, 24, 19, 3, 30, 27, 25, 11, 20, 8, 4, 13, 31, 22,
                                                     28,
                                                     18, 26, 10, 7, 12, 21, 17, 9, 6, 16, 5, 15, 14,
                                                 };

        private void setupDeBruijn32()
        {
            for (int i = 0; i < 32; i++)
            {
                _index32[(_debruijn32 << i) >> 27] = i;
            }
        }

        // http://supertech.csail.mit.edu/papers/debruijn.pdf p.4
        /// <summary>
        ///  Blijkt een stuk sneller dan op basis van BitCount
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private int bitScanForward32(uint b)
        {

            //b = b & (uint)-b;
            //b *= _debruijn32;
            //b >>= 27;
            //return _index32[b];
            return _index32[((b & (uint)-b) * _debruijn32) >> 27];
        }

        //http://chessprogramming.wikispaces.com/De+Bruijn+sequence
        /**
        * bitScanForward
        * @author Martin Läuter (1997)
        *         Charles E. Leiserson
        *         Harald Prokop
        *         Keith H. Randall
        * "Using de Bruijn Sequences to Index a 1 in a Computer Word"
        * @param bb bitboard to scan
        * @precondition bb != 0
        * @return index (0..63) of least significant one bit
        */
        private int bitScanForward64(ulong bb)
        {
            const long debruijn64 = 0x07EDD5E59A4E28C2;
            Debug.Assert(bb != 0);
            return _index64[((bb & (ulong)-(long)bb) * debruijn64) >> 58];
        }

        [Test]
        public void TestReverse()
        {
            for (int i = 0; i < _numTimes / 10 - 1; i++)
            {
                int value = _r.Next();
                int temp = value.Reverse();
                Assert.AreEqual(value, temp.Reverse());
            }
        }

        [Test]
        public void TestReversePerformance()
        {
            for (int i = 0; i < _numTimes - 1; i++)
            {
                int temp = _r.Next().Reverse();
            }
        }

        [Test]
        public void TestBitCount64()
        {
            Assert.AreEqual(17, bitCount64(0x34FB5E3), "case 1");
            Assert.AreEqual(16, bitCount64(0x33333333), "case 2");
            Assert.AreEqual(1, bitCount64(0x8), "case 3");
            Assert.AreEqual(16, bitCount64(0xFF00FF00), "case 4");
            Assert.AreEqual(21, bitCount64(0x7EDD5E59), "case 5");
            Assert.AreEqual(13, bitCount64(0x9A4E28C2), "case 6");
            Assert.AreEqual(9, bitCount64(0x2FCC), "case 7");
            Assert.AreEqual(9, bitCount64(0xFD840000), "case 8"); 
        }



        // Counting bits set in 32-bit words using 64-bit instructions 
        // With 64 bit magic; fast with fast integer multiplication + division
        //This method requires a 64-bit CPU with fast modulus division to be efficient. It takes 15 operations.
        private uint bitCount64(uint v)
        {
            uint c = (uint)(((v & 0xfff) * 0x1001001001001 & 0x84210842108421) % 0x1F);
            c += (uint)((((v & 0xfff000) >> 12) * 0x1001001001001 & 0x84210842108421) % 0x1F);
            c += (uint)(((v >> 24) * 0x1001001001001 & 0x84210842108421) % 0x1F);
            return c;
        }




    }
}
