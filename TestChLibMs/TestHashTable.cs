using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ChLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestChLibMs
{
    [TestClass]
    public class TestHashTable
    {
        private Position _position;
        private Position _best; 

        [TestInitialize]
        public void Setup()
        {
            _position = new Position();
            _position.WhitePieces = 0x66;
            _position.BlackPieces = 0x45;
            _position.Kings = 0x555;
            _best = new Position();
        }

        [TestMethod]
        public void TestStorePosition()
        {
            Position best = new Position();
            const int testValue = 200;
            HashTable.StorePosition(ref _position, Search.BLACK, testValue, 10, Search.LOSS, Search.WIN, best);
            int value = 0;
            bool result = HashTable.RetrievePosition(ref _position, Search.BLACK, ref value, 10, Search.LOSS, Search.WIN, ref best);
            Assert.IsTrue(result);
            Assert.AreEqual(testValue, value);
        }

        [TestMethod]
        public void TestDifferentColor()
        {
            _best.WhitePieces = 0x367;
            _best.BlackPieces = 0xFFF;
            _best.WhitePieces = 0xF;

            Position q = new Position();

            HashTable.StorePosition(ref _position, Search.BLACK, 200, 10, Search.LOSS, Search.WIN, _best);
            int value = 0;
            bool result = HashTable.RetrievePosition(ref _position, Search.WHITE, ref value, 8, Search.LOSS, Search.WIN, ref q);
            Assert.IsFalse(result);
            Assert.AreEqual(0, value);
            Assert.IsTrue(q.IsEmpty());
        }


        [TestMethod]
        public void TestNotEnoughDepth()
        {
            _best.WhitePieces = 0x367;
            _best.BlackPieces = 0xFFF;
            _best.WhitePieces = 0xF;

            Position q = new Position();

            HashTable.StorePosition(ref _position, Search.BLACK, 200, 10, Search.LOSS, Search.WIN, _best);
            int value = 0;
            bool result = HashTable.RetrievePosition(ref _position, Search.BLACK, ref value, 11, Search.LOSS, Search.WIN, ref q);
            Assert.IsFalse(result);
            Assert.AreEqual(0, value);
            Assert.AreEqual(_best, q);
        }

        [TestMethod]
        public void TestUpperBoundValue()
        {
            _best.WhitePieces = 0x367;
            _best.BlackPieces = 0xFFF;
            _best.WhitePieces = 0xF;

            Position q = new Position();

            const int testValue = -238;
            HashTable.StorePosition(ref _position, Search.BLACK, testValue, 10, testValue, Search.WIN, _best);
            int value = 0;
            bool result = HashTable.RetrievePosition(ref _position, Search.BLACK, ref value, 11, Search.LOSS, Search.WIN, ref q);
            Assert.IsFalse(result);
            Assert.AreEqual(0, value);
            Assert.AreEqual(_best, q);
        }

        [TestMethod]
        public void TestLowerBoundValue()
        {
            _best.WhitePieces = 0x367;
            _best.BlackPieces = 0xFFF;
            _best.WhitePieces = 0xF;

            Position q = new Position();

            const int testValue = 1138;
            HashTable.StorePosition(ref _position, Search.BLACK, testValue, 10, Search.LOSS, testValue, _best);
            int value = 0;
            bool result = HashTable.RetrievePosition(ref _position, Search.BLACK, ref value, 11, Search.LOSS, Search.WIN, ref q);
            Assert.IsFalse(result);
            Assert.AreEqual(0, value);
            Assert.AreEqual(_best, q);
        }

        [TestMethod]
        public void TestRetrieveUpperBoundValue()
        {
            _best.WhitePieces = 0x367;
            _best.BlackPieces = 0xFFF;
            _best.WhitePieces = 0xF;

            Position q = new Position();

            const int testValue = -238;
            HashTable.StorePosition(ref _position, Search.BLACK, testValue, 10, Search.LOSS, Search.WIN, _best);
            int value = 0;
            bool result = HashTable.RetrievePosition(ref _position, Search.BLACK, ref value, 8, -200, Search.WIN, ref q);
            Assert.IsTrue(result);
            Assert.AreEqual(testValue, value);
        }

        [TestMethod]
        public void TestRetrieveLowerBoundValue()
        {

        }

    }
}
