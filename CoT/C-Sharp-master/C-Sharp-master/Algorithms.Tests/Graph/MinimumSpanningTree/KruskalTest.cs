using NUnit.Framework;
using Algorithms.Graph.MinimumSpanningTree;
using System;
using System.Collections.Generic;

namespace Algorithms.Tests.Graph.MinimumSpanningTree
{
    [TestFixture]
    public class KruskalTests
    {
        private const float Inf = float.PositiveInfinity;

        #region Helper Methods

        private static bool AreMatricesEqual(float[,] matrix1, float[,] matrix2)
        {
            if (matrix1.GetLength(0) != matrix2.GetLength(0) || matrix1.GetLength(1) != matrix2.GetLength(1))
                return false;

            for (int i = 0; i < matrix1.GetLength(0); i++)
            {
                for (int j = 0; j < matrix1.GetLength(1); j++)
                {
                    if (float.IsPositiveInfinity(matrix1[i, j]) && float.IsPositiveInfinity(matrix2[i, j]))
                        continue;
                    if (float.IsNaN(matrix1[i, j]) && float.IsNaN(matrix2[i, j])) // Handle NaN if necessary, though not typical for weights
                        continue;
                    if (Math.Abs(matrix1[i, j] - matrix2[i, j]) > 1e-6)
                        return false;
                }
            }
            return true;
        }

        private static bool AreAdjacencyListsEqual(Dictionary<int, float>[] list1, Dictionary<int, float>[] list2)
        {
            if (list1.Length != list2.Length)
                return false;

            for (int i = 0; i < list1.Length; i++)
            {
                var dict1 = list1[i];
                var dict2 = list2[i];

                if (dict1.Count != dict2.Count)
                    return false;

                foreach (var kvp1 in dict1)
                {
                    if (!dict2.TryGetValue(kvp1.Key, out float weight2) || Math.Abs(kvp1.Value - weight2) > 1e-6)
                        return false;
                }
            }
            return true;
        }

        private static float GetMatrixTotalWeight(float[,] matrix)
        {
            float totalWeight = 0;
            int numNodes = matrix.GetLength(0);
            for (int i = 0; i < numNodes; i++)
            {
                for (int j = i + 1; j < numNodes; j++) // Iterate over upper triangle
                {
                    if (float.IsFinite(matrix[i, j]))
                    {
                        totalWeight += matrix[i, j];
                    }
                }
            }
            return totalWeight;
        }

        private static float GetListTotalWeight(Dictionary<int, float>[] list)
        {
            float totalWeight = 0;
            var visitedEdges = new HashSet<(int, int)>();

            for (int i = 0; i < list.Length; i++)
            {
                foreach (var edge in list[i])
                {
                    int u = i;
                    int v = edge.Key;
                    var edgeTuple = u < v ? (u, v) : (v, u);
                    if (!visitedEdges.Contains(edgeTuple))
                    {
                        totalWeight += edge.Value;
                        visitedEdges.Add(edgeTuple);
                    }
                }
            }
            return totalWeight;
        }

        #endregion

        [TestFixture]
        public class AdjacencyMatrixTests
        {
            [Test]
            public void Solve_SimpleConnectedGraph_ReturnsMst()
            {
                var graph = new float[,]
                {
                    { Inf, 10, Inf, 30, 100 },
                    { 10, Inf, 50, Inf, Inf },
                    { Inf, 50, Inf, 20, 10 },
                    { 30, Inf, 20, Inf, 60 },
                    { 100, Inf, 10, 60, Inf }
                };
                var expectedMst = new float[,]
                {
                    { Inf, 10, Inf, 30, Inf },
                    { 10, Inf, Inf, Inf, Inf },
                    { Inf, Inf, Inf, 20, 10 },
                    { 30, Inf, 20, Inf, Inf },
                    { Inf, Inf, 10, Inf, Inf }
                };
                float expectedWeight = 70; // 10+10+20+30

                var mst = Kruskal.Solve(graph);

                Assert.That(AreMatricesEqual(expectedMst, mst), Is.True, "MST structure is not as expected.");
                Assert.That(GetMatrixTotalWeight(mst), Is.EqualTo(expectedWeight).Within(1e-6), "MST total weight is incorrect.");
            }

            [Test]
            public void Solve_DisconnectedGraph_ReturnsMsf()
            {
                var graph = new float[,]
                {
                    { Inf, 1, Inf, Inf },
                    { 1, Inf, Inf, Inf },
                    { Inf, Inf, Inf, 2 },
                    { Inf, Inf, 2, Inf }
                };
                var expectedMsf = new float[,]
                {
                    { Inf, 1, Inf, Inf },
                    { 1, Inf, Inf, Inf },
                    { Inf, Inf, Inf, 2 },
                    { Inf, Inf, 2, Inf }
                };
                float expectedWeight = 3; // 1 + 2

                var msf = Kruskal.Solve(graph);

                Assert.That(AreMatricesEqual(expectedMsf, msf), Is.True, "MSF structure is not as expected.");
                Assert.That(GetMatrixTotalWeight(msf), Is.EqualTo(expectedWeight).Within(1e-6), "MSF total weight is incorrect.");
            }

            [Test]
            public void Solve_EmptyGraph_ReturnsEmptyGraph()
            {
                var graph = new float[0, 0];
                var expectedMst = new float[0, 0];

                var mst = Kruskal.Solve(graph);

                Assert.That(AreMatricesEqual(expectedMst, mst), Is.True);
                Assert.That(GetMatrixTotalWeight(mst), Is.EqualTo(0).Within(1e-6));
            }

            [Test]
            public void Solve_SingleNodeGraph_ReturnsSingleNodeGraph()
            {
                var graph = new float[,] { { Inf } };
                var expectedMst = new float[,] { { Inf } };

                var mst = Kruskal.Solve(graph);

                Assert.That(AreMatricesEqual(expectedMst, mst), Is.True);
                Assert.That(GetMatrixTotalWeight(mst), Is.EqualTo(0).Within(1e-6));
            }

            [Test]
            public void Solve_AllEdgesSameWeight_ReturnsValidMst()
            {
                var graph = new float[,]
                {
                    { Inf, 1, 1 },
                    { 1, Inf, 1 },
                    { 1, 1, Inf }
                };
                // Expected MST for K3 with all edge weights 1 (edges (0,1) and (0,2) chosen by this implementation)
                var expectedMst = new float[,]
                {
                    { Inf, 1, 1 },
                    { 1, Inf, Inf },
                    { 1, Inf, Inf }
                };
                float expectedWeight = 2;

                var mst = Kruskal.Solve(graph);

                Assert.That(AreMatricesEqual(expectedMst, mst), Is.True, "MST structure is not as expected for graph with same edge weights.");
                Assert.That(GetMatrixTotalWeight(mst), Is.EqualTo(expectedWeight).Within(1e-6), "MST total weight is incorrect.");
            }

            [Test]
            public void Solve_NonSquareMatrix_ThrowsArgumentException()
            {
                var graph = new float[2, 3];
                Assert.That(() => Kruskal.Solve(graph), Throws.ArgumentException.With.Message.EqualTo("Matrix must be square!"));
            }

            [Test]
            public void Solve_NonSymmetricMatrix_ThrowsArgumentException()
            {
                var graph = new float[,]
                {
                    { Inf, 1 },
                    { 2, Inf }
                };
                Assert.That(() => Kruskal.Solve(graph), Throws.ArgumentException.With.Message.EqualTo("Matrix must be symmetric!"));
            }
        }

        [TestFixture]
        public class AdjacencyListTests
        {
            [Test]
            public void Solve_SimpleConnectedGraph_ReturnsMst()
            {
                var graph = new Dictionary<int, float>[]
                {
                    new() { { 1, 10 }, { 3, 30 }, { 4, 100 } }, // 0
                    new() { { 0, 10 }, { 2, 50 } },            // 1
                    new() { { 1, 50 }, { 3, 20 }, { 4, 10 } }, // 2
                    new() { { 0, 30 }, { 2, 20 }, { 4, 60 } }, // 3
                    new() { { 0, 100 }, { 2, 10 }, { 3, 60 } } // 4
                };
                var expectedMst = new Dictionary<int, float>[]
                {
                    new() { { 1, 10 }, { 3, 30 } },
                    new() { { 0, 10 } },
                    new() { { 4, 10 }, { 3, 20 } }, // Order in dictionary doesn't matter for equality check
                    new() { { 0, 30 }, { 2, 20 } },
                    new() { { 2, 10 } }
                };
                float expectedWeight = 70; // 10+10+20+30

                var mst = Kruskal.Solve(graph);

                Assert.That(AreAdjacencyListsEqual(expectedMst, mst), Is.True, "MST structure is not as expected.");
                Assert.That(GetListTotalWeight(mst), Is.EqualTo(expectedWeight).Within(1e-6), "MST total weight is incorrect.");
            }

            [Test]
            public void Solve_DisconnectedGraph_ReturnsMsf()
            {
                var graph = new Dictionary<int, float>[]
                {
                    new() { { 1, 1 } }, // 0
                    new() { { 0, 1 } }, // 1
                    new() { { 3, 2 } }, // 2
                    new() { { 2, 2 } }  // 3
                };
                var expectedMsf = new Dictionary<int, float>[]
                {
                    new() { { 1, 1 } },
                    new() { { 0, 1 } },
                    new() { { 3, 2 } },
                    new() { { 2, 2 } }
                };
                float expectedWeight = 3; // 1 + 2

                var msf = Kruskal.Solve(graph);

                Assert.That(AreAdjacencyListsEqual(expectedMsf, msf), Is.True, "MSF structure is not as expected.");
                Assert.That(GetListTotalWeight(msf), Is.EqualTo(expectedWeight).Within(1e-6), "MSF total weight is incorrect.");
            }

            [Test]
            public void Solve_EmptyGraph_ReturnsEmptyGraph()
            {
                var graph = new Dictionary<int, float>[0];
                var expectedMst = new Dictionary<int, float>[0];

                var mst = Kruskal.Solve(graph);

                Assert.That(AreAdjacencyListsEqual(expectedMst, mst), Is.True);
                Assert.That(GetListTotalWeight(mst), Is.EqualTo(0).Within(1e-6));
            }

            [Test]
            public void Solve_SingleNodeGraph_ReturnsSingleNodeGraph()
            {
                var graph = new Dictionary<int, float>[] { new Dictionary<int, float>() };
                var expectedMst = new Dictionary<int, float>[] { new Dictionary<int, float>() };

                var mst = Kruskal.Solve(graph);

                Assert.That(AreAdjacencyListsEqual(expectedMst, mst), Is.True);
                Assert.That(GetListTotalWeight(mst), Is.EqualTo(0).Within(1e-6));
            }

            [Test]
            public void Solve_AllEdgesSameWeight_ReturnsValidMst()
            {
                var graph = new Dictionary<int, float>[]
                {
                    new() { { 1, 1 }, { 2, 1 } }, // 0
                    new() { { 0, 1 }, { 2, 1 } }, // 1
                    new() { { 0, 1 }, { 1, 1 } }  // 2
                };

                // Expected MST for K3 with all edge weights 1.
                // The specific edges chosen depend on tie-breaking.
                // Assuming (0,1) and (0,2) are chosen based on processing order.
                var expectedMst = new Dictionary<int, float>[]
                {
                    new() { { 1, 1 }, { 2, 1 } },
                    new() { { 0, 1 } },
                    new() { { 0, 1 } }
                };
                float expectedWeight = 2;

                var mst = Kruskal.Solve(graph);

                Assert.That(AreAdjacencyListsEqual(expectedMst, mst), Is.True, "MST structure is not as expected for graph with same edge weights.");
                Assert.That(GetListTotalWeight(mst), Is.EqualTo(expectedWeight).Within(1e-6), "MST total weight is incorrect.");
            }

            [Test]
            public void Solve_DirectedGraph_MissingReverseEdge_ThrowsArgumentException()
            {
                var graph = new Dictionary<int, float>[]
                {
                    new() { { 1, 1 } }, // 0 -> 1
                    new() { }           // 1 (missing 1 -> 0)
                };

                Assert.That(() => Kruskal.Solve(graph), Throws.ArgumentException.With.Message.EqualTo("Graph must be undirected!"));
            }

            [Test]
            public void Solve_DirectedGraph_DifferentWeights_ThrowsArgumentException()
            {
                var graph = new Dictionary<int, float>[]
                {
                    new() { { 1, 1 } }, // 0 -> 1 (weight 1)
                    new() { { 0, 2 } }  // 1 -> 0 (weight 2)
                };

                Assert.That(() => Kruskal.Solve(graph), Throws.ArgumentException.With.Message.EqualTo("Graph must be undirected!"));
            }

            [Test]
            public void Solve_GraphWithSelfLoopsInAdjacencyList_ProcessesCorrectly()
            {
                // Kruskal's algorithm typically ignores self-loops or assumes they are not present
                // or have infinite weight. The current implementation's ValidateGraph for AdjacencyList
                // might not explicitly forbid adj[i][i], but the MST construction should ignore them
                // as FindSet(nodes[i]) == FindSet(nodes[i]) is true.
                var graph = new Dictionary<int, float>[]
                {
                    new() { { 0, 5 }, { 1, 10 } }, // Node 0 with self-loop and edge to 1
                    new() { { 0, 10 }, { 1, 8 } }  // Node 1 with self-loop and edge to 0
                };

                var expectedMst = new Dictionary<int, float>[]
                {
                    new() { { 1, 10 } },
                    new() { { 0, 10 } }
                };
                float expectedWeight = 10;

                var mst = Kruskal.Solve(graph);

                Assert.That(AreAdjacencyListsEqual(expectedMst, mst), Is.True, "MST structure is not as expected with self-loops.");
                Assert.That(GetListTotalWeight(mst), Is.EqualTo(expectedWeight).Within(1e-6), "MST total weight is incorrect with self-loops.");
            }
        }
    }
}
