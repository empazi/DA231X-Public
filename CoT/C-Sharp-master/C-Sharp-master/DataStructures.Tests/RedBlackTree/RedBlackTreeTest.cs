using NUnit.Framework;
using DataStructures.RedBlackTree;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Legacy;

namespace DataStructures.Tests
{
    [TestFixture]
    public class RedBlackTreeTests
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable IDE1006 // Naming Styles
        private RedBlackTree<int> _tree;
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        [SetUp]
        public void Setup()
        {
            _tree = new RedBlackTree<int>();
        }

        // Constructor Tests
        [Test]
        public void DefaultConstructor_InitializesEmptyTree()
        {
            Assert.That(_tree.Count, Is.EqualTo(0));
            Assert.Throws<InvalidOperationException>(() => _tree.GetMin());
            Assert.Throws<InvalidOperationException>(() => _tree.GetMax()); // Already constraint-based style
            Assert.That(_tree.GetKeysInOrder(), Is.Empty);
            Assert.That(_tree.GetKeysPreOrder(), Is.Empty);
            Assert.That(_tree.GetKeysPostOrder(), Is.Empty);
        }

        [Test]
        public void CustomComparerConstructor_InitializesEmptyTree()
        {
            var customTree = new RedBlackTree<int>(new ReverseComparer<int>());
            Assert.That(customTree.Count, Is.EqualTo(0));
            Assert.Throws<InvalidOperationException>(() => customTree.GetMin());
            Assert.Throws<InvalidOperationException>(() => customTree.GetMax()); // Already constraint-based style
            Assert.That(customTree.GetKeysInOrder(), Is.Empty);
        }

        [Test]
        public void CustomComparerConstructor_AddsAndRetrievesInCustomOrder()
        {
            var customTree = new RedBlackTree<int>(new ReverseComparer<int>());
            customTree.Add(10);
            customTree.Add(5);
            customTree.Add(15);

            Assert.That(customTree.Count, Is.EqualTo(3));
            Assert.That(customTree.GetMin(), Is.EqualTo(15)); // Min according to reverse comparer is max
            Assert.That(customTree.GetMax(), Is.EqualTo(5));   // Max according to reverse comparer is min
            Assert.That(customTree.GetKeysInOrder(), Is.EqualTo(new[] { 15, 10, 5 }));
        }

        // Add Tests
        [Test]
        public void Add_ToEmptyTree_CountIsOne_ContainsElement()
        {
            _tree.Add(10);
            Assert.That(_tree.Count, Is.EqualTo(1));
            Assert.That(_tree.Contains(10), Is.True);
            Assert.That(_tree.GetMin(), Is.EqualTo(10));
            Assert.That(_tree.GetMax(), Is.EqualTo(10));
        }

        [Test]
        public void Add_MultipleElements_CountIsCorrect_AndElementsAreRetrievableInOrder()
        {
            _tree.Add(10);
            _tree.Add(5);
            _tree.Add(15);
            _tree.Add(3);
            _tree.Add(7);

            Assert.That(_tree.Count, Is.EqualTo(5));
            Assert.That(_tree.Contains(10), Is.True);
            Assert.That(_tree.Contains(5), Is.True);
            Assert.That(_tree.Contains(15), Is.True);
            Assert.That(_tree.Contains(3), Is.True);
            Assert.That(_tree.Contains(7), Is.True);
            Assert.That(_tree.Contains(100), Is.False);

            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(new[] { 3, 5, 7, 10, 15 }));
        }

        [Test]
        public void Add_ThrowsArgumentException_WhenAddingDuplicateKey()
        {
            _tree.Add(10);
            Assert.Throws<ArgumentException>(() => _tree.Add(10));
            Assert.That(_tree.Count, Is.EqualTo(1)); // Count should not change
        }

        [Test]
        public void Add_AscendingOrder_MaintainsBalance() // Triggers left rotations
        {
            for (int i = 1; i <= 10; i++)
            {
                _tree.Add(i);
            }
            Assert.That(_tree.Count, Is.EqualTo(10));
            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(Enumerable.Range(1, 10)));
        }

        [Test]
        public void Add_DescendingOrder_MaintainsBalance() // Triggers right rotations
        {
            for (int i = 10; i >= 1; i--)
            {
                _tree.Add(i);
            }
            Assert.That(_tree.Count, Is.EqualTo(10));
            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(Enumerable.Range(1, 10)));
        }

        [Test]
        public void Add_ComplexSequence_MaintainsBalance()
        {
            var numbers = new[] { 10, 85, 15, 70, 20, 60, 30, 50, 65, 80, 90, 40, 5, 55 };
            foreach (var num in numbers)
            {
                _tree.Add(num);
            }
            Assert.That(_tree.Count, Is.EqualTo(numbers.Length));
            Array.Sort(numbers);
            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(numbers));
        }

        // AddRange Tests
        [Test]
        public void AddRange_ToEmptyTree_AddsAllElements()
        {
            var items = new List<int> { 10, 5, 15, 3, 7 };
            _tree.AddRange(items);

            Assert.That(_tree.Count, Is.EqualTo(items.Count));
            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(items.OrderBy(x => x)));
            foreach (var item in items)
            {
                Assert.That(_tree.Contains(item), Is.True);
            }
        }

        [Test]
        public void AddRange_ToNonEmptyTree_AddsAllElements()
        {
            _tree.Add(100);
            _tree.Add(1);

            var items = new List<int> { 10, 5, 15, 3, 7 };
            _tree.AddRange(items);

            var expectedItems = new List<int> { 100, 1, 10, 5, 15, 3, 7 };
            Assert.That(_tree.Count, Is.EqualTo(expectedItems.Count));
            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(expectedItems.OrderBy(x => x)));

        }

        [Test]
        public void AddRange_WithDuplicatesInInput_ThrowsArgumentException()
        {
            var items = new List<int> { 10, 5, 10 }; // Duplicate 10
            Assert.Throws<ArgumentException>(() => _tree.AddRange(items));
            // Check state after exception: only elements before duplicate should be added
            Assert.That(_tree.Count, Is.EqualTo(2)); // 10, 5
            Assert.That(_tree.Contains(10), Is.True);
            Assert.That(_tree.Contains(5), Is.True);
        }

        [Test]
        public void AddRange_EmptyCollection_DoesNothing()
        {
            _tree.Add(1);
            _tree.AddRange(new List<int>());
            Assert.That(_tree.Count, Is.EqualTo(1));
            Assert.That(_tree.Contains(1), Is.True);
        }

        // Contains Tests
        [Test]
        public void Contains_EmptyTree_ReturnsFalse()
        {
            Assert.That(_tree.Contains(10), Is.False);
        }

        [Test]
        public void Contains_ExistingKey_ReturnsTrue()
        {
            _tree.Add(10);
            _tree.Add(5);
            Assert.That(_tree.Contains(10), Is.True);
            Assert.That(_tree.Contains(5), Is.True);
        }

        [Test]
        public void Contains_NonExistingKey_ReturnsFalse()
        {
            _tree.Add(10);
            Assert.That(_tree.Contains(5), Is.False);
        }

        // GetMin/GetMax Tests
        [Test]
        public void GetMin_EmptyTree_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _tree.GetMin());
        }

        [Test]
        public void GetMin_SingleElementTree_ReturnsElement()
        {
            _tree.Add(10);
            Assert.That(_tree.GetMin(), Is.EqualTo(10));
        }

        [Test]
        public void GetMin_MultipleElementsTree_ReturnsSmallest()
        {
            _tree.Add(10);
            _tree.Add(5);
            _tree.Add(15);
            Assert.That(_tree.GetMin(), Is.EqualTo(5));
        }

        [Test]
        public void GetMax_EmptyTree_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _tree.GetMax());
        }

        [Test]
        public void GetMax_SingleElementTree_ReturnsElement()
        {
            _tree.Add(10);
            Assert.That(_tree.GetMax(), Is.EqualTo(10));
        }

        [Test]
        public void GetMax_MultipleElementsTree_ReturnsLargest()
        {
            _tree.Add(10);
            _tree.Add(5);
            _tree.Add(15);
            Assert.That(_tree.GetMax(), Is.EqualTo(15));
        }

        // Traversal Tests
        [Test]
        public void GetKeysInOrder_EmptyTree_ReturnsEmptyCollection()
        {
            Assert.That(_tree.GetKeysInOrder(), Is.Empty);
        }

        [Test]
        public void GetKeysInOrder_PopulatedTree_ReturnsSortedCollection()
        {
            _tree.Add(10);
            _tree.Add(5);
            _tree.Add(15);
            _tree.Add(3);
            _tree.Add(7);
            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(new[] { 3, 5, 7, 10, 15 }));

        }

        [Test]
        public void GetKeysPreOrder_EmptyTree_ReturnsEmptyCollection()
        {
            Assert.That(_tree.GetKeysPreOrder(), Is.Empty);

        }

        [Test]
        public void GetKeysPreOrder_PopulatedTree_ReturnsCorrectOrder()
        {
            _tree.Add(10); _tree.Add(5); _tree.Add(15); _tree.Add(3); _tree.Add(7); _tree.Add(12); _tree.Add(17);
            // Expected structure after these adds (root 10(B), children 5(B),15(B), their children all Red):
            //      10(B)
            //     /    \
            //    5(B)   15(B)
            //   /  \   /  \
            //  3(R)7(R)12(R)17(R)
            var expectedPreOrder = new List<int> { 10, 5, 3, 7, 15, 12, 17 };
            Assert.That(_tree.GetKeysPreOrder(), Is.EqualTo(expectedPreOrder));
        }

        [Test]
        public void GetKeysPostOrder_EmptyTree_ReturnsEmptyCollection()
        {
            Assert.That(_tree.GetKeysPostOrder(), Is.Empty);
        }

        [Test]
        public void GetKeysPostOrder_PopulatedTree_ReturnsCorrectOrder()
        {
            _tree.Add(10); _tree.Add(5); _tree.Add(15); _tree.Add(3); _tree.Add(7); _tree.Add(12); _tree.Add(17);
            // Same structure as PreOrder test.
            // PostOrder: 3, 7, 5, 12, 17, 15, 10
            var expectedPostOrder = new List<int> { 3, 7, 5, 12, 17, 15, 10 };
            Assert.That(_tree.GetKeysPostOrder(), Is.EqualTo(expectedPostOrder));
        }

        // Remove Tests
        [Test]
        public void Remove_FromEmptyTree_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _tree.Remove(10));
        }

        [Test]
        public void Remove_NonExistingKey_ThrowsKeyNotFoundException()
        {
            _tree.Add(10);
            Assert.Throws<KeyNotFoundException>(() => _tree.Remove(5));
            Assert.That(_tree.Count, Is.EqualTo(1));
        }

        [Test]
        public void Remove_LeafNode_Black_ReducesCountAndNodeIsGone()
        {
            _tree.Add(10); _tree.Add(5); _tree.Add(15); // All become black due to balancing
                                                        // Structure: 10(B), L:5(B), R:15(B)

            _tree.Remove(5);
            Assert.That(_tree.Count, Is.EqualTo(2));
            Assert.That(_tree.Contains(5), Is.False);
            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(new[] { 10, 15 }));

            _tree.Remove(15);
            Assert.That(_tree.Count, Is.EqualTo(1));
            Assert.That(_tree.Contains(15), Is.False);
            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(new[] { 10 }));
        }

        [Test]
        public void Remove_LeafNode_Red_ReducesCountAndNodeIsGone()
        {
            _tree.Add(10); // B
            _tree.Add(5);  // R
            // Structure: 10(B), L:5(R)
            _tree.Remove(5);
            Assert.That(_tree.Count, Is.EqualTo(1));
            Assert.That(_tree.Contains(5), Is.False);
            Assert.That(_tree.Contains(10), Is.True);
            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(new[] { 10 }));
        }

        [Test]
        public void Remove_NodeWithOneChild_BlackNodeRedChild_ReducesCountAndNodeIsGone()
        {
            _tree.Add(10); _tree.Add(5); _tree.Add(15); _tree.Add(3);
            // Structure: 10(B) / L:5(B) \ R:15(B)
            //                 /
            //                3(R)
            _tree.Remove(5); // 5 is black, child 3 is red. 3 replaces 5, becomes black.
            Assert.That(_tree.Count, Is.EqualTo(3));
            Assert.That(_tree.Contains(5), Is.False);
            Assert.That(_tree.Contains(3), Is.True);
            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(new[] { 3, 10, 15 }));
        }

        [Test]
        public void Remove_NodeWithTwoChildren_SuccessorIsLeaf()
        {
            _tree.Add(10); _tree.Add(5); _tree.Add(20); _tree.Add(15); _tree.Add(25);
            // Structure (after balancing): 10(B) L:5(B) R:20(B) with 20 L:15(R) R:25(R)
            // Or root could be 15 or 20. Let's check InOrder: 5, 10, 15, 20, 25
            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(new[] { 5, 10, 15, 20, 25 }));

            _tree.Remove(10); // Successor of 10 is 15 (a leaf in its original position relative to 20).
            Assert.That(_tree.Count, Is.EqualTo(4));
            Assert.That(_tree.Contains(10), Is.False);
            Assert.That(_tree.Contains(15), Is.True);
            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(new[] { 5, 15, 20, 25 }));
        }

        [Test]
        public void Remove_NodeWithTwoChildren_SuccessorHasRightChild()
        {
            _tree.Add(10); _tree.Add(5); _tree.Add(20); _tree.Add(15); _tree.Add(25); _tree.Add(17);
            // InOrder: 5, 10, 15, 17, 20, 25
            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(new[] { 5, 10, 15, 17, 20, 25 }));


            _tree.Remove(10); // Successor of 10 is 15. Original 15 has right child 17.
            Assert.That(_tree.Count, Is.EqualTo(5));
            Assert.That(_tree.Contains(10), Is.False);
            Assert.That(_tree.Contains(15), Is.True);
            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(new[] { 5, 15, 17, 20, 25 }));

        }

        [Test]
        public void Remove_RootNode_SingleElementTree_TreeBecomesEmpty()
        {
            _tree.Add(10);
            _tree.Remove(10);
            Assert.That(_tree.Count, Is.EqualTo(0));
            Assert.That(_tree.GetKeysInOrder(), Is.Empty);
            Assert.Throws<InvalidOperationException>(() => _tree.GetMin());
        }

        [Test]
        public void Remove_RootNode_MultipleElementsTree_NewRootCorrect()
        {
            _tree.Add(10); _tree.Add(5); _tree.Add(15); // 10(B), 5(B), 15(B)
            _tree.Remove(10); // Successor is 15.
            Assert.That(_tree.Count, Is.EqualTo(2));
            Assert.That(_tree.Contains(10), Is.False);
            Assert.That(_tree.Contains(15), Is.True);
            Assert.That(_tree.Contains(5), Is.True);
            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(new[] { 5, 15 }));
            Assert.That(_tree.GetMin(), Is.EqualTo(5));
            Assert.That(_tree.GetMax(), Is.EqualTo(15));
        }

        [Test]
        public void Remove_AllElements_TreeBecomesEmpty()
        {
            var elements = new[] { 10, 5, 15, 3, 7, 12, 17 };
            _tree.AddRange(elements);
            Assert.That(_tree.Count, Is.EqualTo(elements.Length));

            foreach (var el in elements.OrderBy(x => Guid.NewGuid())) // Remove in random order
            {
                _tree.Remove(el);
            }

            Assert.That(_tree.Count, Is.EqualTo(0));
            Assert.That(_tree.GetKeysInOrder(), Is.Empty);
            Assert.Throws<InvalidOperationException>(() => _tree.GetMin());
        }

        [Test]
        public void Remove_ComplexSequence_MaintainsBalanceAndOrder()
        {
            var numbers = Enumerable.Range(0, 30).ToList();

            _tree.AddRange(numbers.OrderBy(x => Guid.NewGuid())); // Add in somewhat random order
            Assert.That(_tree.Count, Is.EqualTo(30));
            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(numbers.OrderBy(x => x)));

            var keysToRemove = numbers.Where((x, i) => i % 3 == 0).ToList();
            var remainingKeys = numbers.Except(keysToRemove).OrderBy(x => x).ToList();

            foreach (var key in keysToRemove.OrderBy(x => Guid.NewGuid())) // Remove in random order
            {
                _tree.Remove(key);
            }

            Assert.That(_tree.Count, Is.EqualTo(remainingKeys.Count));
            Assert.That(_tree.GetKeysInOrder(), Is.EqualTo(remainingKeys));
            foreach (var key in remainingKeys) Assert.That(_tree.Contains(key), Is.True);
            foreach (var key in keysToRemove) Assert.That(_tree.Contains(key), Is.False);
        }

        // Count Property Tests
        [Test]
        public void Count_InitiallyZero()
        {
            Assert.That(_tree.Count, Is.EqualTo(0));
        }

        [Test]
        public void Count_IncrementsOnAdd_DecrementsOnRemove()
        {
            Assert.That(_tree.Count, Is.EqualTo(0));
            _tree.Add(10);
            Assert.That(_tree.Count, Is.EqualTo(1));
            _tree.Add(20);
            Assert.That(_tree.Count, Is.EqualTo(2));
            _tree.Remove(10);
            Assert.That(_tree.Count, Is.EqualTo(1));
            _tree.Remove(20);
            Assert.That(_tree.Count, Is.EqualTo(0));
        }
    }

    // Helper comparer for testing custom comparer constructor
    public class ReverseComparer<T> : Comparer<T> where T : IComparable<T>
    {
        public override int Compare(T? x, T? y)
        {
            // Handle nulls according to reversed comparison logic:
            // Standard: null is "less than" non-null.
            // Reversed: null is "greater than" non-null.
            if (x == null && y == null) return 0; // Both null, considered equal.
            if (x == null) return 1;  // x is null, y is not. x is "greater" in reversed order.
            if (y == null) return -1; // y is null, x is not. x is "lesser" in reversed order.

            // Both x and y are not null.
            // Reverses the natural comparison order.
            // Calling y.CompareTo(x) achieves this:
            // If x < y naturally, then y > x, so y.CompareTo(x) is positive. ReverseComparer returns positive (x > y).
            // If x > y naturally, then y < x, so y.CompareTo(x) is negative. ReverseComparer returns negative (x < y).
            return y.CompareTo(x);
        }
    }
}
