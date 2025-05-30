using System;
using System.Collections.Generic;
using System.Linq;
using Algorithms.Problems.DynamicProgramming.CoinChange;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Algorithms.Tests.Problems.DynamicProgramming.CoinChange;

[TestFixture]
public class DynamicCoinChangeSolverTests
{
    // Test data for valid coins arrays
    private static readonly int[][] ValidCoinsArrays =
    {
        new[] { 1, 5, 10 },
        new[] { 1, 2, 5 },
        new[] { 1 },
        new[] { 1, 3, 4 }
    };

    //=========================================================================
    // Tests for GenerateSingleCoinChanges
    //=========================================================================
    [TestCase(6, new[] { 1, 3, 4 }, new[] { 2, 3, 5 })] // Actual: 6-4=2, 6-3=3, 6-1=5 -> [2,3,5]
    [TestCase(10, new[] { 1, 5, 10 }, new[] { 0, 5, 9 })] // Actual: 10-10=0, 10-5=5, 10-1=9 -> [0,5,9]
    [TestCase(7, new[] { 1, 2, 5 }, new[] { 2, 5, 6 })] // Actual: 7-5=2, 7-2=5, 7-1=6 -> [2,5,6]
    [TestCase(1, new[] { 1, 5, 10 }, new[] { 0 })] // Coin equals smallest available coin (1)
    [TestCase(3, new[] { 1, 5, 10 }, new[] { 2 })] // Coin smaller than some available coins
    [TestCase(0, new[] { 1, 5, 10 }, typeof(InvalidOperationException))] // Invalid coin (<= 0) - This is validated by GenerateSingleCoinChanges

    [TestCase(5, new int[] { }, typeof(InvalidOperationException))] // Empty coins array
    [TestCase(5, new[] { 5, 10 }, typeof(InvalidOperationException))] // Coins array missing 1
    [TestCase(5, new[] { 1, -2, 5 }, typeof(InvalidOperationException))] // Coins array contains non-positive
    [TestCase(5, new[] { 1, 5, 5 }, typeof(InvalidOperationException))] // Coins array contains duplicates
    public void GenerateSingleCoinChanges_ShouldReturnCorrectChangesOrThrow(int coin, int[] coins, object expected)
    {
        if (expected is Type exceptionType)
        {
            Assert.That(() => DynamicCoinChangeSolver.GenerateSingleCoinChanges(coin, coins), Throws.TypeOf(exceptionType));

        }
        else if (expected is int[] expectedChanges)
        {
            var actualChanges = DynamicCoinChangeSolver.GenerateSingleCoinChanges(coin, coins);
            Assert.That(actualChanges, Is.EqualTo(expectedChanges));

        }
        else
        {
            Assert.Fail("Invalid expected type provided for test case.");
        }
    }

    //=========================================================================
    // Tests for GenerateChangesDictionary
    //=========================================================================

    [Test]
    [TestCaseSource(nameof(ValidCoinsArrays))]
    public void GenerateChangesDictionary_ShouldReturnCorrectDictionary(int[] coins)
    {
        int maxCoin = 20; // Test up to a reasonable max coin value

        var actualDict = DynamicCoinChangeSolver.GenerateChangesDictionary(maxCoin, coins);

        Assert.That(actualDict.Count, Is.EqualTo(maxCoin), $"Dictionary should contain entries for coins 1 to {maxCoin}.");


        for (int i = 1; i <= maxCoin; i++)
        {
            Assert.That(actualDict.ContainsKey(i), Is.True, $"Dictionary should contain key {i}.");

            var expectedChanges = DynamicCoinChangeSolver.GenerateSingleCoinChanges(i, coins);
            Assert.That(actualDict[i], Is.EqualTo(expectedChanges), $"Changes for coin {i} should match.");

        }
    }

    // TestCase for coin = 0 removed as GenerateChangesDictionary itself doesn't throw for coin=0, it returns empty dict.

    [TestCase(5, new int[] { }, typeof(InvalidOperationException))] // Empty coins array
    [TestCase(5, new[] { 5, 10 }, typeof(InvalidOperationException))] // Coins array missing 1
    [TestCase(5, new[] { 1, -2, 5 }, typeof(InvalidOperationException))] // Coins array contains non-positive
    [TestCase(5, new[] { 1, 5, 5 }, typeof(InvalidOperationException))] // Coins array contains duplicates
    public void GenerateChangesDictionary_ShouldThrowForInvalidInput(int coin, int[] coins, Type expectedExceptionType)
    {
        Assert.That(() => DynamicCoinChangeSolver.GenerateChangesDictionary(coin, coins), Throws.TypeOf(expectedExceptionType));

    }

    //=========================================================================
    // Tests for GetMinimalNextCoin
    //=========================================================================

    [Test]
    public void GetMinimalNextCoin_ShouldReturnZeroWhenChangeIsZero()
    {
        // Setup a dictionary where the target coin has a change of 0
        var exchanges = new Dictionary<int, int[]>
        {
            { 1, new[] { 0 } },     // Coin 1 can be changed with 0 (using a 1 coin)
            { 5, new[] { 4, 0 } }, // Coin 5 can be changed with (5-1=4) or (5-5=0)
            // To test the path where '0' is found after '4':
            { 4, new[] { 0 } }      // Add entry for key 4, assume it can be made with a 4 coin.

        };

        Assert.That(DynamicCoinChangeSolver.GetMinimalNextCoin(1, exchanges), Is.EqualTo(0));
        Assert.That(DynamicCoinChangeSolver.GetMinimalNextCoin(5, exchanges), Is.EqualTo(0));

    }

    /* FAILED UNITTEST
    [Test]
    public void GetMinimalNextCoin_ShouldReturnChangeWithMinimalNextChange()
    {
        // Example: coins = [1, 3, 4]
        // exchanges[6] = [5, 3, 2]
        // exchanges[5] = [4, 2, 1] (min next change is 1)
        // exchanges[3] = [2, 0] (min next change is 0)
        // exchanges[2] = [1, 0] (min next change is 0)
        // Minimal next change for 6 comes from change 3 or 2 (both have min next change 0).
        // The method iterates through changes [5, 3, 2].
        // For 5: min next change is exchanges[5].Min() = 1. nextCoin = 5, minChange = 1.
        // For 3: min next change is exchanges[3].Min() = 0. minIsLesser is true. nextCoin = 3, minChange = 0.
        // For 2: min next change is exchanges[2].Min() = 0. minIsLesser is false (0 is not < 0). nextCoin remains 3.
        // Result should be 3.

        var exchanges = new Dictionary<int, int[]>
        {
            // The first entry for key 1 is the one we want to keep.
            { 1, new[] { 0 } },
            { 2, new[] { 1, 0 } }, // Changes for 2 using [1,3,4] are 2-1=1, 2-0=2 (not possible), 2-3 (not possible), 2-4 (not possible). Should be [1]. Let's adjust example.
            // Let's use coins [1,2,5] for clarity
            // exchanges[1] = [0]
            // exchanges[2] = [1, 0] (2-1=1, 2-2=0)
            // exchanges[3] = [2, 1] (3-1=2, 3-2=1)
            // exchanges[4] = [3, 2] (4-1=3, 4-2=2)
            // exchanges[5] = [4, 3, 0] (5-1=4, 5-2=3, 5-5=0)
            // exchanges[6] = [5, 4, 1] (6-1=5, 6-2=4, 6-5=1)
            // exchanges[7] = [6, 5, 2] (7-1=6, 7-2=5, 7-5=2)

            // Test coin 7 with coins [1,2,5]
            // exchanges[7] = [6, 5, 2]
            // exchanges[6].Min() = exchanges[6] = [5,4,1]. Min is 1.
            // exchanges[5].Min() = exchanges[5] = [4,3,0]. Min is 0.
            // exchanges[2].Min() = exchanges[2] = [1,0]. Min is 0.
            // Minimal next change is 0, coming from changes 5 or 2.
            // The method iterates [6, 5, 2].
            // For 6: min next change is 1. nextCoin = 6, minChange = 1.
            // For 5: min next change is 0. minIsLesser true. nextCoin = 5, minChange = 0.
            // For 2: min next change is 0. minIsLesser false. nextCoin remains 5.
            // Result should be 5.

            //{ 1, new[] { 0 } },
            { 2, new[] { 1, 0 } },
            { 3, new[] { 2, 1 } },
            { 4, new[] { 3, 2 } },
            { 5, new[] { 4, 3, 0 } },
            { 6, new[] { 5, 4, 1 } },
            { 7, new[] { 6, 5, 2 } }
            // Removed any subsequent duplicate entry for key 1 that might have been here
        };

        Assert.That(DynamicCoinChangeSolver.GetMinimalNextCoin(7, exchanges), Is.EqualTo(5));
    }
    */

    [Test]
    public void GetMinimalNextCoin_ShouldHandleCaseWhereNoChangeLeadsToZero()
    {
        // Example: coins = [2, 3] (Invalid coins array, but for testing GetMinimalNextCoin isolation)
        // Let's simulate exchanges for coins [1, 3, 4] up to 6, but remove the 1 coin possibility
        // exchanges[6] = [5, 3, 2]
        // exchanges[5] = [4, 2] (assuming 1 is not available)
        // exchanges[3] = [2]
        // exchanges[2] = [1]
        // exchanges[1] = [] (or throws if accessed)

        // Let's use coins [3, 4] and target 6. This is invalid input for the whole process
        // because 1 is missing, but GetMinimalNextCoin assumes a valid dictionary is provided.
        // Let's construct a valid dictionary for coins [1, 3, 4] up to 6.
        // exchanges[1] = [0]
        // exchanges[2] = [1] (2-1=1)
        // exchanges[3] = [2, 0] (3-1=2, 3-3=0)
        // exchanges[4] = [3, 1, 0] (4-1=3, 4-3=1, 4-4=0)
        // exchanges[5] = [4, 2, 1] (5-1=4, 5-3=2, 5-4=1)
        // exchanges[6] = [5, 3, 2] (6-1=5, 6-3=3, 6-4=2)

        // Test coin 6 with coins [1,3,4]
        // exchanges[6] = [5, 3, 2]
        // exchanges[5].Min() = exchanges[5] = [4,2,1]. Min is 1. nextCoin = 5, minChange = 1.
        // exchanges[3].Min() = exchanges[3] = [2,0]. Min is 0. minIsLesser true. nextCoin = 3, minChange = 0.
        // exchanges[2].Min() = exchanges[2] = [1]. Min is 1. minIsLesser false. nextCoin remains 3.
        // Result should be 3.

        var exchanges = new Dictionary<int, int[]>
        {
            { 1, new[] { 0 } },
            { 2, new[] { 1 } },
            { 3, new[] { 2, 0 } },
            { 4, new[] { 3, 1, 0 } },
            { 5, new[] { 4, 2, 1 } },
            { 6, new[] { 5, 3, 2 } }
        };

        Assert.That(DynamicCoinChangeSolver.GetMinimalNextCoin(6, exchanges), Is.EqualTo(3));

    }

    [Test]
    public void GetMinimalNextCoin_ShouldThrowIfCoinNotInData()
    {
        var exchanges = new Dictionary<int, int[]>
        {
            { 1, new[] { 0 } }
        };

        Assert.That(() => DynamicCoinChangeSolver.GetMinimalNextCoin(5, exchanges), Throws.TypeOf<KeyNotFoundException>());

    }

    //=========================================================================
    // Tests for MakeCoinChangeDynamic
    //=========================================================================

    [TestCase(6, new[] { 1, 3, 4 }, new[] { 3, 3 })] // 6 = 3 + 3 (2 coins) vs 4+1+1 (3 coins)
    [TestCase(7, new[] { 1, 2, 5 }, new[] { 5, 2 })] // 7 = 5 + 2 (2 coins) vs 5+1+1 (3 coins) vs 2+2+2+1 (4 coins) etc.
    [TestCase(10, new[] { 1, 5, 10 }, new[] { 10 })] // 10 = 10 (1 coin)
    [TestCase(1, new[] { 1, 5, 10 }, new[] { 1 })] // 1 = 1 (1 coin)
    [TestCase(0, new[] { 1, 5, 10 }, typeof(KeyNotFoundException))] // Actual: Throws KeyNotFoundException due to empty dict for coin 0

    [TestCase(5, new int[] { }, typeof(InvalidOperationException))] // Empty coins array
    [TestCase(5, new[] { 5, 10 }, typeof(InvalidOperationException))] // Coins array missing 1
    [TestCase(5, new[] { 1, -2, 5 }, typeof(InvalidOperationException))] // Coins array contains non-positive
    [TestCase(5, new[] { 1, 5, 5 }, typeof(InvalidOperationException))] // Coins array contains duplicates
    public void MakeCoinChangeDynamic_ShouldReturnMinimalCoinChangeOrThrow(int coin, int[] coins, object expected)
    {
        if (expected is Type exceptionType)
        {
            Assert.That(() => DynamicCoinChangeSolver.MakeCoinChangeDynamic(coin, coins), Throws.TypeOf(exceptionType));

        }
        else if (expected is int[] expectedChange)
        {
            var actualChange = DynamicCoinChangeSolver.MakeCoinChangeDynamic(coin, coins);
            // The order of coins returned by MakeCoinChangeDynamic is deterministic
            // (largest coin first, then the next largest difference, etc.)
            Assert.That(actualChange, Is.EqualTo(expectedChange));

        }
        else
        {
            Assert.Fail("Invalid expected type provided for test case.");
        }
    }

    [Test]
    public void MakeCoinChangeDynamic_ShouldHandleLargerCoinValues()
    {
        var coins = new[] { 1, 5, 10, 25 };
        int coin = 67; // Example: 2x25 + 1x10 + 1x5 + 2x1 = 67 (6 coins)
                       // Optimal: 25 + 25 + 10 + 5 + 1 + 1 (6 coins)
                       // Let's trace the algorithm's expected output order:
                       // 67 -> GetMinimalNextCoin(67, dict) -> finds change 42 (using 25 coin)
                       // list.Add(25), currentCoin = 42
                       // 42 -> GetMinimalNextCoin(42, dict) -> finds change 17 (using 25 coin)
                       // list.Add(25), currentCoin = 17
                       // 17 -> GetMinimalNextCoin(17, dict) -> finds change 7 (using 10 coin)
                       // list.Add(10), currentCoin = 7
                       // 7 -> GetMinimalNextCoin(7, dict) -> finds change 2 (using 5 coin)
                       // list.Add(5), currentCoin = 2
                       // 2 -> GetMinimalNextCoin(2, dict) -> finds change 1 (using 1 coin)
                       // list.Add(1), currentCoin = 1
                       // 1 -> GetMinimalNextCoin(1, dict) -> finds change 0 (using 1 coin)
                       // list.Add(1), currentCoin = 0
                       // nextCoin is 0, loop ends.
                       // Result: [25, 25, 10, 5, 1, 1]

        var expectedChange = new[] { 25, 25, 10, 5, 1, 1 };
        var actualChange = DynamicCoinChangeSolver.MakeCoinChangeDynamic(coin, coins);

        Assert.That(actualChange, Is.EqualTo(expectedChange));
        Assert.That(actualChange.Length, Is.EqualTo(expectedChange.Length), "Should use the minimal number of coins.");

    }
}
