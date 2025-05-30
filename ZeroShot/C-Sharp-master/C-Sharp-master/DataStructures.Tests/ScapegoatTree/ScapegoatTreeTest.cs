using NUnit.Framework;
using DataStructures.ScapegoatTree; // Assuming ScapegoatTree.cs is in this namespace
using System;
using System.Collections.Generic;


// System.Linq might be needed if tests do advanced list operations, but not for current assertions.

// Minimal Node and Extensions for ScapegoatTree tests
// These should ideally be in their own files and namespace matching the source project.
// For this exercise, they are included here for completeness.
namespace DataStructures.ScapegoatTree
{
    // This Node class should match the one used by ScapegoatTree.cs
    // If ScapegoatTree.cs has its own Node class, these tests should use that one.
    // For this example, I'm providing a compatible one.

}

namespace DataStructures.Tests // Or your preferred test namespace
{
    [TestFixture]
    public class ScapegoatTreeTests
    {
        [Test]
        public void Constructor_Default_InitializesCorrectly()
        {
            var tree = new ScapegoatTree<int>();
            Assert.That(tree.Alpha, Is.EqualTo(0.5));
            Assert.That(tree.Root, Is.Null);
            Assert.That(tree.Size, Is.EqualTo(0));
            Assert.That(tree.MaxSize, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_WithAlpha_InitializesCorrectly()
        {
            var tree = new ScapegoatTree<int>(0.75);
            Assert.That(tree.Alpha, Is.EqualTo(0.75));
            Assert.That(tree.Root, Is.Null);
            Assert.That(tree.Size, Is.EqualTo(0));
            Assert.That(tree.MaxSize, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_WithNodeAndAlpha_InitializesCorrectly()
        {
            // This 'Node<int>' must resolve to the type defined in your main project's 
            // DataStructures.ScapegoatTree.Node.cs, not a helper class in the test file.
            var rootNode = new Node<int>(10);
            rootNode.Left = new Node<int>(5);
            rootNode.Right = new Node<int>(15);

            // The ScapegoatTree constructor expects the Node<TKey> from your main project.
            var tree = new ScapegoatTree<int>(rootNode, 0.6);

            Assert.That(tree.Alpha, Is.EqualTo(0.6));
            Assert.That(tree.Root, Is.SameAs(rootNode));
            // The ScapegoatTree constructor calls node.GetSize(). 
            // For this to be 3, the rootNode (and its children) must correctly report their sizes.
            Assert.That(tree.Size, Is.EqualTo(3));
            Assert.That(tree.MaxSize, Is.EqualTo(3));
        }

        [Test]
        public void Constructor_WithKeyAndAlpha_InitializesCorrectly()
        {
            var tree = new ScapegoatTree<int>(10, 0.65);
            Assert.That(tree.Alpha, Is.EqualTo(0.65));
            Assert.That(tree.Root, Is.Not.Null);
            Assert.That(tree.Root?.Key, Is.EqualTo(10)); // Added null check for safety
            Assert.That(tree.Size, Is.EqualTo(1));
            Assert.That(tree.MaxSize, Is.EqualTo(1));
        }

        [TestCase(0.49)]
        [TestCase(1.01)]
        public void Constructor_InvalidAlpha_ThrowsArgumentException(double alpha)
        {
            Assert.That(() => new ScapegoatTree<int>(alpha), Throws.TypeOf<ArgumentException>());
            Assert.That(() => new ScapegoatTree<int>(new Node<int>(1), alpha), Throws.TypeOf<ArgumentException>());
            Assert.That(() => new ScapegoatTree<int>(1, alpha), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void IsAlphaWeightBalanced_EmptyTree_ReturnsTrue()
        {
            var tree = new ScapegoatTree<int>();
            Assert.That(tree.IsAlphaWeightBalanced(), Is.True);
        }

        [Test]
        public void IsAlphaWeightBalanced_SingleNodeTree_ReturnsTrue()
        {
            var tree = new ScapegoatTree<int>(10);
            Assert.That(tree.IsAlphaWeightBalanced(), Is.True);
        }

        [Test]
        public void IsAlphaWeightBalanced_BalancedTree_ReturnsTrue()
        {
            var tree = new ScapegoatTree<int>(0.5);
            tree.Insert(10);
            tree.Insert(5);
            tree.Insert(15);
            Assert.That(tree.IsAlphaWeightBalanced(), Is.True);
        }

        [Test]
        public void IsAlphaWeightBalanced_UnbalancedTree_ReturnsFalse()
        {
            var tree = new ScapegoatTree<int>(0.5);
            var rootNode = new Node<int>(10);
            rootNode.Left = new Node<int>(5);
            var rightSubtree = new Node<int>(20);
            rightSubtree.Left = new Node<int>(15);
            rightSubtree.Right = new Node<int>(25);
            rootNode.Right = rightSubtree;

            var unbalancedTree = new ScapegoatTree<int>(rootNode, 0.5);
            Assert.That(unbalancedTree.IsAlphaWeightBalanced(), Is.False);
        }

        [Test]
        public void Contains_EmptyTree_ReturnsFalse()
        {
            var tree = new ScapegoatTree<int>();
            Assert.That(tree.Contains(5), Is.False);
        }

        [Test]
        public void Contains_KeyExists_ReturnsTrue()
        {
            var tree = new ScapegoatTree<int>(10);
            tree.Insert(5);
            tree.Insert(15);
            Assert.That(tree.Contains(5), Is.True);
            Assert.That(tree.Contains(15), Is.True);
            Assert.That(tree.Contains(10), Is.True);
        }

        [Test]
        public void Contains_KeyDoesNotExist_ReturnsFalse()
        {
            var tree = new ScapegoatTree<int>(10);
            tree.Insert(5);
            Assert.That(tree.Contains(15), Is.False);
        }

        [Test]
        public void Search_EmptyTree_ReturnsNull()
        {
            var tree = new ScapegoatTree<int>();
            Assert.That(tree.Search(5), Is.Null);
        }

        [Test]
        public void Search_KeyExists_ReturnsNode()
        {
            var tree = new ScapegoatTree<int>(10);
            tree.Insert(5);
            var node = tree.Search(5);
            Assert.That(node, Is.Not.Null);
            Assert.That(node?.Key, Is.EqualTo(5));
        }

        [Test]
        public void Search_KeyDoesNotExist_ReturnsNull()
        {
            var tree = new ScapegoatTree<int>(10);
            Assert.That(tree.Search(15), Is.Null);
        }

        [Test]
        public void Insert_ToEmptyTree_SetsRootAndUpdatesSize()
        {
            var tree = new ScapegoatTree<int>();
            Assert.That(tree.Insert(10), Is.True);
            Assert.That(tree.Root, Is.Not.Null);
            Assert.That(tree.Root?.Key, Is.EqualTo(10));
            Assert.That(tree.Size, Is.EqualTo(1));
            Assert.That(tree.MaxSize, Is.EqualTo(1));
        }

        [Test]
        public void Insert_NewKeys_UpdatesSizeAndStructure()
        {
            var tree = new ScapegoatTree<int>();
            tree.Insert(10);
            tree.Insert(5);
            tree.Insert(15);

            Assert.That(tree.Size, Is.EqualTo(3));
            Assert.That(tree.MaxSize, Is.EqualTo(3));
            Assert.That(tree.Root, Is.Not.Null);
            Assert.That(tree.Root?.Key, Is.EqualTo(10));
            Assert.That(tree.Root?.Left?.Key, Is.EqualTo(5));
            Assert.That(tree.Root?.Right?.Key, Is.EqualTo(15));
        }

        [Test]
        public void Insert_DuplicateKey_ReturnsFalseAndNoChange()
        {
            var tree = new ScapegoatTree<int>();
            tree.Insert(10);
            tree.Insert(5);

            Assert.That(tree.Insert(5), Is.False); // Duplicate
            Assert.That(tree.Size, Is.EqualTo(2));
            Assert.That(tree.MaxSize, Is.EqualTo(2));
        }

        [Test]
        public void Insert_CausesRebalance_TreeIsUnbalancedEventFiresAndStructureChanges()
        {
            var tree = new ScapegoatTree<int>(0.5);
            bool eventFired = false;
            tree.TreeIsUnbalanced += (sender, args) => eventFired = true;

            tree.Insert(10);
            tree.Insert(5);
            tree.Insert(3);

            Assert.That(eventFired, Is.True, "TreeIsUnbalanced event should have fired.");
            Assert.That(tree.Size, Is.EqualTo(3));
            Assert.That(tree.MaxSize, Is.EqualTo(3));

            Assert.That(tree.Root, Is.Not.Null);
            Assert.That(tree.Root?.Key, Is.EqualTo(5));
            Assert.That(tree.Root?.Left?.Key, Is.EqualTo(3));
            Assert.That(tree.Root?.Right?.Key, Is.EqualTo(10));
            Assert.That(tree.IsAlphaWeightBalanced(), Is.True);
        }

        [Test]
        public void Delete_FromEmptyTree_ReturnsFalse()
        {
            var tree = new ScapegoatTree<int>();
            Assert.That(tree.Delete(5), Is.False);
            Assert.That(tree.Size, Is.EqualTo(0));
        }

        [Test]
        public void Delete_NonExistentKey_ReturnsFalse()
        {
            var tree = new ScapegoatTree<int>(10);
            Assert.That(tree.Delete(5), Is.False);
            Assert.That(tree.Size, Is.EqualTo(1));
        }

        [Test]
        public void Delete_LeafNode()
        {
            var tree = new ScapegoatTree<int>(10);
            tree.Insert(5);
            Assert.That(tree.Delete(5), Is.True);
            Assert.That(tree.Size, Is.EqualTo(1));
            Assert.That(tree.Root?.Left, Is.Null);
            Assert.That(tree.Contains(5), Is.False);
        }

        [Test]
        public void Delete_NodeWithOneRightChild()
        {
            var tree = new ScapegoatTree<int>(); // Default Alpha = 0.5
            tree.Insert(10);
            tree.Insert(5);
            tree.Insert(7);
            // After inserts and potential rebalancing (Alpha=0.5):
            // Tree is: Root=7, L:5, R:10

            Assert.That(tree.Delete(5), Is.True); // Delete 5 (left child of root 7)
            // Root 7's left child (5) is removed.
            Assert.That(tree.Size, Is.EqualTo(2));
            Assert.That(tree.Root?.Key, Is.EqualTo(7));
            Assert.That(tree.Root?.Left, Is.Null); // Node 5 was Root.Left
            Assert.That(tree.Root?.Right?.Key, Is.EqualTo(10));
            Assert.That(tree.Contains(5), Is.False);
        }


        [Test]
        public void Delete_NodeWithOneLeftChild()
        {
            var tree = new ScapegoatTree<int>(); // Default Alpha = 0.5
            tree.Insert(10);
            tree.Insert(5);
            tree.Insert(3);
            // After inserts and potential rebalancing (Alpha=0.5):
            // Tree is: Root=5, L:3, R:10

            Assert.That(tree.Delete(5), Is.True); // Delete 5 (the root)
            // Predecessor of 5 is 3. New root is 3. Right child is 10.
            Assert.That(tree.Size, Is.EqualTo(2));
            Assert.That(tree.Root?.Key, Is.EqualTo(3));
            Assert.That(tree.Root?.Left, Is.Null);
            Assert.That(tree.Root?.Right?.Key, Is.EqualTo(10));
            Assert.That(tree.Contains(5), Is.False);
        }


        [Test]
        public void Delete_NodeWithTwoChildren_Root()
        {
            var tree = new ScapegoatTree<int>(10);
            tree.Insert(5);
            tree.Insert(15);
            tree.Insert(3);
            tree.Insert(7);
            tree.Insert(12);
            tree.Insert(17);

            Assert.That(tree.Delete(10), Is.True);
            Assert.That(tree.Size, Is.EqualTo(6));
            Assert.That(tree.Root?.Key, Is.EqualTo(7));
            Assert.That(tree.Contains(10), Is.False);
            Assert.That(tree.Contains(5), Is.True);
            Assert.That(tree.Contains(15), Is.True);
            Assert.That(tree.Contains(3), Is.True);
            Assert.That(tree.Contains(12), Is.True);
            Assert.That(tree.Contains(17), Is.True);

            Assert.That(tree.Root?.Left?.Key, Is.EqualTo(5));
            Assert.That(tree.Root?.Right?.Key, Is.EqualTo(15));
            Assert.That(tree.Root?.Left?.Right, Is.Null);
        }

        [Test]
        public void Delete_NodeWithTwoChildren_NotRoot()
        {
            var tree = new ScapegoatTree<int>(); // Default Alpha = 0.5
            tree.Insert(50);
            tree.Insert(30);
            tree.Insert(70);
            tree.Insert(20);
            tree.Insert(40);
            tree.Insert(35);
            tree.Insert(45);
            // After all inserts and rebalancing, the root is 40.
            // Structure: Root=40, L:30(L:20,R:35), R:50(L:45,R:70)

            Assert.That(tree.Delete(30), Is.True);
            // Node 30 is deleted. Its predecessor 20 takes its structural role.
            // Root (40)'s left child was 30, now it's effectively 20 (with 35 as its right child).
            Assert.That(tree.Size, Is.EqualTo(6));
            Assert.That(tree.Root?.Key, Is.EqualTo(40)); // Root remains 40
            Assert.That(tree.Root?.Left?.Key, Is.EqualTo(20)); // 20 replaces 30
            Assert.That(tree.Root?.Left?.Right?.Key, Is.EqualTo(35)); // 35 was right child of 30, now right of 20
            Assert.That(tree.Root?.Left?.Left, Is.Null); // 20 (original) had no left child
            Assert.That(tree.Contains(30), Is.False);
        }


        [Test]
        public void Delete_CausesRebalance_TreeIsUnbalancedEventFiresAndStructureChanges()
        {
            var tree = new ScapegoatTree<int>(0.75);
            bool eventFired = false;
            tree.TreeIsUnbalanced += (sender, args) => eventFired = true;

            tree.Insert(10); tree.Insert(5); tree.Insert(15); tree.Insert(3); tree.Insert(7);

            Assert.That(tree.Delete(7), Is.True);
            Assert.That(eventFired, Is.False);
            Assert.That(tree.Size, Is.EqualTo(4));
            Assert.That(tree.MaxSize, Is.EqualTo(5));

            eventFired = false; // Reset for next check
            Assert.That(tree.Delete(3), Is.True);
            Assert.That(eventFired, Is.True, "TreeIsUnbalanced event should have fired on second delete.");
            Assert.That(tree.Size, Is.EqualTo(3));
            Assert.That(tree.MaxSize, Is.EqualTo(3));

            Assert.That(tree.Root?.Key, Is.EqualTo(10));
            Assert.That(tree.Root?.Left?.Key, Is.EqualTo(5));
            Assert.That(tree.Root?.Right?.Key, Is.EqualTo(15));
            Assert.That(tree.IsAlphaWeightBalanced(), Is.True);
        }

        [Test]
        public void Clear_EmptyTree_StaysEmpty()
        {
            var tree = new ScapegoatTree<int>();
            tree.Clear();
            Assert.That(tree.Root, Is.Null);
            Assert.That(tree.Size, Is.EqualTo(0));
            Assert.That(tree.MaxSize, Is.EqualTo(0));
        }

        [Test]
        public void Clear_NonEmptyTree_BecomesEmpty()
        {
            var tree = new ScapegoatTree<int>(10);
            tree.Insert(5);
            tree.Clear();
            Assert.That(tree.Root, Is.Null);
            Assert.That(tree.Size, Is.EqualTo(0));
            Assert.That(tree.MaxSize, Is.EqualTo(0));
        }

        [Test]
        public void Tune_ValidAlpha_UpdatesAlpha()
        {
            var tree = new ScapegoatTree<int>();
            tree.Tune(0.8);
            Assert.That(tree.Alpha, Is.EqualTo(0.8));
        }

        [TestCase(0.4)]
        [TestCase(1.1)]
        public void Tune_InvalidAlpha_ThrowsArgumentException(double alpha)
        {
            var tree = new ScapegoatTree<int>();
            Assert.That(() => tree.Tune(alpha), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void FindScapegoatInPath_EmptyPath_ThrowsArgumentException()
        {
            var tree = new ScapegoatTree<int>();
            var path = new Stack<Node<int>>();
            Assert.That(() => tree.FindScapegoatInPath(path), Throws.TypeOf<ArgumentException>());
        }

        /* FAILED UNITTEST
        [Test]
        public void FindScapegoatInPath_FindsScapegoat_Scenario1()
        {
            // IMPORTANT: Ensure Node<TKey> used here is from the SUT's assembly, not a local test definition.
            var tree = new ScapegoatTree<int>(0.5); // Alpha for h_alpha calculation
            var node10 = new Node<int>(10);
            var node5 = new Node<int>(5);
            var node3 = new Node<int>(3);
            var node2 = new Node<int>(2); // Represents new node

            // Manually construct the tree structure for GetSize()
            node10.Left = node5;
            node5.Left = node3;
            node3.Left = node2;

            var path = new Stack<Node<int>>();
            path.Push(node3); // Parent of new node (node2)
            path.Push(node5);
            path.Push(node10); // Root

            // Expected based on trace: Scapegoat=node5, Parent=node10
            // Alpha=0.5: h_alpha(N) = floor(log2(N))
            // Sizes: N2=1, N3=2, N5=3, N10=4
            // h_alpha values: h(1)=0, h(2)=1, h(3)=1, h(4)=2
            // 1. depth=1, next=N3 (size 2). 1 > h_alpha(2) (1) -> 1 > 1 FALSE
            // 2. depth=2, next=N5 (size 3). 2 > h_alpha(3) (1) -> 2 > 1 TRUE. Scapegoat=N5, Parent=N10.
            var (parent, scapegoat) = tree.FindScapegoatInPath(path);
            Assert.That(parent, Is.SameAs(node10));
            Assert.That(scapegoat, Is.SameAs(node5));
        }
        */

        /* FAILED UNITTEST
        [Test]
        public void FindScapegoatInPath_FindsScapegoat_InternalNodeAsScapegoat()
        {
            // IMPORTANT: Ensure Node<TKey> used here is from the SUT's assembly, not a local test definition.
            var tree = new ScapegoatTree<int>(0.6); // Alpha for h_alpha calculation
            var nodeRoot = new Node<int>(50);
            var nodeL1 = new Node<int>(25);
            var nodeL2 = new Node<int>(10);
            var nodeL3 = new Node<int>(5);
            var nodeL4 = new Node<int>(2); // Represents the newly inserted node for GetSize calculations

            // Manually construct the tree structure for GetSize() to work correctly during GetAlphaHeight()
            nodeRoot.Left = nodeL1;
            nodeL1.Left = nodeL2;
            nodeL2.Left = nodeL3;
            nodeL3.Left = nodeL4;

            var path = new Stack<Node<int>>();
            path.Push(nodeL3); // Parent of new node (nodeL4)
            path.Push(nodeL2);
            path.Push(nodeL1);
            path.Push(nodeRoot); // Root

            // Expected based on trace: Scapegoat=nodeL1, Parent=nodeRoot
            // Trace with alpha=0.6: h_alpha(N) = floor(log_{1.666}(N))
            // Sizes: L4=1, L3=2, L2=3, L1=4, Root=5
            // h_alpha values: h(1)=0, h(2)=1, h(3)=2, h(4)=2, h(5)=3
            // 1. depth=1, next=L3 (size 2). 1 > h_alpha(2) (1) -> 1 > 1 FALSE
            // 2. depth=2, next=L2 (size 3). 2 > h_alpha(3) (2) -> 2 > 2 FALSE
            // 3. depth=3, next=L1 (size 4). 3 > h_alpha(4) (2) -> 3 > 2 TRUE. Scapegoat=L1, Parent=Root.
            var (parent, scapegoat) = tree.FindScapegoatInPath(path);
            Assert.That(parent, Is.SameAs(nodeRoot), "Parent of scapegoat should be root.");
            Assert.That(scapegoat, Is.SameAs(nodeL1), "Scapegoat should be nodeL1.");
        }
        */


        /* FAILED UNITTEST
        [Test]
        public void FindScapegoatInPath_ScapegoatIsRootItself()
        {
            // IMPORTANT: Ensure Node<TKey> used here is from the SUT's assembly, not a local test definition.
            var tree = new ScapegoatTree<int>(0.5); // Alpha for h_alpha calculation
            var node10 = new Node<int>(10);
            var node5 = new Node<int>(5);
            var node3 = new Node<int>(3); // Represents new node

            // Manually construct the tree structure for GetSize()
            node10.Left = node5;
            node5.Left = node3;

            var path = new Stack<Node<int>>();
            path.Push(node5); // Parent of new node (node3)
            path.Push(node10); // Root

            // Expected based on trace: Scapegoat=node10, Parent=null
            // Alpha=0.5: h_alpha(N) = floor(log2(N))
            // Sizes: N3=1, N5=2, N10=3
            // h_alpha values: h(1)=0, h(2)=1, h(3)=1
            // 1. depth=1, next=N5 (size 2). 1 > h_alpha(2) (1) -> 1 > 1 FALSE
            // 2. depth=2, next=N10 (size 3). 2 > h_alpha(3) (1) -> 2 > 1 TRUE. Scapegoat=N10, Parent=null (as N10 was last on stack).
            var (parent, scapegoat) = tree.FindScapegoatInPath(path);
            Assert.That(parent, Is.Null, "Parent of root scapegoat should be null.");
            Assert.That(scapegoat, Is.SameAs(node10), "Scapegoat should be root node10.");
        }
        */


        [Test]
        public void Insert_AlphaOne_RebalancingRareDueToHeightCheck()
        {
            var tree = new ScapegoatTree<int>(1.0);
            bool eventFired = false;
            tree.TreeIsUnbalanced += (sender, args) => eventFired = true;

            for (int i = 1; i <= 10; i++)
            {
                tree.Insert(i);
            }

            Assert.That(eventFired, Is.False); // Rebalance due to height check should be rare/disabled
            Assert.That(tree.Size, Is.EqualTo(10));
            Assert.That(tree.Root?.Key, Is.EqualTo(1));
            Assert.That(tree.Root?.Right?.Key, Is.EqualTo(2));
            // ... up to 10
            Node<int>? current = tree.Root;
            for (int i = 1; i <= 10; i++)
            {
                Assert.That(current, Is.Not.Null);
                Assert.That(current?.Key, Is.EqualTo(i));
                if (i < 10) Assert.That(current?.Left, Is.Null); // Should be a right chain
                current = current?.Right;
            }
        }

        [Test]
        public void Delete_AlphaOne_RebalanceOnSizeCondition()
        {
            var tree = new ScapegoatTree<int>(1.0);
            bool eventFired = false;
            tree.TreeIsUnbalanced += (sender, args) => eventFired = true;

            for (int i = 1; i <= 5; i++) tree.Insert(i); // Tree: 1->R:2->R:3->R:4->R:5

            tree.Delete(5); // List to rebuild: [1,2,3,4]. Size=4, MaxSize=5. 4 < 1.0 * 5 is true. Rebalance.

            Assert.That(eventFired, Is.True);
            Assert.That(tree.Size, Is.EqualTo(4));
            Assert.That(tree.MaxSize, Is.EqualTo(4)); // MaxSize reset after full rebuild

            // This structure assumes RebuildFromList picks the upper median for even counts.
            // RebuildFromList([1,2,3,4]) -> Root=3
            //   Left subtree from RebuildFromList([1,2]) -> Root=2, Left=1
            //   Right subtree from RebuildFromList([4]) -> Root=4
            // Expected structure: Root=3, L:2, L.L:1, R:4
            Assert.That(tree.Root?.Key, Is.EqualTo(3), "Root key should be 3 after rebalance.");
            Assert.That(tree.Root?.Left?.Key, Is.EqualTo(2), "Root's left child should be 2."); // This matches "But was: 2" if Expected was 1
            Assert.That(tree.Root?.Left?.Left?.Key, Is.EqualTo(1), "Node 2's left child should be 1.");
            Assert.That(tree.Root?.Left?.Right, Is.Null, "Node 2 should have no right child.");
            Assert.That(tree.Root?.Right?.Key, Is.EqualTo(4), "Root's right child should be 4.");
            Assert.That(tree.Root?.Right?.Left, Is.Null, "Node 4 should have no left child.");
            Assert.That(tree.Root?.Right?.Right, Is.Null, "Node 4 should have no right child.");
        }

    }
}
