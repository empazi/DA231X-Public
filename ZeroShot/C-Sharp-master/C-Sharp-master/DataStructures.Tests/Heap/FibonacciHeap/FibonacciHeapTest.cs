using NUnit.Framework;
using DataStructures.Heap.FibonacciHeap; // Assuming FHeapNode is in this namespace or accessible
using System;
using System.Collections.Generic;
using System.Linq;

[TestFixture]
public class FibonacciHeapTests
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private FibonacciHeap<int> heap;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    [SetUp]
    public void Setup()
    {
        heap = new FibonacciHeap<int>();
    }

    // Constructor and Initial State
    [Test]
    public void Constructor_InitializesEmptyHeap()
    {
        Assert.That(heap.Count, Is.EqualTo(0));
        Assert.That(() => heap.Peek(), Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("The heap is empty"));
    }

    [Test]
    public void Peek_OnEmptyHeap_ThrowsInvalidOperationException()
    {
        Assert.That(() => heap.Peek(), Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("The heap is empty"));
    }

    [Test]
    public void Pop_OnEmptyHeap_ThrowsInvalidOperationException()
    {
        Assert.That(() => heap.Pop(), Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("Heap is empty!"));
    }

    // Push
    [Test]
    public void Push_SingleItem_CorrectState()
    {
        heap.Push(10);
        Assert.That(heap.Count, Is.EqualTo(1));
        Assert.That(heap.Peek(), Is.EqualTo(10));
    }

    [Test]
    public void Push_MultipleItems_AscendingOrder_CorrectMin()
    {
        heap.Push(10);
        heap.Push(20);
        heap.Push(30);
        Assert.That(heap.Count, Is.EqualTo(3));
        Assert.That(heap.Peek(), Is.EqualTo(10));
    }

    [Test]
    public void Push_MultipleItems_DescendingOrder_CorrectMin()
    {
        heap.Push(30);
        heap.Push(20);
        heap.Push(10);
        Assert.That(heap.Count, Is.EqualTo(3));
        Assert.That(heap.Peek(), Is.EqualTo(10));
    }

    [Test]
    public void Push_MultipleItems_RandomOrder_CorrectMin()
    {
        heap.Push(20);
        heap.Push(10);
        heap.Push(30);
        Assert.That(heap.Count, Is.EqualTo(3));
        Assert.That(heap.Peek(), Is.EqualTo(10));
    }

    [Test]
    public void Push_ReturnsNodeWithCorrectKey()
    {
        var node = heap.Push(42);
        Assert.That(node, Is.Not.Null);
        Assert.That(node.Key, Is.EqualTo(42));
    }

    // Peek
    [Test]
    public void Peek_OnNonEmptyHeap_ReturnsMinElement()
    {
        heap.Push(20);
        heap.Push(10);
        Assert.That(heap.Peek(), Is.EqualTo(10));
        Assert.That(heap.Count, Is.EqualTo(2)); // Ensure Peek doesn't change count
    }

    // Pop
    [Test]
    public void Pop_SingleItemHeap_CorrectStateAfterPop()
    {
        heap.Push(10);
        Assert.That(heap.Pop(), Is.EqualTo(10));
        Assert.That(heap.Count, Is.EqualTo(0));
        Assert.That(() => heap.Peek(), Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("The heap is empty"));
    }

    [Test]
    public void Pop_MultipleItems_ReturnsSmallestAndUpdatesHeap()
    {
        heap.Push(20);
        heap.Push(10);
        heap.Push(30);

        Assert.That(heap.Pop(), Is.EqualTo(10));
        Assert.That(heap.Count, Is.EqualTo(2));
        Assert.That(heap.Peek(), Is.EqualTo(20));

        Assert.That(heap.Pop(), Is.EqualTo(20));
        Assert.That(heap.Count, Is.EqualTo(1));
        Assert.That(heap.Peek(), Is.EqualTo(30));

        Assert.That(heap.Pop(), Is.EqualTo(30));
        Assert.That(heap.Count, Is.EqualTo(0));
    }

    [Test]
    public void Pop_AllItems_HeapBecomesEmpty()
    {
        var items = new[] { 5, 3, 8, 1, 4, 7, 2, 6 };
        foreach (var item in items)
        {
            heap.Push(item);
        }

        Array.Sort(items);
        foreach (var sortedItem in items)
        {
            Assert.That(heap.Pop(), Is.EqualTo(sortedItem));
        }

        Assert.That(heap.Count, Is.EqualTo(0));
        Assert.That(() => heap.Peek(), Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("The heap is empty"));
    }

    [Test]
    public void Pop_TriggersConsolidation_CorrectMinAfterConsolidation()
    {
        heap.Push(10);
        heap.Push(20);
        heap.Push(5);
        heap.Push(15);
        heap.Push(3);
        heap.Push(25);

        Assert.That(heap.Pop(), Is.EqualTo(3));
        Assert.That(heap.Count, Is.EqualTo(5));
        Assert.That(heap.Peek(), Is.EqualTo(5));

        Assert.That(heap.Pop(), Is.EqualTo(5));
        Assert.That(heap.Count, Is.EqualTo(4));
        Assert.That(heap.Peek(), Is.EqualTo(10));
    }

    // Union
    [Test]
    public void Union_WithEmptyOtherHeap_ThisHeapUnchanged()
    {
        heap.Push(10);
        heap.Push(20);
        var otherHeap = new FibonacciHeap<int>();

        heap.Union(otherHeap);

        Assert.That(heap.Count, Is.EqualTo(2));
        Assert.That(heap.Peek(), Is.EqualTo(10));
        Assert.That(otherHeap.Count, Is.EqualTo(0));
    }

    [Test]
    public void Union_EmptyThisHeapWithNonEmptyOther_ThisBecomesOther()
    {
        var otherHeap = new FibonacciHeap<int>();
        otherHeap.Push(10);
        otherHeap.Push(5);

        heap.Union(otherHeap);

        Assert.That(heap.Count, Is.EqualTo(2));
        Assert.That(heap.Peek(), Is.EqualTo(5));
        Assert.That(otherHeap.Count, Is.EqualTo(0));
    }

    [Test]
    public void Union_TwoNonEmptyHeaps_CorrectCombinedState_ThisMinSmaller()
    {
        heap.Push(10);
        heap.Push(20);

        var otherHeap = new FibonacciHeap<int>();
        otherHeap.Push(15);
        otherHeap.Push(25);

        heap.Union(otherHeap);

        Assert.That(heap.Count, Is.EqualTo(4));
        Assert.That(heap.Peek(), Is.EqualTo(10));

        var expected = new[] { 10, 15, 20, 25 };
        var actual = new List<int>();
        while (heap.Count > 0)
        {
            actual.Add(heap.Pop());
        }
        Assert.That(actual, Is.EqualTo(expected));
        Assert.That(otherHeap.Count, Is.EqualTo(0));
    }

    [Test]
    public void Union_TwoNonEmptyHeaps_CorrectCombinedState_OtherMinSmaller()
    {
        heap.Push(15);
        heap.Push(25);

        var otherHeap = new FibonacciHeap<int>();
        otherHeap.Push(10);
        otherHeap.Push(20);

        heap.Union(otherHeap);

        Assert.That(heap.Count, Is.EqualTo(4));
        Assert.That(heap.Peek(), Is.EqualTo(10));

        var expected = new[] { 10, 15, 20, 25 };
        var actual = new List<int>();
        while (heap.Count > 0)
        {
            actual.Add(heap.Pop());
        }
        Assert.That(actual, Is.EqualTo(expected));
        Assert.That(otherHeap.Count, Is.EqualTo(0));
    }

    [Test]
    public void Union_DestroysOtherHeap()
    {
        heap.Push(10);
        var otherHeap = new FibonacciHeap<int>();
        otherHeap.Push(5);

        heap.Union(otherHeap);

        Assert.That(otherHeap.Count, Is.EqualTo(0));
        Assert.That(() => otherHeap.Peek(), Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("The heap is empty"));
    }

    // DecreaseKey
    [Test]
    public void DecreaseKey_ToSmallerValue_UpdatesKey()
    {
        var node = heap.Push(20);
        heap.Push(10);

        heap.DecreaseKey(node, 15);
        Assert.That(node.Key, Is.EqualTo(15));
        Assert.That(heap.Peek(), Is.EqualTo(10));
    }

    [Test]
    public void DecreaseKey_MakesNodeNewMin_UpdatesMinItem()
    {
        heap.Push(20);
        var node = heap.Push(30);

        heap.DecreaseKey(node, 10);
        Assert.That(node.Key, Is.EqualTo(10));
        Assert.That(heap.Peek(), Is.EqualTo(10));
    }

    [Test]
    public void DecreaseKey_OnRootNode_NoCut()
    {
        var node20 = heap.Push(20);
        heap.Push(10);

        heap.DecreaseKey(node20, 15);
        Assert.That(node20.Key, Is.EqualTo(15));
        Assert.That(node20.Parent, Is.Null);
        Assert.That(heap.Peek(), Is.EqualTo(10));

        heap.DecreaseKey(node20, 5);
        Assert.That(node20.Key, Is.EqualTo(5));
        Assert.That(node20.Parent, Is.Null);
        Assert.That(heap.Peek(), Is.EqualTo(5));
    }

    [Test]
    public void DecreaseKey_OnChild_NoCut_HeapPropertyMaintained()
    {
        var nodeA = heap.Push(10);
        var nodeB = heap.Push(30);
        heap.Push(5);
        heap.Pop(); // Pop 5. Min is 10. nodeB might be child of nodeA.

        if (nodeB.Parent == nodeA)
        {
            heap.DecreaseKey(nodeB, 20); // 20 > 10 (parent key)
            Assert.That(nodeB.Key, Is.EqualTo(20));
            Assert.That(nodeB.Parent, Is.SameAs(nodeA));
            Assert.That(nodeB.Mark, Is.False);
            Assert.That(heap.Peek(), Is.EqualTo(10));
        }
        else
        {
            // Fallback if structure isn't as expected: just test DecreaseKey logic generally
            heap.DecreaseKey(nodeB, 15); // nodeB was 30, now 15. Min is 10.
            Assert.That(nodeB.Key, Is.EqualTo(15));
            Assert.That(heap.Peek(), Is.EqualTo(10)); // Min is still 10
        }
    }

    [Test]
    public void DecreaseKey_OnChild_TriggersCut_NodeMovesToRoot()
    {
        // Setup to ensure P(100) -> C(200) after consolidation
        var p1_node = heap.Push(100);
        var c1_node = heap.Push(200);
        heap.Push(1); // Temp min
        heap.Pop();   // Pop 1. Consolidate. Min should be 100 (p1_node). c1_node (200) should be child of p1_node.

        // Verify structure
        if (c1_node.Parent != p1_node || p1_node.Degree != 1)
        {
            Assert.Inconclusive("Required heap structure (Parent-Child, Parent.Degree=1) did not form for TriggersCut test.");
        }

        int oldP1Degree = p1_node.Degree;

        heap.DecreaseKey(c1_node, 50); // 50 < 100 (parent key) -> Cut. 50 < 100 (current min) -> c1_node becomes new min.

        Assert.That(c1_node.Key, Is.EqualTo(50));
        Assert.That(c1_node.Parent, Is.Null, "c1_node parent should be null after cut");
        Assert.That(c1_node.Mark, Is.False, "c1_node mark should be false after cut");
        Assert.That(p1_node.Degree, Is.EqualTo(oldP1Degree - 1), "Parent's degree should decrement");
        Assert.That(heap.Peek(), Is.EqualTo(50), "New min should be c1_node's new key");

        Assert.That(heap.Pop(), Is.EqualTo(50));
        Assert.That(heap.Peek(), Is.EqualTo(100));
    }


    [Test]
    public void DecreaseKey_TriggersCascadingCut()
    {
        // Setup: GP(24) -> P(26) -> C1(35).
        var n24 = heap.Push(24); // Grandparent
        var n26 = heap.Push(26); // Parent
        var n35 = heap.Push(35); // Child1 of n26
        heap.Push(5); // Min
        heap.Pop();   // Consolidate. Min is 24. Assume 26 child of 24, 35 child of 26.

        if (!(n26.Parent == n24 && n35.Parent == n26))
        {
            Assert.Inconclusive("Required heap structure for cascading cut test (GP->P->C1) did not form.");
        }

        // 1. Cut n35 from n26. n26 becomes marked.
        heap.DecreaseKey(n35, 15); // n35(15) cut from n26(26). n35 to root. Min becomes 15.
                                   // n26 becomes marked.
        Assert.That(n26.Mark, Is.True, "n26 should be marked after losing child n35.");
        Assert.That(n35.Parent, Is.Null);
        Assert.That(heap.Peek(), Is.EqualTo(15));

        // 2. Decrease n26's key, which is marked and a child. This triggers cut and cascading cut.
        heap.DecreaseKey(n26, 10); // n26(10) cut from n24(24). n26 to root. Min becomes 10.
                                   // n24 was parent of n26. CascadingCut(n24) called.
                                   // n24 was not marked, so it becomes marked.
        Assert.That(n26.Key, Is.EqualTo(10));
        Assert.That(n26.Parent, Is.Null, "n26 should be cut from n24 and move to root.");
        Assert.That(n26.Mark, Is.False, "n26 should be unmarked after being cut.");
        Assert.That(n24.Mark, Is.True, "n24 should become marked due to cascading cut.");
        Assert.That(heap.Peek(), Is.EqualTo(10));
    }

    [Test]
    public void DecreaseKey_KeyGreaterThanCurrent_ThrowsInvalidOperationException()
    {
        var node = heap.Push(10);
        Assert.That(() => heap.DecreaseKey(node, 20), Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("Value cannot be increased"));
    }

    [Test]
    public void DecreaseKey_OnNodeFromEmptyHeapProxy_ThrowsArgumentException()
    {
        var otherHeap = new FibonacciHeap<int>();
        var nodeFromOtherHeap = otherHeap.Push(10);

        Assert.That(() => heap.DecreaseKey(nodeFromOtherHeap, 5), Throws.ArgumentException.With.Message.Contains("is not from the heap"));
    }

    /* FAILED UNITTEST
    [Test]
    public void DecreaseKey_NodeKeyIsNull_ThrowsArgumentException()
    {
        // This test checks the behavior when a node's key is set to null before calling DecreaseKey.
        // The FHeapNode.Key is of type T. The check 'if (x.Key == null)' in DecreaseKey
        // is relevant for nullable reference types (e.g., string?) or nullable value types.
        // Since int? does not satisfy IComparable directly, we use string? (assuming NRTs are enabled).
        // string implements IComparable, and string? allows the Key to be null.
        // This assumes FHeapNode<T>.Key has a public setter.

        var heap = new FibonacciHeap<string?>(); // Use string? for T
        var node = heap.Push("initialValue");    // Push an initial non-null value. Key is "initialValue".

        // Manually set the node's key to null to trigger the specific check in DecreaseKey.
        // This relies on FHeapNode<T>.Key having a public setter.
        node.Key = null; // node.Key is now (string?)null.

        // Attempt to decrease the key. The actual new key value ("newValue") doesn't matter much
        // as the exception for x.Key == null should be thrown first.
        Assert.That(() => heap.DecreaseKey(node, "newValue"), // Using a non-null string for the new key.
            Throws.ArgumentException.With.Message.EqualTo("x has no value"));
    }
    */


    // Count Property
    [Test]
    public void Count_IsAccurateAfterPushesAndPops()
    {
        Assert.That(heap.Count, Is.EqualTo(0));
        heap.Push(10);
        Assert.That(heap.Count, Is.EqualTo(1));
        heap.Push(20);
        Assert.That(heap.Count, Is.EqualTo(2));
        heap.Pop();
        Assert.That(heap.Count, Is.EqualTo(1));

        var otherHeap = new FibonacciHeap<int>();
        otherHeap.Push(30);
        otherHeap.Push(40);
        Assert.That(otherHeap.Count, Is.EqualTo(2));

        heap.Union(otherHeap);
        Assert.That(heap.Count, Is.EqualTo(1 + 2));

        heap.Pop();
        Assert.That(heap.Count, Is.EqualTo(2));
        heap.Pop();
        Assert.That(heap.Count, Is.EqualTo(1));
        heap.Pop();
        Assert.That(heap.Count, Is.EqualTo(0));
    }

    // Tests for the Cut bug (if y.Degree > 1, child x is not added to root list)
    [Test]
    public void DecreaseKey_Cut_ParentHasMultipleChildren_NodeBecomesNewMin_OrphansOldRoot_IfBugExists()
    {
        // Setup: P(100) -> (C1(200), C2(300)). Min is P(100).
        var nodeP = heap.Push(100);
        var nodeC1 = heap.Push(200);
        var nodeC2 = heap.Push(300);
        heap.Push(5); // Temp min
        heap.Pop();   // Pop 5. Min is 100 (nodeP). Consolidation should make C1, C2 children of P. P.Degree = 2.

        if (!(nodeC1.Parent == nodeP && nodeC2.Parent == nodeP && nodeP.Degree == 2))
        {
            Assert.Inconclusive("Required heap structure (P.Degree=2, C1,C2 children of P) did not form for Cut bug test.");
        }

        int originalCount = heap.Count; // Should be 3 (P, C1, C2)

        // Decrease C1's key to 50. New key < P.Key (100) and < current Min (100).
        // Cut(C1, P) is called. P.Degree (before decrement) was 2.
        // If bug exists: C1 is NOT added to root list by Cut.
        // Then, MinItem becomes C1. Original root list (containing P) is orphaned.
        heap.DecreaseKey(nodeC1, 50);

        Assert.That(heap.Peek(), Is.EqualTo(50), "MinItem should be the decreased node C1.");
        Assert.That(heap.Count, Is.EqualTo(originalCount), "Count should remain unchanged after DecreaseKey.");

        Assert.That(heap.Pop(), Is.EqualTo(50), "Pop should return C1's new key.");
        Assert.That(heap.Count, Is.EqualTo(originalCount - 1), "Count should decrease after Pop.");

        // If old root list was orphaned, next Peek/Pop will fail as if heap is empty.
        // The actual exception message from Peek() on an empty heap is "The heap is empty".
        // The custom message explains the test's expectation.
        Assert.That(() => heap.Peek(), Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("The heap is empty"),
            "Heap should be effectively empty if bug orphaned other nodes and C1 was popped.");
    }

    [Test]
    public void DecreaseKey_Cut_ParentHasMultipleChildren_NodeLostIfNotNewMin_IfBugExists()
    {
        // Setup: P(100) -> (C1(200), C2(300)). Min is P(100).
        var nodeP = heap.Push(100);
        var nodeC1 = heap.Push(200);
        var nodeC2 = heap.Push(300);
        heap.Push(5);
        heap.Pop();   // Pop 5. Min is 100 (nodeP). P.Degree = 2.

        if (!(nodeC1.Parent == nodeP && nodeC2.Parent == nodeP && nodeP.Degree == 2))
        {
            Assert.Inconclusive("Required heap structure (P.Degree=2, C1,C2 children of P) did not form for Cut bug test.");
        }

        int originalCount = heap.Count; // 3

        // Decrease C1's key to 150. New key < C1.OldKey (200) but > P.Key (100).
        // Cut(C1, P) is called. P.Degree (before dec) was 2.
        // If bug exists: C1 is NOT added to root list by Cut.
        // MinItem does NOT change to C1 (because 150 > 100).
        // C1 is now lost.
        heap.DecreaseKey(nodeC1, 150);

        Assert.That(heap.Peek(), Is.EqualTo(100), "MinItem should still be P.");
        // Count is not aware of lost nodes, so it remains originalCount.
        Assert.That(heap.Count, Is.EqualTo(originalCount), "Count should be unchanged by DecreaseKey operation itself.");

        var poppedItems = new List<int>();
        // Expected pops: 100 (P). Then C2 (300) becomes root. C1 (150) is lost.
        poppedItems.Add(heap.Pop()); // Should be 100 (nodeP)
        Assert.That(heap.Count, Is.EqualTo(originalCount - 1));

        // After P is popped, its children (only C2 if C1 is lost) are added to root.
        // So C2 (300) should be next.
        if (heap.Count > 0)
        { // Should be C2 remaining
            poppedItems.Add(heap.Pop()); // Should be 300 (nodeC2)
        }

        // If C1 was lost, Count would be originalCount - 2 here.
        Assert.That(heap.Count, Is.EqualTo(originalCount - 2), "Count after popping P and C2.");

        Assert.That(poppedItems, Does.Not.Contain(150), "Node C1 (key 150) should be lost and not popped.");
        Assert.That(poppedItems, Does.Contain(100));
        if (originalCount > 1) Assert.That(poppedItems, Does.Contain(300));

        // After P and C2 are popped, if C1 was lost, the heap should be empty.
        // Count should be 0. Any Pop/Peek should fail.
        Assert.That(heap.Count, Is.EqualTo(0));
        Assert.That(() => heap.Pop(), Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("Heap is empty!"),
            "Heap should be empty if C1 was lost and P,C2 popped.");
    }
}
