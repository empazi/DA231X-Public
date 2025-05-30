using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures.RedBlackTree;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace DataStructures.Tests.RedBlackTree;

[TestFixture]
public class RedBlackTreeTests
{
    [Test]
    public void Add_SingleElement_CountIsOne()
    {
        var tree = new RedBlackTree<int>();
        tree.Add(10);
        Assert.That(tree.Count, Is.EqualTo(1));
        Assert.That(tree.Contains(10), Is.True);
    }

    [Test]
    public void Add_MultipleElements_CountIsCorrect()
    {
        var tree = new RedBlackTree<int>();
        tree.Add(10);
        tree.Add(5);
        tree.Add(15);
        Assert.That(tree.Count, Is.EqualTo(3));
        Assert.That(tree.Contains(10), Is.True);
        Assert.That(tree.Contains(5), Is.True);
        Assert.That(tree.Contains(15), Is.True);
    }

    [Test]
    public void Add_DuplicateElement_ThrowsArgumentException()
    {
        var tree = new RedBlackTree<int>();
        tree.Add(10);
        Assert.Throws<ArgumentException>(() => tree.Add(10));
    }

    [Test]
    public void AddRange_AddsAllElements()
    {
        var tree = new RedBlackTree<int>();
        var elements = new List<int> { 10, 5, 15, 3, 7, 12, 17 };
        tree.AddRange(elements);
        Assert.That(tree.Count, Is.EqualTo(elements.Count));
        foreach (var el in elements)
        {
            Assert.That(tree.Contains(el), Is.True);
        }
    }

    [Test]
    public void Remove_ExistingElement_RemovesSuccessfully()
    {
        var tree = new RedBlackTree<int>();
        tree.Add(10);
        tree.Add(5);
        tree.Add(15);

        tree.Remove(5);
        Assert.That(tree.Count, Is.EqualTo(2));
        Assert.That(tree.Contains(5), Is.False);
        Assert.That(tree.Contains(10), Is.True);
        Assert.That(tree.Contains(15), Is.True);
    }

    [Test]
    public void Remove_RootElement_RemovesSuccessfully()
    {
        var tree = new RedBlackTree<int>();
        tree.Add(10);
        tree.Add(5);
        tree.Add(15);

        tree.Remove(10);
        Assert.That(tree.Count, Is.EqualTo(2));
        Assert.That(tree.Contains(10), Is.False);
        Assert.That(tree.Contains(5), Is.True);
        Assert.That(tree.Contains(15), Is.True);
    }

    [Test]
    public void Remove_NonExistingElement_ThrowsKeyNotFoundException()
    {
        var tree = new RedBlackTree<int>();
        tree.Add(10);
        Assert.Throws<KeyNotFoundException>(() => tree.Remove(5));
    }

    [Test]
    public void Remove_FromEmptyTree_ThrowsInvalidOperationException()
    {
        var tree = new RedBlackTree<int>();
        Assert.Throws<InvalidOperationException>(() => tree.Remove(5));
    }

    [Test]
    public void Remove_AllElements_TreeIsEmpty()
    {
        var tree = new RedBlackTree<int>();
        var elements = new List<int> { 10, 5, 15, 3, 7, 12, 17 };
        tree.AddRange(elements);

        foreach (var el in elements)
        {
            tree.Remove(el);
        }
        Assert.That(tree.Count, Is.EqualTo(0));
    }

    [Test]
    public void Contains_ExistingElement_ReturnsTrue()
    {
        var tree = new RedBlackTree<int>();
        tree.Add(10);
        Assert.That(tree.Contains(10), Is.True);
    }

    [Test]
    public void Contains_NonExistingElement_ReturnsFalse()
    {
        var tree = new RedBlackTree<int>();
        tree.Add(10);
        Assert.That(tree.Contains(5), Is.False);
    }

    [Test]
    public void Contains_EmptyTree_ReturnsFalse()
    {
        var tree = new RedBlackTree<int>();
        Assert.That(tree.Contains(5), Is.False);
    }

    [Test]
    public void GetMin_PopulatedTree_ReturnsSmallestElement()
    {
        var tree = new RedBlackTree<int>();
        tree.AddRange(new[] { 10, 5, 15, 3, 7 });
        Assert.That(tree.GetMin(), Is.EqualTo(3));
    }

    [Test]
    public void GetMin_EmptyTree_ThrowsInvalidOperationException()
    {
        var tree = new RedBlackTree<int>();
        Assert.Throws<InvalidOperationException>(() => tree.GetMin());
    }

    [Test]
    public void GetMax_PopulatedTree_ReturnsLargestElement()
    {
        var tree = new RedBlackTree<int>();
        tree.AddRange(new[] { 10, 5, 15, 3, 7, 20 });
        Assert.That(tree.GetMax(), Is.EqualTo(20));
    }

    [Test]
    public void GetMax_EmptyTree_ThrowsInvalidOperationException()
    {
        var tree = new RedBlackTree<int>();
        Assert.Throws<InvalidOperationException>(() => tree.GetMax());
    }

    [Test]
    public void GetKeysInOrder_ReturnsSortedKeys()
    {
        var tree = new RedBlackTree<int>();
        var elements = new List<int> { 10, 5, 15, 3, 7, 12, 17 };
        tree.AddRange(elements);
        var sortedElements = elements.OrderBy(x => x).ToList();
        CollectionAssert.AreEqual(sortedElements, tree.GetKeysInOrder().ToList());
    }

    [Test]
    public void GetKeysInOrder_EmptyTree_ReturnsEmptyList()
    {
        var tree = new RedBlackTree<int>();
        CollectionAssert.IsEmpty(tree.GetKeysInOrder().ToList());
    }

    [Test]
    public void GetKeysPreOrder_ReturnsCorrectOrder()
    {
        var tree = new RedBlackTree<int>();
        // For a specific sequence, pre-order can be predicted
        // Example: 10, 5, 3, 7, 15, 12, 17 (this depends on insertion order and rotations)
        // Let's test with a known structure after specific insertions
        tree.Add(10); // Root (Black)
        tree.Add(5);  // Left of 10 (Red)
        tree.Add(15); // Right of 10 (Red)
        // Tree:    10(B)
        //         /   \
        //        5(R) 15(R)
        // After Add(3):
        // Tree:    10(B)
        //         /   \
        //        5(B) 15(B)  <- 5 and 15 become black, 10's parent (null) is black, 3 is red child of 5
        //       /
        //      3(R)
        tree.Add(3);
        // Expected PreOrder: 10, 5, 3, 15
        var expectedPreOrder = new List<int> { 10, 5, 3, 15 };
        CollectionAssert.AreEqual(expectedPreOrder, tree.GetKeysPreOrder().ToList());
    }

    [Test]
    public void GetKeysPostOrder_ReturnsCorrectOrder()
    {
        var tree = new RedBlackTree<int>();
        // Using the same tree structure as PreOrder test
        tree.Add(10);
        tree.Add(5);
        tree.Add(15);
        tree.Add(3);
        // Expected PostOrder: 3, 5, 15, 10
        var expectedPostOrder = new List<int> { 3, 5, 15, 10 };
        CollectionAssert.AreEqual(expectedPostOrder, tree.GetKeysPostOrder().ToList());
    }

    [Test]
    public void CustomComparer_StringLengthComparer_OrdersCorrectly()
    {
        var tree = new RedBlackTree<string>(Comparer<string>.Create((s1, s2) => s1.Length.CompareTo(s2.Length)));
        tree.Add("apple");    // len 5
        tree.Add("banana");   // len 6
        tree.Add("kiwi");     // len 4
        tree.Add("fig");      // len 3

        var expectedOrder = new List<string> { "fig", "kiwi", "apple", "banana" };
        CollectionAssert.AreEqual(expectedOrder, tree.GetKeysInOrder().ToList());
        Assert.That(tree.GetMin(), Is.EqualTo("fig"));
        Assert.That(tree.GetMax(), Is.EqualTo("banana"));
    }

    [Test]
    public void StressTest_AddAndRemoveManyElements()
    {
        var tree = new RedBlackTree<int>();
        var random = new Random(12345); // Seed for reproducibility
        var elements = new HashSet<int>();
        const int numOperations = 1000;

        for (int i = 0; i < numOperations; i++)
        {
            var value = random.Next(numOperations * 2);
            if (elements.Contains(value))
            {
                if (tree.Contains(value))
                {
                    tree.Remove(value);
                    elements.Remove(value);
                    Assert.That(tree.Count, Is.EqualTo(elements.Count));
                    Assert.That(tree.Contains(value), Is.False);
                }
            }
            else
            {
                tree.Add(value);
                elements.Add(value);
                Assert.That(tree.Count, Is.EqualTo(elements.Count));
                Assert.That(tree.Contains(value), Is.True);
            }
        }

        var sortedElements = elements.OrderBy(x => x).ToList();
        CollectionAssert.AreEqual(sortedElements, tree.GetKeysInOrder().ToList());
    }
}
