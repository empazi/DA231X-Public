using DataStructures.ScapegoatTree;
using NUnit.Framework;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataStructures.Tests.ScapegoatTree;

[TestFixture]
public class ScapegoatTreeTests
{
    private List<TKey> GetInOrderTraversal<TKey>(Node<TKey>? node) where TKey : IComparable
    {
        var result = new List<TKey>();
        if (node == null) return result;
        result.AddRange(GetInOrderTraversal(node.Left));
        result.Add(node.Key);
        result.AddRange(GetInOrderTraversal(node.Right));
        return result;
    }

    [Test]
    public void DefaultConstructor_ShouldInitializeCorrectly()
    {
        var tree = new ScapegoatTree<int>();
        tree.Alpha.Should().Be(0.5);
        tree.Size.Should().Be(0);
        tree.MaxSize.Should().Be(0);
        tree.Root.Should().BeNull();
    }

    [Test]
    public void AlphaConstructor_ShouldInitializeCorrectly()
    {
        var tree = new ScapegoatTree<int>(0.75);
        tree.Alpha.Should().Be(0.75);
        tree.Size.Should().Be(0);
        tree.MaxSize.Should().Be(0);
        tree.Root.Should().BeNull();
    }

    [Test]
    public void KeyConstructor_ShouldInitializeCorrectly()
    {
        var tree = new ScapegoatTree<int>(10, 0.6);
        tree.Alpha.Should().Be(0.6);
        tree.Size.Should().Be(1);
        tree.MaxSize.Should().Be(1);
        tree.Root.Should().NotBeNull();
        tree.Root!.Key.Should().Be(10);
    }

    [Test]
    public void NodeConstructor_ShouldInitializeCorrectly()
    {
        var node = new Node<int>(20);
        var tree = new ScapegoatTree<int>(node, 0.7);
        tree.Alpha.Should().Be(0.7);
        tree.Size.Should().Be(1); // Node.GetSize() for a single node is 1
        tree.MaxSize.Should().Be(1);
        tree.Root.Should().Be(node);
    }

    [TestCase(0.49)]
    [TestCase(1.01)]
    public void Constructor_WithInvalidAlpha_ShouldThrowArgumentException(double invalidAlpha)
    {
        Action act1 = () => new ScapegoatTree<int>(invalidAlpha);
        act1.Should().Throw<ArgumentException>().WithMessage("The alpha parameter's value should be in 0.5..1.0 range.*");

        Action act2 = () => new ScapegoatTree<int>(10, invalidAlpha);
        act2.Should().Throw<ArgumentException>().WithMessage("The alpha parameter's value should be in 0.5..1.0 range.*");

        var node = new Node<int>(5);
        Action act3 = () => new ScapegoatTree<int>(node, invalidAlpha);
        act3.Should().Throw<ArgumentException>().WithMessage("The alpha parameter's value should be in 0.5..1.0 range.*");
    }

    [Test]
    public void Insert_EmptyTree_ShouldSetRootAndIncrementSize()
    {
        var tree = new ScapegoatTree<int>();
        tree.Insert(10).Should().BeTrue();
        tree.Size.Should().Be(1);
        tree.MaxSize.Should().Be(1);
        tree.Root.Should().NotBeNull();
        tree.Root!.Key.Should().Be(10);
    }

    [Test]
    public void Insert_ExistingKey_ShouldReturnFalseAndNotChangeSize()
    {
        var tree = new ScapegoatTree<int>(10);
        tree.Insert(10).Should().BeFalse();
        tree.Size.Should().Be(1); // Size was 1, remains 1
    }

    [Test]
    public void Insert_SmallerAndLargerKeys_ShouldMaintainOrderAndIncrementSize()
    {
        var tree = new ScapegoatTree<int>();
        tree.Insert(10);
        tree.Insert(5);
        tree.Insert(15);

        tree.Size.Should().Be(3);
        tree.Root!.Key.Should().Be(10);
        tree.Root!.Left!.Key.Should().Be(5);
        tree.Root!.Right!.Key.Should().Be(15);
        GetInOrderTraversal(tree.Root).Should().Equal(5, 10, 15);
    }

    [Test]
    public void Insert_TriggerRebalance_ShouldRebalanceTreeAndRaiseEvent()
    {
        var tree = new ScapegoatTree<int>(0.5); // Alpha = 0.5 for easier trigger
        bool eventRaised = false;
        tree.TreeIsUnbalanced += (sender, args) => eventRaised = true;

        tree.Insert(10); // Size 1
        tree.Insert(5);  // Size 2. path=[10]. path.Count=1. Root.GetAlphaHeight(0.5) for Size=2 is log2(2)=1. 1>1 false.
        tree.Insert(3);  // Size 3. path=[10,5]. path.Count=2. Root.GetAlphaHeight(0.5) for Size=3 is floor(log2(3))=1. 2>1 true. Rebalance.

        eventRaised.Should().BeTrue();
        tree.Size.Should().Be(3);
        // After rebalance with 10,5,3 (scapegoat 10), new root should be 5.
        // Flattened: [3,5,10]. Rebuilt: Root=5, Left=3, Right=10.
        tree.Root!.Key.Should().Be(5);
        tree.Root!.Left!.Key.Should().Be(3);
        tree.Root!.Right!.Key.Should().Be(10);
        tree.IsAlphaWeightBalanced().Should().BeTrue();
    }

    [Test]
    public void Contains_Search_EmptyTree_ShouldReturnFalseOrNull()
    {
        var tree = new ScapegoatTree<int>();
        tree.Contains(5).Should().BeFalse();
        tree.Search(5).Should().BeNull();
    }

    [Test]
    public void Contains_Search_ExistingKey_ShouldReturnTrueOrNode()
    {
        var tree = new ScapegoatTree<int>(10);
        tree.Insert(5);
        tree.Insert(15);

        tree.Contains(5).Should().BeTrue();
        var node = tree.Search(5);
        node.Should().NotBeNull();
        node!.Key.Should().Be(5);
    }

    [Test]
    public void Contains_Search_NonExistingKey_ShouldReturnFalseOrNull()
    {
        var tree = new ScapegoatTree<int>(10);
        tree.Insert(5);
        tree.Insert(15);

        tree.Contains(100).Should().BeFalse();
        tree.Search(100).Should().BeNull();
    }

    [Test]
    public void Delete_EmptyTree_ShouldReturnFalse()
    {
        var tree = new ScapegoatTree<int>();
        tree.Delete(5).Should().BeFalse();
    }

    [Test]
    public void Delete_NonExistingKey_ShouldReturnFalse()
    {
        var tree = new ScapegoatTree<int>(10);
        tree.Delete(5).Should().BeFalse();
        tree.Size.Should().Be(1);
    }

    [Test]
    public void Delete_LeafNode_ShouldRemoveNodeAndDecrementSize()
    {
        var tree = new ScapegoatTree<int>();
        tree.Insert(10);
        tree.Insert(5);
        tree.Delete(5).Should().BeTrue();
        tree.Size.Should().Be(1);
        tree.Root!.Key.Should().Be(10);
        tree.Root!.Left.Should().BeNull();
        GetInOrderTraversal(tree.Root).Should().Equal(10);
    }

    [Test]
    public void Delete_NodeWithOneLeftChild_ShouldPromoteChild()
    {
        var tree = new ScapegoatTree<int>();
        tree.Insert(10);
        tree.Insert(5);
        tree.Insert(3); // 10 -> 5 -> 3
        tree.Delete(5).Should().BeTrue(); // Delete 5
        tree.Size.Should().Be(2);
        // After inserts (10,5,3), rebalance makes 5 the root: (3 <- 5 -> 10)
        // Deleting 5 (root, two children), predecessor 3 replaces it. Root becomes 3.
        tree.Root!.Key.Should().Be(3);
        tree.Root!.Left.Should().BeNull();
        tree.Root!.Right!.Key.Should().Be(10);
        GetInOrderTraversal(tree.Root).Should().Equal(3, 10); // In-order: 3, 10
    }

    [Test]
    public void Delete_NodeWithOneRightChild_ShouldPromoteChild()
    {
        var tree = new ScapegoatTree<int>();
        tree.Insert(10);
        tree.Insert(5);
        tree.Insert(7); // 10 -> 5 -> 7 (as right child of 5)
        tree.Delete(5).Should().BeTrue(); // Delete 5
        tree.Size.Should().Be(2);
        // After inserts (10,5,7), rebalance makes 7 the root: (5 <- 7 -> 10)
        // Deleting 5 (leaf child of root 7). Root remains 7.
        tree.Root!.Key.Should().Be(7);
        tree.Root!.Left.Should().BeNull();
        tree.Root!.Right!.Key.Should().Be(10);
        GetInOrderTraversal(tree.Root).Should().Equal(7, 10); // In-order: 7, 10
    }

    [Test]
    public void Delete_NodeWithTwoChildren_ShouldReplaceWithPredecessor()
    {
        var tree = new ScapegoatTree<int>(); // Default alpha 0.5
        // Build a tree:
        //      10
        //     /  \
        //    5    15
        //   / \   / \
        //  3   7 12 17
        var keys = new[] { 10, 5, 15, 3, 7, 12, 17 };
        foreach (var key in keys) tree.Insert(key);

        tree.Size.Should().Be(7);
        tree.Delete(10).Should().BeTrue(); // Delete root (10)
        tree.Size.Should().Be(6);

        // Predecessor of 10 is 7. So 7 becomes the new root.
        // Original structure of 5's subtree: 5 -> (3, 7). After 7 is moved, 5's right child should be null.
        // The new node for 7 takes children from original 10 (which were 5 and 15).
        // The recursive delete of 7 from its original position (child of 5) should handle 5's children.
        // Expected inorder: 3, 5, 7, 12, 15, 17. Root should be 7.
        GetInOrderTraversal(tree.Root).Should().Equal(3, 5, 7, 12, 15, 17);
        tree.Root!.Key.Should().Be(7);
        // Check structure around new root 7
        tree.Root.Left!.Key.Should().Be(5);
        tree.Root.Right!.Key.Should().Be(15);
        // Check structure around 5 (original parent of 7)
        tree.Root.Left.Left!.Key.Should().Be(3);
        tree.Root.Left.Right.Should().BeNull(); // 7 was here
    }

    [Test]
    public void Delete_RootNode_SingleNodeTree_ShouldUpdateRootToNull()
    {
        var tree = new ScapegoatTree<int>(10);
        tree.Delete(10).Should().BeTrue();
        tree.Size.Should().Be(0);
        tree.Root.Should().BeNull();
    }

    [Test]
    public void Delete_TriggerRebalance_ShouldRebalanceTreeAndRaiseEvent()
    {
        var tree = new ScapegoatTree<int>(0.75); // Alpha = 0.75
        bool eventRaised = false;
        tree.TreeIsUnbalanced += (sender, args) => eventRaised = true;

        // Insert 8 nodes
        for (int i = 1; i <= 8; i++) tree.Insert(i);
        tree.Size.Should().Be(8);
        tree.MaxSize.Should().Be(8); // MaxSize is 8

        // Alpha * MaxSize = 0.75 * 8 = 6
        // Deleting nodes until Size < 6 should trigger rebalance.
        tree.Delete(8); // Size = 7. MaxSize = 8. 7 not < 6.
        eventRaised.Should().BeFalse();
        tree.Delete(7); // Size = 6. MaxSize = 8. 6 not < 6.
        eventRaised.Should().BeFalse();
        tree.Delete(6); // Size = 5. MaxSize = 8. 5 < 6. Rebalance!

        eventRaised.Should().BeTrue();
        tree.Size.Should().Be(5);
        tree.MaxSize.Should().Be(5); // MaxSize is reset to Size after this type of rebalance
        tree.IsAlphaWeightBalanced().Should().BeTrue();
        GetInOrderTraversal(tree.Root).Should().Equal(1, 2, 3, 4, 5);
    }

    [Test]
    public void IsAlphaWeightBalanced_EmptyTree_ShouldBeTrue()
    {
        var tree = new ScapegoatTree<int>();
        tree.IsAlphaWeightBalanced().Should().BeTrue();
    }

    [Test]
    public void IsAlphaWeightBalanced_SingleNodeTree_ShouldBeTrue()
    {
        var tree = new ScapegoatTree<int>(10);
        tree.IsAlphaWeightBalanced().Should().BeTrue();
    }

    [Test]
    public void IsAlphaWeightBalanced_KnownBalancedTree_ShouldBeTrue()
    {
        var tree = new ScapegoatTree<int>();
        tree.Insert(10);
        tree.Insert(5);
        tree.Insert(15); // Perfectly balanced
        tree.IsAlphaWeightBalanced().Should().BeTrue();
    }

    [Test]
    public void Clear_EmptyTree_ShouldDoNothing()
    {
        var tree = new ScapegoatTree<int>();
        tree.Clear();
        tree.Size.Should().Be(0);
        tree.MaxSize.Should().Be(0);
        tree.Root.Should().BeNull();
    }

    [Test]
    public void Clear_PopulatedTree_ShouldResetTree()
    {
        var tree = new ScapegoatTree<int>();
        tree.Insert(10);
        tree.Insert(5);
        tree.Clear();
        tree.Size.Should().Be(0);
        tree.MaxSize.Should().Be(0);
        tree.Root.Should().BeNull();
    }

    [Test]
    public void Tune_ValidAlpha_ShouldUpdateAlpha()
    {
        var tree = new ScapegoatTree<int>();
        tree.Tune(0.8);
        tree.Alpha.Should().Be(0.8);
    }

    [Test]
    public void Tune_InvalidAlpha_ShouldThrowArgumentException()
    {
        var tree = new ScapegoatTree<int>();
        Action act = () => tree.Tune(0.4);
        act.Should().Throw<ArgumentException>().WithMessage("The alpha parameter's value should be in 0.5..1.0 range.*");
    }

    [Test]
    public void FindScapegoatInPath_EmptyPath_ShouldThrowArgumentException()
    {
        var tree = new ScapegoatTree<int>();
        var path = new Stack<Node<int>>();
        Action act = () => tree.FindScapegoatInPath(path);
        act.Should().Throw<ArgumentException>().WithMessage("The path collection should not be empty.*");
    }

    [Test]
    public void FindScapegoatInPath_ScapegoatIsRoot_ShouldReturnRootAndNullParent()
    {
        // Tree: 1 (Root). Path: [1]. alpha=0.5.
        // Node 1 (size 1). GetAlphaHeight(0.5) = floor(log2(1)) = 0.
        // Pop 1. depth=1. 1 > 0. Scapegoat=1, Parent=null.
        var tree = new ScapegoatTree<int>(0.5);
        var node1 = new Node<int>(1);
        // Manually construct path for testing FindScapegoatInPath directly
        var path = new Stack<Node<int>>();
        path.Push(node1); // This node represents the root in this simple path

        var (parent, scapegoat) = tree.FindScapegoatInPath(path);
        scapegoat.Should().Be(node1);
        parent.Should().BeNull();
    }

    [Test]
    public void FindScapegoatInPath_ScapegoatInMiddle_ShouldReturnNodeAndParent()
    {
        // Tree: 3 -> 2 -> 1. Path: [3,2,1] (1 is deepest on path, 3 is highest). alpha=0.5.
        // Node 1 (size 1). GetAlphaHeight(0.5) = 0.
        // Node 2 (size 2, if 1 is its child). GetAlphaHeight(0.5) = 1.
        // Node 3 (size 3, if 2 is its child). GetAlphaHeight(0.5) = 1.
        // Stack for FindScapegoatInPath: [1 (top), 2, 3]
        // Pop 1. depth=1. 1.GetAlphaHeight()=0. 1 > 0. Scapegoat=1, Parent=2.
        var tree = new ScapegoatTree<int>(0.5);
        var node1 = new Node<int>(1);
        var node2 = new Node<int>(2) { Left = node1 }; // node2 is parent of node1
        var node3 = new Node<int>(3) { Left = node2 }; // node3 is parent of node2

        var path = new Stack<Node<int>>();
        path.Push(node3);
        path.Push(node2);
        path.Push(node1);

        var (parent, scapegoat) = tree.FindScapegoatInPath(path);
        scapegoat.Should().Be(node1);
        parent.Should().Be(node2);
    }

    [Test]
    public void FindScapegoatInPath_DeeperScapegoat_ShouldReturnCorrectNodeAndParent()
    {
        // Example from thought process: Insert 10, 5, 3. Alpha=0.5.
        // Path for Insert(3) is [10, 5]. Stack for FindScapegoatInPath: [5 (top), 10].
        // Node 5 (key 5, actual children 3). Size of subtree at 5 is 2 (5 and 3). GetAlphaHeight(0.5) for size 2 is 1.
        // Node 10 (key 10, actual children 5,3). Size of subtree at 10 is 3. GetAlphaHeight(0.5) for size 3 is 1.
        // 1. Pop 5. depth=1. 1 > 5.GetAlphaHeight() (which is 1) is false.
        // 2. Pop 10. depth=2. 2 > 10.GetAlphaHeight() (which is 1) is true. Scapegoat=10, Parent=null.
        var tree = new ScapegoatTree<int>(0.5);

        var node3 = new Node<int>(3);
        var node5 = new Node<int>(5) { Left = node3 };
        var node10 = new Node<int>(10) { Left = node5 };

        var path = new Stack<Node<int>>();
        path.Push(node10);
        path.Push(node5);

        var (parent, scapegoat) = tree.FindScapegoatInPath(path);
        scapegoat.Should().Be(node10);
        parent.Should().BeNull();
    }

    [Test]
    public void FindScapegoatInPath_PathBalancedAccordingToAlphaHeight_ShouldThrowInvalidOperationException()
    {
        // Need d <= next.GetAlphaHeight(Alpha) for all d and next in path. alpha=0.5
        // Path: [N2, N1]. (N1 is child of N2). Stack: [N1 (top), N2].
        // 1. Pop N1. d=1. Need 1 <= N1.GetAlphaHeight(). So N1.size >= 2.
        // 2. Pop N2. d=2. Need 2 <= N2.GetAlphaHeight(). So N2.size >= 4.
        var tree = new ScapegoatTree<int>(0.5);

        var leaf1_1 = new Node<int>(0);
        var node1 = new Node<int>(1) { Left = leaf1_1 }; // N1.size = 2

        var leaf2_1 = new Node<int>(3);
        var node2 = new Node<int>(2) { Left = node1, Right = leaf2_1 }; // N2.size = 1 (N2) + 2 (N1) + 1 (leaf2_1) = 4

        var path = new Stack<Node<int>>();
        path.Push(node2);
        path.Push(node1);

        Action act = () => tree.FindScapegoatInPath(path);
        act.Should().Throw<InvalidOperationException>().WithMessage("Scapegoat node wasn't found. The tree should be unbalanced.");
    }

    [Test]
    public void MaxSize_ShouldBeUpdatedCorrectly_OnInsertAndDelete()
    {
        var tree = new ScapegoatTree<int>();
        tree.Insert(10); // Size=1, MaxSize=1
        tree.Insert(20); // Size=2, MaxSize=2
        tree.Insert(5);  // Size=3, MaxSize=3
        tree.MaxSize.Should().Be(3);

        tree.Delete(20); // Size=2, MaxSize=3 (no rebalance that resets MaxSize)
        tree.MaxSize.Should().Be(3);

        // Trigger rebalance on delete that resets MaxSize
        // Alpha = 0.75. MaxSize = 3. Size = 2.
        // Need Size < Alpha * MaxSize for rebalance. 2 < 0.75 * 3 (2.25) is true.
        tree.Tune(0.75); // Change alpha
        bool eventRaised = false;
        tree.TreeIsUnbalanced += (s, e) => eventRaised = true;

        tree.Delete(5); // Size=1. MaxSize was 3. 1 < 0.75 * 3 (2.25) is true. Rebalance.
                        // MaxSize should be reset to current Size (1).
        eventRaised.Should().BeTrue();
        tree.Size.Should().Be(1);
        tree.MaxSize.Should().Be(1);
    }
}
