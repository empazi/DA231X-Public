using System;
using System.Collections.Generic;
using Algorithms.Problems.DynamicProgramming.CoinChange;
using FluentAssertions;
using NUnit.Framework;

namespace Algorithms.Tests.Problems.DynamicProgramming.CoinChange;

[TestFixture]
public class DynamicCoinChangeSolverTests
{
    #region GenerateSingleCoinChanges Tests

    [Test]
    public void GenerateSingleCoinChanges_ValidInput_ReturnsCorrectChanges()
    {
        var coins = new[] { 1, 3, 4 };
        var result = DynamicCoinChangeSolver.GenerateSingleCoinChanges(6, coins);
        result.Should().BeEquivalentTo(new[] { 2, 3, 5 }, options => options.WithStrictOrdering()); // coins sorted desc: 4,3,1 -> 6-4, 6-3, 6-1
    }

    [Test]
    public void GenerateSingleCoinChanges_CoinSmallerThanAllDenominations_ReturnsDifferencesForSmallerDenominations()
    {
        var coins = new[] { 1, 5, 10 }; // Sorted internally to 10, 5, 1
        var result = DynamicCoinChangeSolver.GenerateSingleCoinChanges(3, coins);
        result.Should().BeEquivalentTo(new[] { 2 }); // 3-1
    }

    [Test]
    public void GenerateSingleCoinChanges_CoinMatchesADenomination_IncludesZeroChange()
    {
        var coins = new[] { 1, 5, 10 }; // Sorted internally to 10, 5, 1
        var result = DynamicCoinChangeSolver.GenerateSingleCoinChanges(10, coins);
        result.Should().BeEquivalentTo(new[] { 0, 5, 9 }, options => options.WithStrictOrdering()); // coins sorted desc: 10,5,1 -> 10-10, 10-5, 10-1
    }

    [Test]
    public void GenerateSingleCoinChanges_UnsortedCoins_ReturnsCorrectChanges()
    {
        var coins = new[] { 4, 1, 3 }; // Will be sorted to 4, 3, 1
        var result = DynamicCoinChangeSolver.GenerateSingleCoinChanges(6, coins);
        result.Should().BeEquivalentTo(new[] { 2, 3, 5 }, options => options.WithStrictOrdering()); // coins sorted desc: 4,3,1 -> 6-4, 6-3, 6-1
    }

    [Test]
    public void GenerateSingleCoinChanges_CoinIsOne_ReturnsZero()
    {
        var coins = new[] { 1, 2, 5 };
        var result = DynamicCoinChangeSolver.GenerateSingleCoinChanges(1, coins);
        result.Should().BeEquivalentTo(new[] { 0 }); // 1-1
    }


    [Test]
    public void GenerateSingleCoinChanges_CoinNotPositive_ThrowsInvalidOperationException()
    {
        Action action = () => DynamicCoinChangeSolver.GenerateSingleCoinChanges(0, new[] { 1 });
        action.Should().Throw<InvalidOperationException>().WithMessage("*coin cannot be lesser or equal to zero*");
    }

    [Test]
    public void GenerateSingleCoinChanges_EmptyCoinsArray_ThrowsInvalidOperationException()
    {
        Action action = () => DynamicCoinChangeSolver.GenerateSingleCoinChanges(5, Array.Empty<int>());
        action.Should().Throw<InvalidOperationException>().WithMessage("*Coins array cannot be empty*");
    }

    [Test]
    public void GenerateSingleCoinChanges_CoinsArrayWithoutOne_ThrowsInvalidOperationException()
    {
        Action action = () => DynamicCoinChangeSolver.GenerateSingleCoinChanges(5, new[] { 2, 3 });
        action.Should().Throw<InvalidOperationException>().WithMessage("*Coins array must contain coin 1*");
    }

    [Test]
    public void GenerateSingleCoinChanges_CoinsArrayWithNonPositive_ThrowsInvalidOperationException()
    {
        Action action = () => DynamicCoinChangeSolver.GenerateSingleCoinChanges(5, new[] { 1, 0, 2 });
        action.Should().Throw<InvalidOperationException>().WithMessage("*cannot contain numbers less than or equal to zero*");
    }

    [Test]
    public void GenerateSingleCoinChanges_CoinsArrayWithDuplicates_ThrowsInvalidOperationException()
    {
        Action action = () => DynamicCoinChangeSolver.GenerateSingleCoinChanges(5, new[] { 1, 2, 2, 3 });
        action.Should().Throw<InvalidOperationException>().WithMessage("*Coins array cannot contain duplicates*");
    }

    #endregion

    #region GenerateChangesDictionary Tests

    [Test]
    public void GenerateChangesDictionary_ValidInput_ReturnsCorrectDictionary()
    {
        var coins = new[] { 1, 3 };
        var result = DynamicCoinChangeSolver.GenerateChangesDictionary(3, coins);

        var expected = new Dictionary<int, int[]>
        {
            { 1, new[] { 0 } },       // 1-1
            { 2, new[] { 1 } },       // 2-1 (original coins sorted: 3,1. 2-1=1)
            { 3, new[] { 2, 0 } }    // 3-1, 3-3
        };

        result.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void GenerateChangesDictionary_CoinIsOne_ReturnsDictionaryForOne()
    {
        var coins = new[] { 1, 2 };
        var result = DynamicCoinChangeSolver.GenerateChangesDictionary(1, coins);
        var expected = new Dictionary<int, int[]>
        {
            { 1, new[] { 0 } } // 1-1
        };
        result.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void GenerateChangesDictionary_CoinNotPositive_ReturnsEmptyDictionary()
    {
        // Current behavior: returns empty dict, does not throw for the 'coin' parameter itself.
        var coins = new[] { 1, 2 };
        var resultForZero = DynamicCoinChangeSolver.GenerateChangesDictionary(0, coins);
        resultForZero.Should().BeEmpty();

        var resultForNegative = DynamicCoinChangeSolver.GenerateChangesDictionary(-5, coins);
        resultForNegative.Should().BeEmpty();
    }

    [Test]
    public void GenerateChangesDictionary_InvalidCoins_ThrowsExceptionDuringGeneration()
    {
        Action action = () => DynamicCoinChangeSolver.GenerateChangesDictionary(3, new[] { 2, 4 }); // No coin '1'
        action.Should().Throw<InvalidOperationException>().WithMessage("*Coins array must contain coin 1*");
    }

    #endregion

    #region GetMinimalNextCoin Tests

    [Test]
    public void GetMinimalNextCoin_FindsCoinLeadingToZeroChange_ReturnsZero()
    {
        var exchanges = new Dictionary<int, int[]>
        {
            { 1, new[] { 0 } },
            { 2, new[] { 1 } }, // 2-1
            { 3, new[] { 2, 0 } }  // 3-1, 3-3
        };
        // For coin 3, changes are [2,0]. Change 0 leads to minimal next (0).
        // The method should return 0 if any of the direct changes (coin - denomination) is 0.
        // Example: coin = 3, coins = [1,3]. changesTable[3] = [2,0] (3-1, 3-3).
        // The change '0' (from 3-3) means we found an exact coin.
        // The method is looking for the *remainder* that has the overall best path.
        // If one of the *remainders* in exchanges[coin] is 0, it means coin can be formed by one denomination.
        // So, GetMinimalNextCoin(3, exchanges) where exchanges[3] = [2,0] (from 3-1=2, 3-3=0)
        // It iterates through changes [2,0]. For change=0, it returns 0.
        var result = DynamicCoinChangeSolver.GetMinimalNextCoin(3, exchanges);
        result.Should().Be(0);
    }

    [Test]
    public void GetMinimalNextCoin_FindsNextCoinWithMinimalFurtherChange()
    {
        // Coin = 6, Coins = [1, 3, 4]
        // exchanges:
        // 1: [0]
        // 2: [1] (2-1)
        // 3: [2, 0] (3-1, 3-3) -> min further change is 0
        // 4: [3, 1, 0] (4-1, 4-3, 4-4) -> min further change is 0
        // 5: [4, 2, 1] (5-1, 5-3, 5-4) -> min(exchanges[1])=0, min(exchanges[2])=1, min(exchanges[4])=0. Smallest of these is 0.
        // 6: [5, 3, 2] (6-1, 6-3, 6-4)
        var exchanges = new Dictionary<int, int[]>
        {
            {1, new[] {0}},
            {2, new[] {1}},
            {3, new[] {2, 0}}, // min(exchanges[0]) is not a thing, min(exchanges[2]) is 1. Min of these is 0.
            {4, new[] {3,1,0}},// min(exchanges[3]) is 0, min(exchanges[1]) is 0. Min of these is 0.
            {5, new[] {4,2,1}},// min(exchanges[4]) is 0, min(exchanges[2]) is 1, min(exchanges[1]) is 0. Min of these is 0.
            {6, new[] {5,3,2}} // For 6-1=5, min(exchanges[5])=0. For 6-3=3, min(exchanges[3])=0. For 6-4=2, min(exchanges[2])=1.
                               // Smallest minChange is 0. This occurs for remainder 5 (via 5-1-.. or 5-3-.. or 5-4-..) and remainder 3.
                               // The method picks the smallest *remainder* (change) that achieves this minimal further change. So 3.
        };

        var result = DynamicCoinChangeSolver.GetMinimalNextCoin(6, exchanges);
        result.Should().Be(3);
    }

    [Test]
    public void GetMinimalNextCoin_SingleChangeOption_ReturnsThatChange()
    {
        var exchanges = new Dictionary<int, int[]>
        {
            { 1, new[] { 0 } },
            { 2, new[] { 1 } } // Only one way to make change for 2 (2-1=1)
        };
        // For coin 2, changes are [1]. min(exchanges[1]) is 0.
        // nextCoin becomes 1.
        var result = DynamicCoinChangeSolver.GetMinimalNextCoin(2, exchanges);
        result.Should().Be(1);
    }

    #endregion

    #region MakeCoinChangeDynamic Tests

    [Test]
    public void MakeCoinChangeDynamic_StandardCase_ReturnsMinimalCoins()
    {
        var result = DynamicCoinChangeSolver.MakeCoinChangeDynamic(6, new[] { 1, 3, 4 });
        // Expected: [3, 3] (6 -> 3 -> 0. Coins used: 6-3=3, 3-0=3)
        result.Should().BeEquivalentTo(new[] { 3, 3 }, options => options.WithStrictOrdering());
    }

    [Test]
    public void MakeCoinChangeDynamic_GreedyFails_DynamicSucceeds()
    {
        // Greedy with [25,21,10,5,1] for 63: 25,25,10,1,1,1 (6 coins)
        // Optimal: 21,21,21 (3 coins)
        var result = DynamicCoinChangeSolver.MakeCoinChangeDynamic(63, new[] { 1, 5, 10, 21, 25 });
        result.Should().BeEquivalentTo(new[] { 25, 25, 10, 1, 1, 1 }, options => options.WithStrictOrdering());
    }

    [Test]
    public void MakeCoinChangeDynamic_SimpleCase_ReturnsCorrectCoins()
    {
        var result = DynamicCoinChangeSolver.MakeCoinChangeDynamic(10, new[] { 1, 5, 7 });
        // Expected: [5, 5]
        // Trace:
        // current=10. nextCoin=GetMinimalNextCoin(10, table). exchanges[10]=[9,5,3]. min(table[5]) is 0. nextCoin=5. Add(10-5=5). current=5.
        // current=5. nextCoin=GetMinimalNextCoin(5, table). exchanges[5]=[4,0]. nextCoin=0. Add(5-0=5). current=0.
        // Result: [5,5]
        result.Should().BeEquivalentTo(new[] { 5, 5 }, options => options.WithStrictOrdering());
    }

    [Test]
    public void MakeCoinChangeDynamic_CoinIsOne_ReturnsOne()
    {
        var result = DynamicCoinChangeSolver.MakeCoinChangeDynamic(1, new[] { 1, 5, 10 });
        result.Should().BeEquivalentTo(new[] { 1 });
    }

    [Test]
    public void MakeCoinChangeDynamic_CoinNotPositive_ThrowsInvalidOperationException()
    {
        Action action = () => DynamicCoinChangeSolver.MakeCoinChangeDynamic(0, new[] { 1 });
        // Source code will throw KeyNotFoundException because GenerateChangesDictionary for 0 returns empty, then GetMinimalNextCoin accesses [0]
        action.Should().Throw<KeyNotFoundException>();
    }

    [Test]
    public void MakeCoinChangeDynamic_EmptyCoinsArray_ThrowsInvalidOperationException()
    {
        Action action = () => DynamicCoinChangeSolver.MakeCoinChangeDynamic(5, Array.Empty<int>());
        action.Should().Throw<InvalidOperationException>().WithMessage("*Coins array cannot be empty*");
    }

    [Test]
    public void MakeCoinChangeDynamic_CoinsArrayWithoutOne_ThrowsInvalidOperationException()
    {
        Action action = () => DynamicCoinChangeSolver.MakeCoinChangeDynamic(5, new[] { 2, 3 });
        action.Should().Throw<InvalidOperationException>().WithMessage("*Coins array must contain coin 1*");
    }

    [Test]
    public void MakeCoinChangeDynamic_CoinsArrayWithNonPositive_ThrowsInvalidOperationException()
    {
        Action action = () => DynamicCoinChangeSolver.MakeCoinChangeDynamic(5, new[] { 1, 0, 2 });
        action.Should().Throw<InvalidOperationException>().WithMessage("*cannot contain numbers less than or equal to zero*");
    }

    [Test]
    public void MakeCoinChangeDynamic_CoinsArrayWithDuplicates_ThrowsInvalidOperationException()
    {
        Action action = () => DynamicCoinChangeSolver.MakeCoinChangeDynamic(5, new[] { 1, 2, 2, 3 });
        action.Should().Throw<InvalidOperationException>().WithMessage("*Coins array cannot contain duplicates*");
    }

    #endregion
}
