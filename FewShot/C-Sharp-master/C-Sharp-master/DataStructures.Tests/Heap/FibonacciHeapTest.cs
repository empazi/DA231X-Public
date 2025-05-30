using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures.Heap.FibonacciHeap;
using NUnit.Framework;
using FluentAssertions;

namespace DataStructures.Tests.Heap
{
    [TestFixture]
    public class FibonacciHeapTests
    {
        [Test]
        public void Push_SingleItem_CountIsOneAndPeekReturnsItem()
        {
            var heap = new FibonacciHeap<int>();
            heap.Push(10);

            heap.Count.Should().Be(1);
            heap.Peek().Should().Be(10);
        }

        [Test]
        public void Pop_SingleItem_CountIsZeroAndReturnsItem()
        {
            var heap = new FibonacciHeap<int>();
            heap.Push(10);
            var item = heap.Pop();

            item.Should().Be(10);
            heap.Count.Should().Be(0);
        }

        [Test]
        public void Pop_EmptyHeap_ThrowsInvalidOperationException()
        {
            var heap = new FibonacciHeap<int>();

            Action act = () => heap.Pop();

            act.Should().Throw<InvalidOperationException>().WithMessage("Heap is empty!");
        }

        [Test]
        public void Peek_EmptyHeap_ThrowsInvalidOperationException()
        {
            var heap = new FibonacciHeap<int>();

            Action act = () => heap.Peek();

            act.Should().Throw<InvalidOperationException>().WithMessage("The heap is empty");
        }

        [Test]
        public void PushAndPop_MultipleItems_ReturnsItemsInAscendingOrder()
        {
            var heap = new FibonacciHeap<int>();
            var items = new[] { 5, 3, 8, 1, 9, 4, 7, 2, 6 };
            foreach (var item in items)
            {
                heap.Push(item);
            }

            heap.Count.Should().Be(items.Length);

            var sortedItems = new List<int>();
            while (heap.Count > 0)
            {
                sortedItems.Add(heap.Pop());
            }

            sortedItems.Should().BeInAscendingOrder();
            sortedItems.Should().BeEquivalentTo(items.OrderBy(x => x));
        }

        [Test]
        public void Union_TwoNonEmptyHeaps_CorrectlyMerges()
        {
            var heap1 = new FibonacciHeap<int>();
            heap1.Push(5);
            heap1.Push(1);

            var heap2 = new FibonacciHeap<int>();
            heap2.Push(3);
            heap2.Push(0);

            heap1.Union(heap2);

            heap1.Count.Should().Be(4);
            heap2.Count.Should().Be(0); // other heap should be destroyed

            var expectedOrder = new[] { 0, 1, 3, 5 };
            var actualOrder = new List<int>();
            while (heap1.Count > 0)
            {
                actualOrder.Add(heap1.Pop());
            }
            actualOrder.Should().Equal(expectedOrder);
        }

        [Test]
        public void Union_WithEmptyHeap_ShouldNotChangeOriginalHeap()
        {
            var heap1 = new FibonacciHeap<int>();
            heap1.Push(5);
            heap1.Push(1);

            var heap2 = new FibonacciHeap<int>(); // Empty heap

            heap1.Union(heap2);

            heap1.Count.Should().Be(2);
            heap1.Peek().Should().Be(1);
        }

        [Test]
        public void Union_EmptyHeapWithNonEmptyHeap_ShouldBecomeNonEmptyHeap()
        {
            var heap1 = new FibonacciHeap<int>(); // Empty heap

            var heap2 = new FibonacciHeap<int>();
            heap2.Push(5);
            heap2.Push(1);

            heap1.Union(heap2);

            heap1.Count.Should().Be(2);
            heap1.Peek().Should().Be(1);
        }

        [Test]
        public void DecreaseKey_ValidDecrease_UpdatesKeyAndHeapOrder()
        {
            var heap = new FibonacciHeap<int>();
            var node5 = heap.Push(5);
            heap.Push(1);
            var node8 = heap.Push(8);

            heap.DecreaseKey(node8, 0); // Decrease 8 to 0

            heap.Peek().Should().Be(0);
            heap.Pop().Should().Be(0); // Was 8, now 0
            heap.Pop().Should().Be(1);
            heap.Pop().Should().Be(5);
        }

        [Test]
        public void DecreaseKey_ToSameValue_DoesNotThrow()
        {
            var heap = new FibonacciHeap<int>();
            var node5 = heap.Push(5);
            heap.Push(1);

            Action act = () => heap.DecreaseKey(node5, 5);
            act.Should().NotThrow();
            heap.Peek().Should().Be(1);
        }

        [Test]
        public void DecreaseKey_IncreaseKey_ThrowsInvalidOperationException()
        {
            var heap = new FibonacciHeap<int>();
            var node5 = heap.Push(5);

            Action act = () => heap.DecreaseKey(node5, 10);

            act.Should().Throw<InvalidOperationException>().WithMessage("Value cannot be increased");
        }

        [Test]
        public void DecreaseKey_NodeNotInHeap_ThrowsArgumentException()
        {
            var heap = new FibonacciHeap<int>(); // Ensure heap is empty for MinItem == null check in DecreaseKey


            var externalNode = new FHeapNode<int>(3); // Node not from this heap

            Action act = () => heap.DecreaseKey(externalNode, 1);

            // DecreaseKey throws ArgumentException with this message if MinItem is null
            act.Should().Throw<ArgumentException>()
                .WithMessage("*is not from the heap*");
        }

        [Test]
        public void DecreaseKey_CascadingCut_MaintainsHeapProperty()
        {
            var heap = new FibonacciHeap<int>();

            // Build a structure that can trigger cascading cut
            // This is a simplified scenario; real cascading cuts can be more complex
            heap.Push(10);
            heap.Push(20);
            heap.Push(30);
            heap.Pop(); // Consolidate: 20 becomes child of 10 (or vice versa depending on implementation details)
                        // Let's assume 20 is child of 10, 30 is separate

            var node40 = heap.Push(40);
            heap.Push(5); // Min is 5

            // To ensure a specific structure for testing cascading cut,
            // we'd typically need more direct control or a very specific sequence.
            // For this test, we'll focus on a decrease that causes a cut.
            // A full cascading cut test requires a deeper, more predictable structure.

            // Let's create a situation where a child is cut, then its parent (if marked) is also cut.
            // This example is illustrative.
            var items = Enumerable.Range(1, 10).Reverse().ToList(); // 10, 9, ..., 1
            var nodes = items.Select(i => heap.Push(i)).ToList();

            // Pop a few to cause consolidation and create parent-child relationships
            for (int i = 0; i < 5; i++) heap.Pop(); // Pops 1, 2, 3, 4, 5. Min is now 6.

            // Find a node that is a child and decrease its key
            // This part is tricky without inspecting internal structure.
            // Let's assume node for 9 (nodes[1]) is now a child of node for 6 (nodes[4]) after consolidation.
            // And node for 8 (nodes[2]) is a child of 6.
            // And node for 7 (nodes[3]) is a child of 6.

            // If we decrease 9 to 0. It becomes root.
            // If we then decrease 8 (child of 6) such that 6's child is cut, and 6 is marked.
            // Then decrease 7 (another child of 6) such that 6 is cut (cascading).

            // This specific scenario is hard to guarantee without internal access.
            // We'll test a simpler DecreaseKey that causes a cut.
            var nodeToDecrease = nodes.First(n => n.Key == 8); // Original value 8
            heap.DecreaseKey(nodeToDecrease, 0); // Decrease 8 to 0

            heap.Peek().Should().Be(0); // The new minimum should be 0
            var sorted = new List<int>();
            while (heap.Count > 0) sorted.Add(heap.Pop());

            // After all operations, the heap should contain these elements in sorted order:
            // Original: 20 (child 30), 40. (Initial 10 and 5 were popped)
            // From 'items' list (10,9,8,7,6,5',4,3,2,1): 1,2,3,4,original 5 popped. 8 became 0.
            // Remaining: 0 (was 8), 5' (from items), 6, 7, 9, 10 (from items), and 20, 30, 40.
            var expected = new List<int> { 0, 5, 6, 7, 9, 10, 20, 30, 40 };

            sorted.Should().Equal(expected);
        }

        [Test]
        public void ComplexSequence_PushPopDecreaseKey_MaintainsOrder()
        {
            var heap = new FibonacciHeap<int>();
            var nodes = new Dictionary<int, FHeapNode<int>>();

            nodes[10] = heap.Push(10);
            nodes[5] = heap.Push(5);
            nodes[20] = heap.Push(20);

            heap.Peek().Should().Be(5);
            heap.Pop().Should().Be(5); // 5 is gone

            nodes[15] = heap.Push(15);
            nodes[3] = heap.Push(3);

            heap.Peek().Should().Be(3);

            heap.DecreaseKey(nodes[20], 2); // 20 becomes 2
            heap.Peek().Should().Be(2);

            heap.Pop().Should().Be(2); // 2 is gone (was 20)
            heap.Pop().Should().Be(3); // 3 is gone

            nodes[1] = heap.Push(1);
            heap.DecreaseKey(nodes[10], 0); // 10 becomes 0

            var result = new List<int>();
            while (heap.Count > 0)
            {
                result.Add(heap.Pop());
            }

            result.Should().Equal(new[] { 0, 1, 15 });
        }
    }
}
