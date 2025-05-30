using NUnit.Framework;
using DataStructures.RedBlackTree;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Legacy;

namespace DataStructures.Tests.RedBlackTree
{
    [TestFixture]
    public class RedBlackTreeTests
    {
        [Test]
        public void Constructor_Default_CreatesEmptyTree()
        {
            var tree = new RedBlackTree<int>();
            Assert.That(tree.Count, Is.EqualTo(0));
            Assert.That(tree.GetKeysInOrder(), Is.Empty);
        }

        [Test]
        public void Constructor_CustomComparer_UsesComparer()
        {
            var reverseComparer = Comparer<int>.Create((x, y) => y.CompareTo(x));
            var tree = new RedBlackTree<int>(reverseComparer);
            tree.Add(5);
            tree.Add(10);
            tree.Add(1);

            // With reverse comparer, 10 should be min, 1 should be max according to the comparer
            // GetMin/GetMax still return the absolute min/max based on the comparer's definition of "less"
            Assert.That(tree.GetMin(), Is.EqualTo(10)); // 10 is "smallest" with reverse comparer
            Assert.That(tree.GetMax(), Is.EqualTo(1));  // 1 is "largest" with reverse comparer
            CollectionAssert.AreEqual(new[] { 10, 5, 1 }, tree.GetKeysInOrder());
        }

        [Test]
        public void Add_SingleElement_AddsToTreeAndCountIsOne()
        {
            var tree = new RedBlackTree<int>();
            tree.Add(10);
            Assert.That(tree.Count, Is.EqualTo(1));
            Assert.That(tree.Contains(10), Is.True);
            Assert.That(tree.GetMin(), Is.EqualTo(10));
            Assert.That(tree.GetMax(), Is.EqualTo(10));
        }

        [Test]
        public void Add_MultipleElements_MaintainsOrderAndCount()
        {
            var tree = new RedBlackTree<int>();
            var elements = new[] { 10, 5, 15, 3, 7, 12, 17, 1, 20 };
            foreach (var el in elements)
            {
                tree.Add(el);
            }

            Assert.That(tree.Count, Is.EqualTo(elements.Length));
            Array.Sort(elements);
            CollectionAssert.AreEqual(elements, tree.GetKeysInOrder());
            foreach (var el in elements)
            {
                Assert.That(tree.Contains(el), Is.True);
            }
        }

        [Test]
        public void Add_DuplicateElement_ThrowsArgumentException()
        {
            var tree = new RedBlackTree<int>();
            tree.Add(10);
            Assert.Throws<ArgumentException>(() => tree.Add(10));
            Assert.That(tree.Count, Is.EqualTo(1)); // Count should not change
        }

        [Test]
        public void Add_AscendingOrder_MaintainsBalanceAndOrder()
        {
            var tree = new RedBlackTree<int>();
            var elements = Enumerable.Range(1, 100).ToArray();
            foreach (var el in elements)
            {
                tree.Add(el);
            }

            Assert.That(tree.Count, Is.EqualTo(elements.Length));
            CollectionAssert.AreEqual(elements, tree.GetKeysInOrder());
            Assert.That(tree.GetMin(), Is.EqualTo(1));
            Assert.That(tree.GetMax(), Is.EqualTo(100));
        }

        [Test]
        public void Add_DescendingOrder_MaintainsBalanceAndOrder()
        {
            var tree = new RedBlackTree<int>();
            var elements = Enumerable.Range(1, 100).Reverse().ToArray();
            foreach (var el in elements)
            {
                tree.Add(el);
            }

            Assert.That(tree.Count, Is.EqualTo(elements.Length));
            Array.Sort(elements); // Sort for comparison with InOrder
            CollectionAssert.AreEqual(elements, tree.GetKeysInOrder());
            Assert.That(tree.GetMin(), Is.EqualTo(1));
            Assert.That(tree.GetMax(), Is.EqualTo(100));
        }

        [Test]
        public void Add_SpecificCase_TriggeringRecolorsAndRotations()
        {
            var tree = new RedBlackTree<int>();
            // This sequence is known to trigger various RB cases.
            // We verify the end state (count, contains, order).
            var elements = new[] { 10, 85, 15, 70, 20, 60, 30, 50, 65, 80, 90, 40, 5, 55 };
            foreach (var el in elements)
            {
                tree.Add(el);
            }

            Assert.That(tree.Count, Is.EqualTo(elements.Length));
            Array.Sort(elements);
            CollectionAssert.AreEqual(elements, tree.GetKeysInOrder());
            foreach (var el in elements)
            {
                Assert.That(tree.Contains(el), Is.True);
            }
            Assert.That(tree.GetMin(), Is.EqualTo(elements.First()));
            Assert.That(tree.GetMax(), Is.EqualTo(elements.Last()));
        }


        [Test]
        public void AddRange_MultipleElements_AddsAllToTree()
        {
            var tree = new RedBlackTree<int>();
            var elements = new[] { 10, 5, 15, 3, 7, 12, 17 };
            tree.AddRange(elements);

            Assert.That(tree.Count, Is.EqualTo(elements.Length));
            Array.Sort(elements);
            CollectionAssert.AreEqual(elements, tree.GetKeysInOrder());
        }

        [Test]
        public void AddRange_EmptyCollection_TreeRemainsEmpty()
        {
            var tree = new RedBlackTree<int>();
            tree.AddRange(new List<int>());
            Assert.That(tree.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddRange_WithDuplicatesInCollection_ThrowsArgumentExceptionAndAddsPreceding()
        {
            var tree = new RedBlackTree<int>();
            var elements = new[] { 10, 5, 10, 15 }; // Duplicate 10

            Assert.Throws<ArgumentException>(() => tree.AddRange(elements));
            // Elements before the duplicate should be added
            Assert.That(tree.Count, Is.EqualTo(2));
            Assert.That(tree.Contains(10), Is.True);
            Assert.That(tree.Contains(5), Is.True);
            Assert.That(tree.Contains(15), Is.False); // Should not be added
            CollectionAssert.AreEqual(new[] { 5, 10 }, tree.GetKeysInOrder());
        }

        [Test]
        public void Remove_FromEmptyTree_ThrowsInvalidOperationException()
        {
            var tree = new RedBlackTree<int>();
            Assert.Throws<InvalidOperationException>(() => tree.Remove(10));
        }

        [Test]
        public void Remove_NonExistentKey_ThrowsKeyNotFoundException()
        {
            var tree = new RedBlackTree<int>();
            tree.Add(10);
            tree.Add(5);
            Assert.Throws<KeyNotFoundException>(() => tree.Remove(100));
            Assert.That(tree.Count, Is.EqualTo(2)); // Count should not change
        }

        [Test]
        public void Remove_SingleElementTree_TreeBecomesEmpty()
        {
            var tree = new RedBlackTree<int>();
            tree.Add(10);
            tree.Remove(10);
            Assert.That(tree.Count, Is.EqualTo(0));
            Assert.That(tree.Contains(10), Is.False);
            Assert.That(tree.GetKeysInOrder(), Is.Empty);
        }

        [Test]
        public void Remove_LeafNode_RemovesCorrectly()
        {
            var tree = new RedBlackTree<int>();
            // Structure after adds: 10(B) <- (5(B) <- 3(R)), -> 15(B)
            tree.AddRange(new[] { 10, 5, 15, 3 });
            tree.Remove(3); // 3 is a red leaf
            Assert.That(tree.Count, Is.EqualTo(3));
            Assert.That(tree.Contains(3), Is.False);
            CollectionAssert.AreEqual(new[] { 5, 10, 15 }, tree.GetKeysInOrder());

            tree.Remove(15); // 15 is a black leaf
            Assert.That(tree.Count, Is.EqualTo(2));
            Assert.That(tree.Contains(15), Is.False);
            CollectionAssert.AreEqual(new[] { 5, 10 }, tree.GetKeysInOrder());
        }

        [Test]
        public void Remove_NodeWithOneChild_RemovesCorrectly()
        {
            var tree = new RedBlackTree<int>();
            // Structure after adds: 10(B) <- (5(B) <- 3(R)), -> 15(B)
            tree.AddRange(new[] { 10, 5, 15, 3 });
            // Remove 5 (black node with one red child 3)
            tree.Remove(5);
            Assert.That(tree.Count, Is.EqualTo(3));
            Assert.That(tree.Contains(5), Is.False);
            // Expected: 3 becomes black and replaces 5. 10(B) <- 3(B), -> 15(B)
            CollectionAssert.AreEqual(new[] { 3, 10, 15 }, tree.GetKeysInOrder());
        }

        [Test]
        public void Remove_NodeWithTwoChildren_RemovesCorrectly_SuccessorIsLeaf()
        {
            var tree = new RedBlackTree<int>();
            tree.AddRange(new[] { 10, 5, 15, 3, 7, 12, 20, 11 }); // 11 is R child of 12(B)
            // Remove 10. Successor is 11 (a leaf in its subtree context).
            tree.Remove(10);
            Assert.That(tree.Count, Is.EqualTo(7));
            Assert.That(tree.Contains(10), Is.False);
            CollectionAssert.AreEqual(new[] { 3, 5, 7, 11, 12, 15, 20 }, tree.GetKeysInOrder());
        }

        [Test]
        public void Remove_NodeWithTwoChildren_RemovesCorrectly_SuccessorHasRightChild()
        {
            var tree = new RedBlackTree<int>();
            tree.AddRange(new[] { 10, 5, 15, 12, 20, 13 });
            // Remove 10. Successor is 12. 12 has right child 13.
            tree.Remove(10);
            Assert.That(tree.Count, Is.EqualTo(5));
            Assert.That(tree.Contains(10), Is.False);
            CollectionAssert.AreEqual(new[] { 5, 12, 13, 15, 20 }, tree.GetKeysInOrder());
        }

        [Test]
        public void Remove_RootNodeMultipleTimes_MaintainsIntegrity()
        {
            var tree = new RedBlackTree<int>();
            var elements = new List<int> { 50, 25, 75, 10, 30, 60, 80, 5, 15, 28, 35, 55, 65, 78, 85 };
            tree.AddRange(elements);

            var currentElements = new List<int>(elements);

            // Remove current root (50), its successor (55) will become the new value at root's original position
            tree.Remove(50);
            currentElements.Remove(50);
            Assert.That(tree.Count, Is.EqualTo(currentElements.Count));
            CollectionAssert.AreEqual(currentElements.OrderBy(x => x), tree.GetKeysInOrder());
            Assert.That(tree.Contains(50), Is.False);

            // The key 55 was moved to the root's original structural position. Now remove key 55.
            // The actual root node's key might be different after balancing.
            // We need to find the new root to remove it, or just remove by value.
            // Let's remove 55 (which was the successor of 50)
            tree.Remove(55);
            currentElements.Remove(55);
            Assert.That(tree.Count, Is.EqualTo(currentElements.Count));
            CollectionAssert.AreEqual(currentElements.OrderBy(x => x), tree.GetKeysInOrder());
            Assert.That(tree.Contains(55), Is.False);

            // Remove 60 (successor of 55 in the remaining set)
            tree.Remove(60);
            currentElements.Remove(60);
            Assert.That(tree.Count, Is.EqualTo(currentElements.Count));
            CollectionAssert.AreEqual(currentElements.OrderBy(x => x), tree.GetKeysInOrder());
            Assert.That(tree.Contains(60), Is.False);
        }

        [Test]
        public void Remove_AllElements_TreeBecomesEmpty()
        {
            var tree = new RedBlackTree<int>();
            var elements = new[] { 10, 5, 15, 3, 7, 12, 17, 1, 20, 30, 25 };
            tree.AddRange(elements);

            var randomOrderToRemove = elements.OrderBy(x => Guid.NewGuid()).ToList();
            foreach (var el in randomOrderToRemove)
            {
                tree.Remove(el);
                Assert.That(tree.Contains(el), Is.False, $"Element {el} should have been removed.");
            }

            Assert.That(tree.Count, Is.EqualTo(0));
            Assert.That(tree.GetKeysInOrder(), Is.Empty);
        }

        [Test]
        public void Remove_ComplexScenario_MaintainsIntegrity()
        {
            var tree = new RedBlackTree<int>();
            var initialElements = Enumerable.Range(1, 30).ToList();
            tree.AddRange(initialElements);

            var currentElements = new List<int>(initialElements);

            var elementsToRemove = new List<int> { 15, 7, 22, 1, 30, 10, 20 };
            foreach (var el in elementsToRemove)
            {
                tree.Remove(el);
                currentElements.Remove(el);
                Assert.That(tree.Count, Is.EqualTo(currentElements.Count));
                Assert.That(tree.Contains(el), Is.False);
                CollectionAssert.AreEqual(currentElements.OrderBy(x => x), tree.GetKeysInOrder());
            }

            var elementsToAdd = new List<int> { -5, 100, 16 }; // 16 was not in elementsToRemove, so it's still there if not removed.
                                                               // Let's assume 16 was removed if it was in initialElements and elementsToRemove
                                                               // For this test, let's ensure 16 is added back or added if not present.
            if (elementsToRemove.Contains(16)) currentElements.Add(16); // If it was removed, add it to tracking list
            else if (!currentElements.Contains(16)) currentElements.Add(16); // If never there, add to tracking

            foreach (var el in elementsToAdd)
            {
                if (tree.Contains(el)) // If it's already there from a previous add (e.g. 16)
                {
                    Assert.Throws<ArgumentException>(() => tree.Add(el)); // Adding duplicate
                }
                else
                {
                    tree.Add(el);
                    currentElements.Add(el);
                }
                Assert.That(tree.Count, Is.EqualTo(currentElements.Count(x => tree.Contains(x)))); // Ensure count matches distinct elements
                Assert.That(tree.Contains(el), Is.True);
                CollectionAssert.AreEqual(currentElements.Distinct().OrderBy(x => x), tree.GetKeysInOrder());
            }
        }


        [Test]
        public void Contains_OnEmptyTree_ReturnsFalse()
        {
            var tree = new RedBlackTree<int>();
            Assert.That(tree.Contains(10), Is.False);
        }

        [Test]
        public void Contains_ExistingKey_ReturnsTrue()
        {
            var tree = new RedBlackTree<int>();
            tree.Add(10);
            tree.Add(5);
            Assert.That(tree.Contains(5), Is.True);
            Assert.That(tree.Contains(10), Is.True);
        }

        [Test]
        public void Contains_NonExistingKey_ReturnsFalse()
        {
            var tree = new RedBlackTree<int>();
            tree.Add(10);
            tree.Add(5);
            Assert.That(tree.Contains(100), Is.False);
            Assert.That(tree.Contains(1), Is.False);
        }

        [Test]
        public void GetMin_OnEmptyTree_ThrowsInvalidOperationException()
        {
            var tree = new RedBlackTree<int>();
            Assert.Throws<InvalidOperationException>(() => tree.GetMin());
        }

        [Test]
        public void GetMin_SingleElementTree_ReturnsElement()
        {
            var tree = new RedBlackTree<int>();
            tree.Add(10);
            Assert.That(tree.GetMin(), Is.EqualTo(10));
        }

        [Test]
        public void GetMin_MultipleElements_ReturnsSmallest()
        {
            var tree = new RedBlackTree<int>();
            tree.AddRange(new[] { 10, 5, 15, 3, 20, 1, 7 });
            Assert.That(tree.GetMin(), Is.EqualTo(1));
        }

        [Test]
        public void GetMax_OnEmptyTree_ThrowsInvalidOperationException()
        {
            var tree = new RedBlackTree<int>();
            Assert.Throws<InvalidOperationException>(() => tree.GetMax());
        }

        [Test]
        public void GetMax_SingleElementTree_ReturnsElement()
        {
            var tree = new RedBlackTree<int>();
            tree.Add(10);
            Assert.That(tree.GetMax(), Is.EqualTo(10));
        }

        [Test]
        public void GetMax_MultipleElements_ReturnsLargest()
        {
            var tree = new RedBlackTree<int>();
            tree.AddRange(new[] { 10, 5, 15, 3, 20, 1, 7 });
            Assert.That(tree.GetMax(), Is.EqualTo(20));
        }

        [Test]
        public void GetKeysInOrder_EmptyTree_ReturnsEmptyEnumerable()
        {
            var tree = new RedBlackTree<int>();
            Assert.That(tree.GetKeysInOrder(), Is.Empty);
        }

        [Test]
        public void GetKeysInOrder_SingleElementTree_ReturnsElement()
        {
            var tree = new RedBlackTree<int>();
            tree.Add(10);
            CollectionAssert.AreEqual(new[] { 10 }, tree.GetKeysInOrder());
        }

        [Test]
        public void GetKeysInOrder_MultipleElements_ReturnsSortedOrder()
        {
            var tree = new RedBlackTree<int>();
            var elements = new[] { 10, 5, 15, 3, 7, 12, 17, 1, 20 };
            tree.AddRange(elements);
            Array.Sort(elements);
            CollectionAssert.AreEqual(elements, tree.GetKeysInOrder());
        }

        [Test]
        public void GetKeysPreOrder_EmptyTree_ReturnsEmptyEnumerable()
        {
            var tree = new RedBlackTree<string>();
            Assert.That(tree.GetKeysPreOrder(), Is.Empty);
        }

        [Test]
        public void GetKeysPreOrder_PopulatedTree_ReturnsCorrectOrder()
        {
            var tree = new RedBlackTree<int>();
            // Sequence: 10, 5, 15, 3, 7, 12, 17
            // After RB balancing, a possible structure (actual may vary but traversals are defined):
            // Add 10: 10(B)
            // Add 5:  10(B) L-5(R)
            // Add 15: 10(B) L-5(R), R-15(R)
            // PreOrder: 10, 5, 15
            tree.Add(10); tree.Add(5); tree.Add(15);
            CollectionAssert.AreEqual(new[] { 10, 5, 15 }, tree.GetKeysPreOrder().ToList());

            // Add 3: Parent 5(R), Uncle 15(R). Recolor 5(B), 15(B), 10(R)->10(B) root.
            // Tree: 10(B) L-(5(B) L-3(R)), R-15(B)
            // PreOrder: 10, 5, 3, 15
            tree.Add(3);
            CollectionAssert.AreEqual(new[] { 10, 5, 3, 15 }, tree.GetKeysPreOrder().ToList());

            // Add 7: Parent 5(B).
            // Tree: 10(B) L-(5(B) L-3(R), R-7(R)), R-15(B)
            // PreOrder: 10, 5, 3, 7, 15
            tree.Add(7);
            CollectionAssert.AreEqual(new[] { 10, 5, 3, 7, 15 }, tree.GetKeysPreOrder().ToList());

            // Add 12: Parent 15(B).
            // Tree: 10(B) L-(5(B) L-3(R),R-7(R)), R-(15(B) L-12(R))
            // PreOrder: 10, 5, 3, 7, 15, 12
            tree.Add(12);
            CollectionAssert.AreEqual(new[] { 10, 5, 3, 7, 15, 12 }, tree.GetKeysPreOrder().ToList());

            // Add 17: Parent 15(B).
            // Tree: 10(B) L-(5(B) L-3(R),R-7(R)), R-(15(B) L-12(R), R-17(R))
            // PreOrder: 10, 5, 3, 7, 15, 12, 17
            tree.Add(17);
            CollectionAssert.AreEqual(new[] { 10, 5, 3, 7, 15, 12, 17 }, tree.GetKeysPreOrder().ToList());
        }

        [Test]
        public void GetKeysPostOrder_EmptyTree_ReturnsEmptyEnumerable()
        {
            var tree = new RedBlackTree<string>();
            Assert.That(tree.GetKeysPostOrder(), Is.Empty);
        }

        [Test]
        public void GetKeysPostOrder_PopulatedTree_ReturnsCorrectOrder()
        {
            var tree = new RedBlackTree<int>();
            // Using the same tree structure as PreOrder test
            // Add 10, 5, 15
            // Tree: 10(B) L-5(R), R-15(R)
            // PostOrder: 5, 15, 10
            tree.Add(10); tree.Add(5); tree.Add(15);
            CollectionAssert.AreEqual(new[] { 5, 15, 10 }, tree.GetKeysPostOrder().ToList());

            // Add 3
            // Tree: 10(B) L-(5(B) L-3(R)), R-15(B)
            // PostOrder: 3, 5, 15, 10
            tree.Add(3);
            CollectionAssert.AreEqual(new[] { 3, 5, 15, 10 }, tree.GetKeysPostOrder().ToList());

            // Add 7
            // Tree: 10(B) L-(5(B) L-3(R), R-7(R)), R-15(B)
            // PostOrder: 3, 7, 5, 15, 10
            tree.Add(7);
            CollectionAssert.AreEqual(new[] { 3, 7, 5, 15, 10 }, tree.GetKeysPostOrder().ToList());

            // Add 12
            // Tree: 10(B) L-(5(B) L-3(R),R-7(R)), R-(15(B) L-12(R))
            // PostOrder: 3, 7, 5, 12, 15, 10
            tree.Add(12);
            CollectionAssert.AreEqual(new[] { 3, 7, 5, 12, 15, 10 }, tree.GetKeysPostOrder().ToList());

            // Add 17
            // Tree: 10(B) L-(5(B) L-3(R),R-7(R)), R-(15(B) L-12(R), R-17(R))
            // PostOrder: 3, 7, 5, 12, 17, 15, 10
            tree.Add(17);
            CollectionAssert.AreEqual(new[] { 3, 7, 5, 12, 17, 15, 10 }, tree.GetKeysPostOrder().ToList());
        }

        [Test]
        public void Count_ReflectsNumberOfElements()
        {
            var tree = new RedBlackTree<int>();
            Assert.That(tree.Count, Is.EqualTo(0));

            tree.Add(10);
            Assert.That(tree.Count, Is.EqualTo(1));

            tree.Add(5);
            Assert.That(tree.Count, Is.EqualTo(2));

            tree.AddRange(new[] { 1, 20, 15 });
            Assert.That(tree.Count, Is.EqualTo(5));

            tree.Remove(10);
            Assert.That(tree.Count, Is.EqualTo(4));

            tree.Remove(1);
            Assert.That(tree.Count, Is.EqualTo(3));
            tree.Remove(20);
            Assert.That(tree.Count, Is.EqualTo(2));
            tree.Remove(15);
            Assert.That(tree.Count, Is.EqualTo(1));
            tree.Remove(5);
            Assert.That(tree.Count, Is.EqualTo(0));
        }
    }
}
