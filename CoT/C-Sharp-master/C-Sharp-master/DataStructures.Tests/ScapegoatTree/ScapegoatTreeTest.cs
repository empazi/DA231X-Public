using NUnit.Framework;
using DataStructures.ScapegoatTree; // Your namespace
using System;
using System.Collections.Generic;
using System.Linq; // For ToList

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
            Assert.That(tree.Size, Is.EqualTo(0));
            Assert.That(tree.MaxSize, Is.EqualTo(0));
            Assert.That(tree.Root, Is.Null);
        }

        [Test]
        public void Constructor_WithAlpha_InitializesCorrectly()
        {
            var tree = new ScapegoatTree<int>(0.75);
            Assert.That(tree.Alpha, Is.EqualTo(0.75));
            Assert.That(tree.Size, Is.EqualTo(0));
            Assert.That(tree.MaxSize, Is.EqualTo(0));
            Assert.That(tree.Root, Is.Null);
        }

        [Test]
        public void Constructor_WithNodeAndAlpha_InitializesCorrectly()
        {
            var rootNode = new Node<int>(10);
            rootNode.Left = new Node<int>(5);
            rootNode.Right = new Node<int>(15);
            // Manually update sizes for this test scenario if Node doesn't do it automatically,
            // or rely on GetSize(). ScapegoatTree constructor calls node.GetSize().

            var tree = new ScapegoatTree<int>(rootNode, 0.6);
            Assert.That(tree.Alpha, Is.EqualTo(0.6));
            Assert.That(tree.Root, Is.EqualTo(rootNode));
            Assert.That(tree.Size, Is.EqualTo(3)); // Assuming GetSize() works
            Assert.That(tree.MaxSize, Is.EqualTo(3));
        }

        [Test]
        public void Constructor_WithKeyAndAlpha_InitializesCorrectly()
        {
            var tree = new ScapegoatTree<int>(10, 0.65);
            Assert.That(tree.Alpha, Is.EqualTo(0.65));
            Assert.That(tree.Size, Is.EqualTo(1));
            Assert.That(tree.MaxSize, Is.EqualTo(1));
            Assert.That(tree.Root, Is.Not.Null);
            Assert.That(tree.Root!.Key, Is.EqualTo(10));
        }

        [TestCase(0.49)]
        [TestCase(1.01)]
        public void Constructor_WithInvalidAlpha_ThrowsArgumentException(double invalidAlpha)
        {
            Assert.Throws<ArgumentException>(() => new ScapegoatTree<int>(invalidAlpha));
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
        public void Tune_InvalidAlpha_ThrowsArgumentException(double invalidAlpha)
        {
            var tree = new ScapegoatTree<int>();
            Assert.Throws<ArgumentException>(() => tree.Tune(invalidAlpha));
        }

        [Test]
        public void Insert_EmptyTree_AddsNodeAsRoot()
        {
            var tree = new ScapegoatTree<int>();
            Assert.That(tree.Insert(10), Is.True);
            Assert.That(tree.Size, Is.EqualTo(1));
            Assert.That(tree.MaxSize, Is.EqualTo(1));
            Assert.That(tree.Root, Is.Not.Null);
            Assert.That(tree.Root!.Key, Is.EqualTo(10));
        }

        [Test]
        public void Insert_SmallerKey_AddsToLeft()
        {
            var tree = new ScapegoatTree<int>(10);
            Assert.That(tree.Insert(5), Is.True);
            Assert.That(tree.Size, Is.EqualTo(2));
            Assert.That(tree.Root!.Left, Is.Not.Null);
            Assert.That(tree.Root!.Left!.Key, Is.EqualTo(5));
        }

        [Test]
        public void Insert_LargerKey_AddsToRight()
        {
            var tree = new ScapegoatTree<int>(10);
            Assert.That(tree.Insert(15), Is.True);
            Assert.That(tree.Size, Is.EqualTo(2));
            Assert.That(tree.Root!.Right, Is.Not.Null);
            Assert.That(tree.Root!.Right!.Key, Is.EqualTo(15));
        }

        [Test]
        public void Insert_DuplicateKey_ReturnsFalse()
        {
            var tree = new ScapegoatTree<int>(10);
            tree.Insert(5);
            Assert.That(tree.Insert(5), Is.False);
            Assert.That(tree.Size, Is.EqualTo(2)); // Size should not change
        }

        [Test]
        public void Insert_MultipleKeys_CorrectSizeAndStructure()
        {
            var tree = new ScapegoatTree<int>();
            int[] values = { 50, 30, 70, 20, 40, 60, 80 };
            foreach (var val in values)
            {
                tree.Insert(val);
            }
            Assert.That(tree.Size, Is.EqualTo(values.Length));
            Assert.That(tree.MaxSize, Is.EqualTo(values.Length));
            // Basic structure check
            Assert.That(tree.Root!.Key, Is.EqualTo(50));
            Assert.That(tree.Root!.Left!.Key, Is.EqualTo(30));
            Assert.That(tree.Root!.Right!.Key, Is.EqualTo(70));
            Assert.That(tree.Root!.Left!.Left!.Key, Is.EqualTo(20));
        }

        [Test]
        public void Contains_EmptyTree_ReturnsFalse()
        {
            var tree = new ScapegoatTree<int>();
            Assert.That(tree.Contains(10), Is.False);
        }

        [Test]
        public void Contains_ExistingKey_ReturnsTrue()
        {
            var tree = new ScapegoatTree<int>(10);
            tree.Insert(5);
            tree.Insert(15);
            Assert.That(tree.Contains(15), Is.True);
        }

        [Test]
        public void Contains_NonExistingKey_ReturnsFalse()
        {
            var tree = new ScapegoatTree<int>(10);
            tree.Insert(5);
            tree.Insert(15);
            Assert.That(tree.Contains(100), Is.False);
        }

        [Test]
        public void Search_EmptyTree_ReturnsNull()
        {
            var tree = new ScapegoatTree<int>();
            Assert.That(tree.Search(10), Is.Null);
        }

        [Test]
        public void Search_ExistingKey_ReturnsNode()
        {
            var tree = new ScapegoatTree<int>(10);
            tree.Insert(5);
            var node = tree.Search(5);
            Assert.That(node, Is.Not.Null);
            Assert.That(node!.Key, Is.EqualTo(5));
        }

        [Test]
        public void Search_NonExistingKey_ReturnsNull()
        {
            var tree = new ScapegoatTree<int>(10);
            Assert.That(tree.Search(100), Is.Null);
        }

        [Test]
        public void Delete_EmptyTree_ReturnsFalse()
        {
            var tree = new ScapegoatTree<int>();
            Assert.That(tree.Delete(10), Is.False);
        }

        [Test]
        public void Delete_NonExistingKey_ReturnsFalse()
        {
            var tree = new ScapegoatTree<int>(10);
            Assert.That(tree.Delete(100), Is.False);
            Assert.That(tree.Size, Is.EqualTo(1));
        }

        [Test]
        public void Delete_LeafNode_RemovesNode()
        {
            var tree = new ScapegoatTree<int>(10);
            tree.Insert(5);
            Assert.That(tree.Delete(5), Is.True);
            Assert.That(tree.Size, Is.EqualTo(1));
            Assert.That(tree.Root!.Left, Is.Null);
            Assert.That(tree.Contains(5), Is.False);
        }

        [Test]
        public void Delete_NodeWithOneRightChild_RemovesNode()
        {
            var tree = new ScapegoatTree<int>(10);
            tree.Insert(5);
            tree.Insert(7); // 5 has right child 7
            Assert.That(tree.Delete(5), Is.True);
            Assert.That(tree.Size, Is.EqualTo(2));
            Assert.That(tree.Root!.Key, Is.EqualTo(7));
            Assert.That(tree.Root!.Left, Is.Null);
            Assert.That(tree.Root!.Right, Is.Not.Null);
            Assert.That(tree.Root!.Right!.Key, Is.EqualTo(10));
            Assert.That(tree.Contains(5), Is.False);
        }

        [Test]
        public void Delete_NodeWithOneLeftChild_RemovesNode()
        {
            var tree = new ScapegoatTree<int>(10);
            tree.Insert(5);
            tree.Insert(3); // 5 has left child 3
            Assert.That(tree.Delete(5), Is.True);
            Assert.That(tree.Size, Is.EqualTo(2));
            Assert.That(tree.Root!.Key, Is.EqualTo(3));
            Assert.That(tree.Root!.Left, Is.Null);
            Assert.That(tree.Root!.Right, Is.Not.Null);
            Assert.That(tree.Root!.Right!.Key, Is.EqualTo(10));
            Assert.That(tree.Contains(5), Is.False);
        }

        [Test]
        public void Delete_NodeWithTwoChildren_RemovesNodeAndReplacesWithPredecessor()
        {
            var tree = new ScapegoatTree<int>();
            int[] values = { 50, 30, 70, 20, 40, 60, 80 }; // Balanced tree
            foreach (var val in values) tree.Insert(val);

            // Delete 30 (predecessor is 20)
            Assert.That(tree.Delete(30), Is.True);
            Assert.That(tree.Size, Is.EqualTo(6));
            Assert.That(tree.Contains(30), Is.False);
            Assert.That(tree.Root!.Left!.Key, Is.EqualTo(20)); // 20 replaces 30
            Assert.That(tree.Root!.Left!.Right!.Key, Is.EqualTo(40)); // 40 is still child of new 20
            Assert.That(tree.Root!.Left!.Left, Is.Null); // Original 20's spot is now null
            Assert.That(tree.IsAlphaWeightBalanced(), Is.True); // Check if still balanced
        }

        [Test]
        public void Delete_RootNode_WithNoChildren_SetsRootToNull()
        {
            var tree = new ScapegoatTree<int>(10);
            Assert.That(tree.Delete(10), Is.True);
            Assert.That(tree.Size, Is.EqualTo(0));
            Assert.That(tree.Root, Is.Null);
        }

        [Test]
        public void Delete_RootNode_WithOneChild_ReplacesRoot()
        {
            var tree = new ScapegoatTree<int>(10);
            tree.Insert(5);
            Assert.That(tree.Delete(10), Is.True);
            Assert.That(tree.Size, Is.EqualTo(1));
            Assert.That(tree.Root!.Key, Is.EqualTo(5));
        }

        [Test]
        public void Delete_RootNode_WithTwoChildren_ReplacesRootWithPredecessor()
        {
            var tree = new ScapegoatTree<int>(10);
            tree.Insert(5);
            tree.Insert(15);
            tree.Insert(3); // predecessor of 10 is 5

            Assert.That(tree.Delete(10), Is.True);
            Assert.That(tree.Size, Is.EqualTo(3));
            Assert.That(tree.Root!.Key, Is.EqualTo(5));
            Assert.That(tree.Root!.Left!.Key, Is.EqualTo(3));
            Assert.That(tree.Root!.Right!.Key, Is.EqualTo(15));
            Assert.That(tree.Contains(10), Is.False);
        }

        [Test]
        public void Clear_EmptiesTree()
        {
            var tree = new ScapegoatTree<int>(10);
            tree.Insert(5);
            tree.Insert(15);
            tree.Clear();
            Assert.That(tree.Size, Is.EqualTo(0));
            Assert.That(tree.MaxSize, Is.EqualTo(0));
            Assert.That(tree.Root, Is.Null);
            Assert.That(tree.Contains(10), Is.False);
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
        public void IsAlphaWeightBalanced_PerfectlyBalancedTree_ReturnsTrue()
        {
            var tree = new ScapegoatTree<int>(0.5); // Alpha 0.5
            tree.Insert(10);
            tree.Insert(5);
            tree.Insert(15); // Perfectly balanced
            Assert.That(tree.IsAlphaWeightBalanced(), Is.True);
        }

        [Test]
        public void IsAlphaWeightBalanced_UnbalancedTree_ReturnsFalse()
        {
            // Construct a tree that is not alpha-weight-balanced manually
            // For alpha = 0.5, size(left) <= 0.5 * size(node) and size(right) <= 0.5 * size(node) is too strict.
            // The definition is: size(left) <= alpha * size(node) AND size(right) <= alpha * size(node)
            // Let alpha = 0.6
            // Node A (size 5), Left B (size 3), Right C (size 1)
            // size(B) = 3. alpha * size(A) = 0.6 * 5 = 3.  3 <= 3 is true.
            // size(C) = 1. alpha * size(A) = 0.6 * 5 = 3.  1 <= 3 is true.
            // This node A is balanced.
            //
            // Let's try alpha = 0.7
            // Node A (size 5), Left B (size 4), Right null (size 0)
            // size(B) = 4. alpha * size(A) = 0.7 * 5 = 3.5.  4 <= 3.5 is false. Node A is unbalanced.
            var nodeA = new Node<int>(10); // Root
            var nodeB = new Node<int>(5);
            var nodeC = new Node<int>(3);
            var nodeD = new Node<int>(1);
            var nodeE = new Node<int>(7);

            nodeA.Left = nodeB; // A is root, B is left child
            nodeB.Left = nodeC; // B is parent of C and E
            nodeC.Left = nodeD; // C is parent of D
            nodeB.Right = nodeE;
            // Structure:
            //      A(10) size 5
            //     /
            //    B(5) size 4
            //   /  \
            //  C(3) E(7) size 1
            // /
            //D(1) size 1
            // nodeC size 2 (C,D)
            // nodeB size 4 (B,C,D,E)
            // nodeA size 5 (A,B,C,D,E)

            var tree = new ScapegoatTree<int>(nodeA, 0.7); // Alpha = 0.7
            // Check node B: size(B)=4. Left child C size 2. Right child E size 1.
            // size(C) = 2. alpha * size(B) = 0.7 * 4 = 2.8.  2 <= 2.8 (ok)
            // size(E) = 1. alpha * size(B) = 0.7 * 4 = 2.8.  1 <= 2.8 (ok)
            // So B is balanced.
            // Check node A: size(A)=5. Left child B size 4. Right child null size 0.
            // size(B) = 4. alpha * size(A) = 0.7 * 5 = 3.5.  4 <= 3.5 is FALSE.
            // So A is unbalanced.
            Assert.That(tree.IsAlphaWeightBalanced(), Is.False);
        }

        [Test]
        public void Insert_TriggersRebalance_AndEvent()
        {
            var tree = new ScapegoatTree<int>(0.5); // Alpha 0.5 for predictable rebalance
            bool eventRaised = false;
            tree.TreeIsUnbalanced += (sender, args) => eventRaised = true;

            tree.Insert(10); // Size 1, MaxSize 1. path.Count for next insert (depth of new node) = 1. h_a(1)=0. 1>0.
                             // No, path.Count is depth of parent.
                             // Insert 10: size=1, maxS=1. Root=10.
            tree.Insert(20); // Size 2, MaxSize 2. path=[10], path.Count=1. h_a(Root.Size=2)=1. 1>1 is false. No rebalance.
                             // Tree: 10-R->20
            Assert.That(eventRaised, Is.False);

            tree.Insert(30); // Size 3, MaxSize 3. path=[10,20], path.Count=2. h_a(Root.Size=3)=1. 2>1 is true. Rebalance!
                             // Scapegoat is Root (10). New root should be 20.
            Assert.That(eventRaised, Is.True);
            Assert.That(tree.Root!.Key, Is.EqualTo(20));
            Assert.That(tree.Root!.Left!.Key, Is.EqualTo(10));
            Assert.That(tree.Root!.Right!.Key, Is.EqualTo(30));
            Assert.That(tree.Size, Is.EqualTo(3));
            Assert.That(tree.IsAlphaWeightBalanced(), Is.True);
        }

        [Test]
        public void Delete_TriggersRebalance_AndEvent()
        {
            var tree = new ScapegoatTree<int>(0.5); // Alpha 0.5
            bool eventRaised = false;

            tree.Insert(20);
            tree.Insert(10);
            tree.Insert(30); // Root=20, L=10, R=30. Size=3, MaxSize=3
            tree.Insert(5);  // Root=20, L=10(L=5), R=30. Size=4, MaxSize=4

            tree.TreeIsUnbalanced += (sender, args) => eventRaised = true;

            // MaxSize = 4.
            // Delete 30. Size becomes 3.  3 < 0.5 * 4 (2.0) is false. No rebalance.
            tree.Delete(30);
            Assert.That(eventRaised, Is.False);
            Assert.That(tree.Size, Is.EqualTo(3)); // 20, 10, 5
            Assert.That(tree.MaxSize, Is.EqualTo(4)); // MaxSize not reset yet

            // Delete 5. Size becomes 2. 2 < 0.5 * 4 (2.0) is false. (2 is not < 2). No rebalance.
            tree.Delete(5);
            Assert.That(eventRaised, Is.False);
            Assert.That(tree.Size, Is.EqualTo(2)); // 20, 10
            Assert.That(tree.MaxSize, Is.EqualTo(4));

            // Delete 10. Size becomes 1. 1 < 0.5 * 4 (2.0) is true. Rebalance!
            tree.Delete(10);
            Assert.That(eventRaised, Is.True);
            Assert.That(tree.Size, Is.EqualTo(1)); // Root=20
            Assert.That(tree.Root!.Key!, Is.EqualTo(20));
            Assert.That(tree.MaxSize, Is.EqualTo(1)); // MaxSize reset to Size
            Assert.That(tree.IsAlphaWeightBalanced(), Is.True);
        }

        [Test]
        public void FindScapegoatInPath_EmptyPath_ThrowsArgumentException()
        {
            var tree = new ScapegoatTree<int>(0.5);
            var path = new Stack<Node<int>>();
            Assert.Throws<ArgumentException>(() => tree.FindScapegoatInPath(path));
        }

        [Test]
        public void FindScapegoatInPath_PathLeadsToScapegoat_ReturnsScapegoatAndParent()
        {
            var tree = new ScapegoatTree<int>(0.5); // Alpha = 0.5

            // Construct path: A -> B -> C (parent of new node X)
            // New node X is inserted under C.
            // Sizes after insertion of X:
            // X (key 5), size 1
            // C (key 10), with X (key 5) as left child. C.Size = 2.
            // B (key 20), left child C. B.Size = 1 (B) + 2 (C's subtree) = 3.
            // A (key 30), left child B. A.Size = 1 (A) + 3 (B's subtree) = 4.
            var nodeA = new Node<int>(30); // Root of this manual structure
            var nodeB = new Node<int>(20);
            var nodeC = new Node<int>(10);
            var nodeX = new Node<int>(5); // Represents the new node

            nodeA.Left = nodeB;
            nodeB.Left = nodeC;
            nodeC.Left = nodeX; // This makes GetSize() work correctly

            var path = new Stack<Node<int>>();
            path.Push(nodeA); // Root
            path.Push(nodeB); // Child
            path.Push(nodeC); // Parent of new node (conceptually)

            // Expected: Scapegoat is B, Parent is A
            // Check:
            // 1. Pop C(10). C.GetSize()=2. depth_var=1. h_0.5(2)=1. Is 1 > 1? No.
            // 2. Pop B(20). B.GetSize()=3. depth_var=2. h_0.5(3)=1. Is 2 > 1? Yes. Scapegoat=B(20), Parent=A(30).
            var (parent, scapegoat) = tree.FindScapegoatInPath(path);

            Assert.That(scapegoat, Is.EqualTo(nodeB));
            Assert.That(parent, Is.EqualTo(nodeA));
        }

        [Test]
        public void FindScapegoatInPath_ScapegoatIsRoot_ParentIsNull()
        {
            var tree = new ScapegoatTree<int>(0.5);
            // Path: A (Root, parent of new node X)
            // New node X under A.
            // A (key 10), with X (key 5) as left child. A.Size = 2
            // This scenario is too simple, GetAlphaHeight(1)=0, depth=1, 1>0.
            // Let's use the 10,20,30 example where root became scapegoat.
            // Insert 10, 20, 30. Path for 30 is [10,20].
            // Sizes: Node(30) size 1. Node(20) parent of 30, size 2. Node(10) parent of 20, size 3.
            var node10 = new Node<int>(10);
            var node20 = new Node<int>(20);
            var node30 = new Node<int>(30); // Represents the new node

            node10.Right = node20;
            node20.Right = node30;

            var path = new Stack<Node<int>>();
            path.Push(node10); // Root
            path.Push(node20); // Parent of new node (node30)

            // Check:
            // 1. Pop 20. next=20(s=2). depth_var=1. h_0.5(2)=1. Is 1 > 1? No.
            // 2. Pop 10. next=10(s=3). depth_var=2. h_0.5(3)=1. Is 2 > 1? Yes. Scapegoat=10, Parent=null (from stack).
            var (parent, scapegoat) = tree.FindScapegoatInPath(path);
            Assert.That(scapegoat, Is.EqualTo(node10));
            Assert.That(parent, Is.Null);
        }

        [Test]
        public void FindScapegoatInPath_BalancedPath_ThrowsInvalidOperationException()
        {
            var tree = new ScapegoatTree<int>(0.5);
            // Path: A -> B. Tree is balanced along this path.
            // A (key 20, size 3), L: B (key 10, size 1), R: C (key 30, size 1)
            // Suppose we are checking path [A, B] after inserting something under B, making B.size=1.
            // This setup is for a path that *would not* be passed to FindScapegoatInPath if tree was balanced.
            // We are testing the method in isolation.
            var nodeA = new Node<int>(20);
            var nodeB = new Node<int>(10);
            var nodeC = new Node<int>(30); // Not on path, but affects A's size

            nodeA.Left = nodeB;
            nodeA.Right = nodeC;
            // nodeB.Size = 1, nodeA.Size = 3

            var path = new Stack<Node<int>>();
            path.Push(nodeA);
            path.Push(nodeB); // Path to parent of a hypothetical new node under B

            // Check:
            // 1. Pop B. next=B(s=1). depth_var=1. h_0.5(1)=0. Is 1 > 0? Yes.
            // This means B is the scapegoat. The test name is misleading.
            // The exception is "Scapegoat node wasn't found. The tree should be unbalanced."
            // This occurs if the loop finishes.
            // For the loop to finish, for all nodes `next` on path, `depth <= next.GetAlphaHeight(Alpha)`.

            // Let's try a path where no scapegoat is found.
            // A(s=4), L:B(s=2), R:C(s=1). Path [A,B]. New node under B.
            // B becomes size 2 (B + new node). A becomes size 4 (A + B_subtree + C).
            var rootNode = new Node<int>(40); // size 4
            var childNode = new Node<int>(20); // size 2 (child + new hypothetical)
            var otherChild = new Node<int>(60); // size 1
            var newGrandChild = new Node<int>(10); // hypothetical new node

            rootNode.Left = childNode;
            rootNode.Right = otherChild;
            childNode.Left = newGrandChild; // So childNode.GetSize() is 2

            var pathForEx = new Stack<Node<int>>();
            pathForEx.Push(rootNode);
            pathForEx.Push(childNode);

            // Check with alpha = 0.5:
            // 1. Pop childNode(s=2). depth_var=1. h_0.5(2)=1. Is 1 > 1? No.
            // 2. Pop rootNode(s=4). depth_var=2. h_0.5(4)=2. Is 2 > 2? No.
            // Loop ends. Exception should be thrown.
            Assert.Throws<InvalidOperationException>(() => tree.FindScapegoatInPath(pathForEx));
        }

        [Test]
        public void ComplexOperations_InsertDeleteRebalance_MaintainsIntegrity()
        {
            var tree = new ScapegoatTree<int>(0.55); // Use a slightly different alpha
            var numbers = new List<int>();
            var random = new Random(123); // Seeded for reproducibility

            for (int i = 0; i < 100; i++)
            {
                var num = random.Next(200);
                if (tree.Insert(num))
                {
                    numbers.Add(num);
                }
            }

            Assert.That(tree.Size, Is.EqualTo(numbers.Count));
            foreach (var num in numbers)
            {
                Assert.That(tree.Contains(num), Is.True, $"Tree should contain {num} after insertions.");
            }
            Assert.That(tree.IsAlphaWeightBalanced(), Is.True, "Tree should be balanced after insertions.");

            // Delete about half of them
            var numbersToRemove = numbers.OrderBy(x => random.Next()).Take(numbers.Count / 2).ToList();
            foreach (var num in numbersToRemove)
            {
                Assert.That(tree.Delete(num), Is.True, $"Deleting {num} should succeed.");
                numbers.Remove(num);
            }

            Assert.That(tree.Size, Is.EqualTo(numbers.Count));
            foreach (var num in numbers)
            {
                Assert.That(tree.Contains(num), Is.True, $"Tree should still contain {num}.");
            }
            foreach (var num in numbersToRemove)
            {
                Assert.That(tree.Contains(num), Is.False, $"Tree should not contain removed {num}.");
            }
            Assert.That(tree.IsAlphaWeightBalanced(), Is.True, "Tree should be balanced after deletions.");
        }
    }
}
