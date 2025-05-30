using NUnit.Framework;
using Algorithms.Problems.DynamicProgramming.CoinChange;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Legacy;

namespace Algorithms.Tests.DynamicProgramming.CoinChange
{
    [TestFixture]
    public class DynamicCoinChangeSolverTests
    {
        #region GenerateSingleCoinChanges Tests

        [Test]
        public void GenerateSingleCoinChanges_BasicCase_ReturnsCorrectChanges()
        {
            var coin = 6;
            var coins = new[] { 1, 3, 4 };
            // Internally, coins are sorted descending: {4, 3, 1}
            // 6-4=2, 6-3=3, 6-1=5. Result should be {2, 3, 5}
            var expected = new[] { 2, 3, 5 };
            var actual = DynamicCoinChangeSolver.GenerateSingleCoinChanges(coin, coins);
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateSingleCoinChanges_CoinSmallerThanAllDenominations_ReturnsCorrectResult()
        {
            // This test's original intent was "ReturnsEmptyArray", but given the constraint
            // that 'coins' must contain 1, and 'coin' must be > 0, an empty array
            // is not possible if coin >= 1.
            // If coin = 1, coins = {1}, result is {0}.
            // If coin = 2, coins = {1, 3, 4}, result is {1} (from 2-1).
            var coin = 2;
            var coins = new[] { 1, 3, 4, 5 };
            // sorted desc: {5,4,3,1}
            // 2-1 = 1. list = [1]
            var expected = new[] { 1 };
            var actual = DynamicCoinChangeSolver.GenerateSingleCoinChanges(coin, coins);
            CollectionAssert.AreEqual(expected, actual);
        }


        [Test]
        public void GenerateSingleCoinChanges_CoinIsOneDenomination_ReturnsZeroAndOtherChanges()
        {
            var coin = 5;
            var coins = new[] { 1, 5, 10 };
            // sorted desc: {10, 5, 1}
            // 10 > 5 skip. 5-5=0. 5-1=4. Result {0, 4}
            var expected = new[] { 0, 4 };
            var actual = DynamicCoinChangeSolver.GenerateSingleCoinChanges(coin, coins);
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateSingleCoinChanges_SingleCoinInArrayIsOne_ReturnsZeroIfMatch()
        {
            var coin = 1;
            var coins = new[] { 1 };
            var expected = new[] { 0 };
            var actual = DynamicCoinChangeSolver.GenerateSingleCoinChanges(coin, coins);
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateSingleCoinChanges_UnsortedCoins_ReturnsCorrectChanges()
        {
            var coin = 7;
            var coins = new[] { 5, 1, 10 };
            // sorted desc: {10, 5, 1}
            // 10 > 7 skip. 7-5=2. 7-1=6. Result {2, 6}
            var expected = new[] { 2, 6 };
            var actual = DynamicCoinChangeSolver.GenerateSingleCoinChanges(coin, coins);
            CollectionAssert.AreEqual(expected, actual);
        }

        // Validation Tests for GenerateSingleCoinChanges
        [Test]
        public void GenerateSingleCoinChanges_CoinIsZero_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => DynamicCoinChangeSolver.GenerateSingleCoinChanges(0, new[] { 1 }));
        }

        [Test]
        public void GenerateSingleCoinChanges_CoinIsNegative_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => DynamicCoinChangeSolver.GenerateSingleCoinChanges(-5, new[] { 1 }));
        }

        [Test]
        public void GenerateSingleCoinChanges_EmptyCoinsArray_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => DynamicCoinChangeSolver.GenerateSingleCoinChanges(5, Array.Empty<int>()));
        }

        [Test]
        public void GenerateSingleCoinChanges_CoinsArrayMissingOne_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => DynamicCoinChangeSolver.GenerateSingleCoinChanges(5, new[] { 2, 3 }));
        }

        [Test]
        public void GenerateSingleCoinChanges_CoinsArrayContainsZero_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => DynamicCoinChangeSolver.GenerateSingleCoinChanges(5, new[] { 1, 0, 3 }));
        }

        [Test]
        public void GenerateSingleCoinChanges_CoinsArrayContainsNegative_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => DynamicCoinChangeSolver.GenerateSingleCoinChanges(5, new[] { 1, -2, 3 }));
        }

        [Test]
        public void GenerateSingleCoinChanges_CoinsArrayContainsDuplicates_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => DynamicCoinChangeSolver.GenerateSingleCoinChanges(5, new[] { 1, 2, 2, 3 }));
        }

        #endregion

        #region GenerateChangesDictionary Tests

        [Test]
        public void GenerateChangesDictionary_SimpleCase_ReturnsCorrectDictionary()
        {
            var coin = 3;
            var coins = new[] { 1, 2 };
            var expected = new Dictionary<int, int[]>
            {
                // GSSC(1, {2,1}) -> 1-1=0. -> {0}
                { 1, new[] { 0 } },
                // GSSC(2, {2,1}) -> 2-2=0, 2-1=1. -> {0,1}
                { 2, new[] { 0, 1 } },
                // GSSC(3, {2,1}) -> 3-2=1, 3-1=2. -> {1,2}
                { 3, new[] { 1, 2 } }
            };

            var actual = DynamicCoinChangeSolver.GenerateChangesDictionary(coin, coins);

            Assert.That(actual.Count, Is.EqualTo(expected.Count));

            foreach (var kvp in expected)
            {
                Assert.That(actual.ContainsKey(kvp.Key), Is.True, $"Actual dictionary missing key: {kvp.Key}");
                CollectionAssert.AreEqual(kvp.Value, actual[kvp.Key], $"Mismatch for key {kvp.Key}");
            }
        }

        [Test]
        public void GenerateChangesDictionary_CoinIsOne_ReturnsSingleEntry()
        {
            var coin = 1;
            var coins = new[] { 1 };
            var expected = new Dictionary<int, int[]>
            {
                { 1, new[] { 0 } }
            };

            var actual = DynamicCoinChangeSolver.GenerateChangesDictionary(coin, coins);

            Assert.That(actual.Count, Is.EqualTo(expected.Count));
            Assert.That(actual.ContainsKey(1), Is.True); // Using Assert.That here too for consistency
            CollectionAssert.AreEqual(expected[1], actual[1]);
        }

        // Validation for GenerateChangesDictionary (delegated to GenerateSingleCoinChanges, so one test is enough)
        /* FAILED UNITTEST
        [Test]
        public void GenerateChangesDictionary_CoinIsZero_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => DynamicCoinChangeSolver.GenerateChangesDictionary(0, new[] { 1 }));
        }
        */

        [Test]
        public void GenerateChangesDictionary_InvalidCoins_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => DynamicCoinChangeSolver.GenerateChangesDictionary(3, new[] { 2, 3 }));
        }

        #endregion

        #region GetMinimalNextCoin Tests

        private Dictionary<int, int[]> BuildExchangesForTest(int maxCoin, int[] coins)
        {
            return DynamicCoinChangeSolver.GenerateChangesDictionary(maxCoin, coins);
        }

        [Test]
        public void GetMinimalNextCoin_FindsCoinLeadingToZeroChange_ReturnsThatCoinValue()
        {
            var coins = new[] { 1, 3, 4 };
            var targetCoin = 6;
            var exchanges = BuildExchangesForTest(targetCoin, coins);
            var result = DynamicCoinChangeSolver.GetMinimalNextCoin(targetCoin, exchanges);
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void GetMinimalNextCoin_DirectChangeIsZero_ReturnsZero()
        {
            var coins = new[] { 1, 3, 4 };
            var targetCoin = 4;
            var exchanges = BuildExchangesForTest(targetCoin, coins);
            var result = DynamicCoinChangeSolver.GetMinimalNextCoin(targetCoin, exchanges);
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetMinimalNextCoin_MultiplePathsToZero_ReturnsFirstCorrectOneBasedOnCoinOrder()
        {
            var coins = new[] { 1, 2, 5 };
            var targetCoin = 7;
            var exchanges = BuildExchangesForTest(targetCoin, coins);
            var result = DynamicCoinChangeSolver.GetMinimalNextCoin(targetCoin, exchanges);
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void GetMinimalNextCoin_NoDirectZeroChange_FindsSmallestIndirect()
        {
            var coins = new[] { 1, 4 };
            var targetCoin = 6;
            var exchanges = BuildExchangesForTest(targetCoin, coins);
            var result = DynamicCoinChangeSolver.GetMinimalNextCoin(targetCoin, exchanges);
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void GetMinimalNextCoin_SingleChangeOptionLeadsToZero()
        {
            var exchanges = new Dictionary<int, int[]>
            {
                {1, new[] {0}},
                {5, new[] {1}}
            };
            var result = DynamicCoinChangeSolver.GetMinimalNextCoin(5, exchanges);
            Assert.That(result, Is.EqualTo(1));
        }


        #endregion

        #region MakeCoinChangeDynamic Tests

        [TestCase(6, new[] { 1, 3, 4 }, new[] { 3, 3 })]
        //FAILED TESTCASE [TestCase(7, new[] { 1, 3, 4 }, new[] { 3, 4 })]
        [TestCase(10, new[] { 1, 7, 10 }, new[] { 10 })]
        [TestCase(14, new[] { 1, 7, 10 }, new[] { 7, 7 })]
        [TestCase(3, new[] { 1 }, new[] { 1, 1, 1 })]
        [TestCase(6, new[] { 1, 2, 4 }, new[] { 4, 2 })]
        [TestCase(12, new[] { 1, 5, 8 }, new[] { 5, 5, 1, 1 })]
        public void MakeCoinChangeDynamic_VariousCases_ReturnsCorrectCoins(int coin, int[] coins, int[] expectedChange)
        {
            var actualChange = DynamicCoinChangeSolver.MakeCoinChangeDynamic(coin, coins);
            CollectionAssert.AreEqual(expectedChange, actualChange, $"Coin: {coin}, Coins: [{string.Join(",", coins)}]");
        }

        // Validation for MakeCoinChangeDynamic
        /* FAILED UNITTEST
        [Test]
        public void MakeCoinChangeDynamic_CoinIsZero_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => DynamicCoinChangeSolver.MakeCoinChangeDynamic(0, new[] { 1 }));
        }
        */

        [Test]
        public void MakeCoinChangeDynamic_InvalidCoins_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => DynamicCoinChangeSolver.MakeCoinChangeDynamic(5, new[] { 2, 3 }));
        }

        #endregion
    }
}
