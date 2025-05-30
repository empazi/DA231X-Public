using NUnit.Framework;
using Algorithms.Graph.MinimumSpanningTree; // For Kruskal class
using System; // For ArgumentException, float constants
using System.Collections.Generic; // For Dictionary
using System.Linq; // For Enumerable.Select

namespace Algorithms.Tests.Graph.MinimumSpanningTree
{
    [TestFixture]
    public class KruskalTests
    {
        private const float Inf = float.PositiveInfinity;
        private const double Delta = 1e-6; // Used for float comparisons, will be cast to float

        #region Helper Methods for Calculations (Unaffected by NUnit version)

        private float CalculateTotalWeight(float[,] mst)
        {
            float totalWeight = 0;
            int numNodes = mst.GetLength(0);
            for (int i = 0; i < numNodes; i++)
            {
                for (int j = i + 1; j < numNodes; j++) // Iterate only upper triangle
                {
                    if (float.IsFinite(mst[i, j]) && mst[i, j] != Inf)
                    {
                        totalWeight += mst[i, j];
                    }
                }
            }
            return totalWeight;
        }

        private int CountEdges(float[,] mst)
        {
            int edgeCount = 0;
            int numNodes = mst.GetLength(0);
            for (int i = 0; i < numNodes; i++)
            {
                for (int j = i + 1; j < numNodes; j++) // Iterate only upper triangle
                {
                    if (float.IsFinite(mst[i, j]) && mst[i, j] != Inf)
                    {
                        edgeCount++;
                    }
                }
            }
            return edgeCount;
        }

        private float CalculateTotalWeight(Dictionary<int, float>[] mst)
        {
            float totalWeight = 0;
            var visitedEdges = new HashSet<(int, int)>();
            for (int i = 0; i < mst.Length; i++)
            {
                if (mst[i] == null) continue;
                foreach (var edge in mst[i])
                {
                    var node1 = Math.Min(i, edge.Key);
                    var node2 = Math.Max(i, edge.Key);
                    if (!visitedEdges.Contains((node1, node2)))
                    {
                        totalWeight += edge.Value;
                        visitedEdges.Add((node1, node2));
                    }
                }
            }
            return totalWeight;
        }

        private int CountEdges(Dictionary<int, float>[] mst)
        {
            int edgeCount = 0;
            var visitedEdges = new HashSet<(int, int)>();
            for (int i = 0; i < mst.Length; i++)
            {
                if (mst[i] == null) continue;
                foreach (var edge in mst[i])
                {
                    var node1 = Math.Min(i, edge.Key);
                    var node2 = Math.Max(i, edge.Key);
                    if (!visitedEdges.Contains((node1, node2)))
                    {
                        edgeCount++;
                        visitedEdges.Add((node1, node2));
                    }
                }
            }
            return edgeCount;
        }

        #endregion

        #region Helper Methods for Assertions (Updated to Constraint Model)

        private void AssertMatricesAreEqual(float[,] expected, float[,] actual)
        {
            Assert.That(actual.GetLength(0), Is.EqualTo(expected.GetLength(0)), "Matrix row count mismatch.");
            Assert.That(actual.GetLength(1), Is.EqualTo(expected.GetLength(1)), "Matrix column count mismatch.");
            for (int i = 0; i < expected.GetLength(0); i++)
            {
                for (int j = 0; j < expected.GetLength(1); j++)
                {
                    string message = $"Mismatch at [{i},{j}].";
                    if (float.IsPositiveInfinity(expected[i, j]))
                        Assert.That(float.IsPositiveInfinity(actual[i, j]), Is.True, $"{message} Expected PositiveInfinity, got {actual[i, j]}.");
                    else if (float.IsNegativeInfinity(expected[i, j]))
                        Assert.That(float.IsNegativeInfinity(actual[i, j]), Is.True, $"{message} Expected NegativeInfinity, got {actual[i, j]}.");
                    else if (float.IsNaN(expected[i, j]))
                        Assert.That(float.IsNaN(actual[i, j]), Is.True, $"{message} Expected NaN, got {actual[i, j]}.");
                    else
                        Assert.That(actual[i, j], Is.EqualTo(expected[i, j]).Within((float)Delta), $"{message}");
                }
            }
        }

        private void AssertAdjacencyListsAreEqual(Dictionary<int, float>[] expected, Dictionary<int, float>[] actual)
        {
            Assert.That(actual.Length, Is.EqualTo(expected.Length), "Adjacency list array length mismatch.");
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.That(actual[i], Is.Not.Null, $"Actual adjacency list for node {i} is null.");
                Assert.That(actual[i].Count, Is.EqualTo(expected[i].Count),
                    $"Edge count mismatch for node {i}. " +
                    $"Expected: {string.Join(",", expected[i].Select(kv => $"({kv.Key},{kv.Value})"))}, " +
                    $"Actual: {string.Join(",", actual[i].Select(kv => $"({kv.Key},{kv.Value})"))}");

                foreach (var edge in expected[i])
                {
                    Assert.That(actual[i].ContainsKey(edge.Key), Is.True, $"Node {i}: Missing edge to {edge.Key} in actual MST.");
                    Assert.That(actual[i][edge.Key], Is.EqualTo(edge.Value).Within((float)Delta), $"Node {i}: Weight mismatch for edge to {edge.Key}.");
                }
            }
        }

        #endregion

        #region Tests for Solve(float[,] adjacencyMatrix) (Updated to Constraint Model)

        [Test]
        public void Solve_Matrix_EmptyGraph_ReturnsEmptyMatrix()
        {
            var graph = new float[0, 0];
            var expectedMst = new float[0, 0];

            var actualMst = Kruskal.Solve(graph);

            AssertMatricesAreEqual(expectedMst, actualMst);
            Assert.That(CalculateTotalWeight(actualMst), Is.EqualTo(0f).Within((float)Delta));
            Assert.That(CountEdges(actualMst), Is.EqualTo(0));
        }

        [Test]
        public void Solve_Matrix_SingleNodeGraph_ReturnsSingleNodeMatrix()
        {
            var graph = new float[,] { { Inf } };
            var expectedMst = new float[,] { { Inf } };

            var actualMst = Kruskal.Solve(graph);

            AssertMatricesAreEqual(expectedMst, actualMst);
            Assert.That(CalculateTotalWeight(actualMst), Is.EqualTo(0f).Within((float)Delta));
            Assert.That(CountEdges(actualMst), Is.EqualTo(0));
        }

        [Test]
        public void Solve_Matrix_SimpleConnectedGraph_ReturnsCorrectMst()
        {
            var graph = new float[,]
            {
                { Inf, 1, 3 },
                { 1, Inf, 2 },
                { 3, 2, Inf }
            };
            var expectedMst = new float[,]
            {
                { Inf, 1, Inf },
                { 1, Inf, 2 },
                { Inf, 2, Inf }
            };

            var actualMst = Kruskal.Solve(graph);

            AssertMatricesAreEqual(expectedMst, actualMst);
            Assert.That(CalculateTotalWeight(actualMst), Is.EqualTo(3f).Within((float)Delta));
            Assert.That(CountEdges(actualMst), Is.EqualTo(2));
        }

        [Test]
        public void Solve_Matrix_KentStateExampleGraph_ReturnsCorrectMst()
        {
            var graph = new float[,]
            {
                { Inf, 10,  20,  Inf, Inf },
                { 10,  Inf, Inf, 30,  25 },
                { 20,  Inf, Inf, 15,  Inf },
                { Inf, 30,  15,  Inf, 5 },
                { Inf, 25,  Inf, 5,   Inf }
            };
            var expectedMst = new float[,]
            {
                { Inf, 10,  20,  Inf, Inf },
                { 10,  Inf, Inf, Inf, Inf },
                { 20,  Inf, Inf, 15,  Inf },
                { Inf, Inf, 15,  Inf, 5 },
                { Inf, Inf, Inf, 5,   Inf }
            };

            var actualMst = Kruskal.Solve(graph);

            AssertMatricesAreEqual(expectedMst, actualMst);
            Assert.That(CalculateTotalWeight(actualMst), Is.EqualTo(50f).Within((float)Delta));
            Assert.That(CountEdges(actualMst), Is.EqualTo(4));
        }

        [Test]
        public void Solve_Matrix_DisconnectedGraph_ReturnsCorrectMsf()
        {
            var graph = new float[,]
            {
                { Inf, 1, Inf, Inf },
                { 1, Inf, Inf, Inf },
                { Inf, Inf, Inf, 5 },
                { Inf, Inf, 5, Inf }
            };
            var expectedMsf = new float[,]
            {
                { Inf, 1, Inf, Inf },
                { 1, Inf, Inf, Inf },
                { Inf, Inf, Inf, 5 },
                { Inf, Inf, 5, Inf }
            };

            var actualMst = Kruskal.Solve(graph);

            AssertMatricesAreEqual(expectedMsf, actualMst);
            Assert.That(CalculateTotalWeight(actualMst), Is.EqualTo(6f).Within((float)Delta));
            Assert.That(CountEdges(actualMst), Is.EqualTo(2));
        }

        [Test]
        public void Solve_Matrix_GraphWithNoEdges_ReturnsCorrectMsf()
        {
            var graph = new float[,]
            {
                { Inf, Inf },
                { Inf, Inf }
            };
            var expectedMsf = new float[,]
            {
                { Inf, Inf },
                { Inf, Inf }
            };

            var actualMst = Kruskal.Solve(graph);

            AssertMatricesAreEqual(expectedMsf, actualMst);
            Assert.That(CalculateTotalWeight(actualMst), Is.EqualTo(0f).Within((float)Delta));
            Assert.That(CountEdges(actualMst), Is.EqualTo(0));
        }


        [Test]
        public void Solve_Matrix_AllEdgesSameWeight_ReturnsValidMst()
        {
            var graph = new float[,]
            {
                { Inf, 1, 1 },
                { 1, Inf, 1 },
                { 1, 1, Inf }
            };

            var actualMst = Kruskal.Solve(graph);

            Assert.That(CalculateTotalWeight(actualMst), Is.EqualTo(2f).Within((float)Delta));
            Assert.That(CountEdges(actualMst), Is.EqualTo(2));
        }

        [Test]
        public void ValidateGraph_Matrix_NonSquare_ThrowsArgumentException()
        {
            var graph = new float[,]
            {
                { Inf, 1 }
            }; // 1x2 matrix
            Assert.Throws<ArgumentException>(() => Kruskal.Solve(graph));
            // Alternative constraint model syntax:
            // Assert.That(() => Kruskal.Solve(graph), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ValidateGraph_Matrix_NonSymmetric_ThrowsArgumentException()
        {
            var graph = new float[,]
            {
                { Inf, 1 },
                { 2, Inf } // Not symmetric: graph[0,1] != graph[1,0]
            };
            Assert.Throws<ArgumentException>(() => Kruskal.Solve(graph));
        }

        #endregion

        #region Tests for Solve(Dictionary<int, float>[] adjacencyList) (Updated to Constraint Model)

        [Test]
        public void Solve_List_EmptyGraph_ReturnsEmptyListArray()
        {
            var graph = new Dictionary<int, float>[0];
            var expectedMst = new Dictionary<int, float>[0];

            var actualMst = Kruskal.Solve(graph);

            AssertAdjacencyListsAreEqual(expectedMst, actualMst);
            Assert.That(CalculateTotalWeight(actualMst), Is.EqualTo(0f).Within((float)Delta));
            Assert.That(CountEdges(actualMst), Is.EqualTo(0));
        }

        [Test]
        public void Solve_List_SingleNodeGraph_ReturnsSingleNodeListArray()
        {
            var graph = new Dictionary<int, float>[1];
            graph[0] = new Dictionary<int, float>();

            var expectedMst = new Dictionary<int, float>[1];
            expectedMst[0] = new Dictionary<int, float>();

            var actualMst = Kruskal.Solve(graph);

            AssertAdjacencyListsAreEqual(expectedMst, actualMst);
            Assert.That(CalculateTotalWeight(actualMst), Is.EqualTo(0f).Within((float)Delta));
            Assert.That(CountEdges(actualMst), Is.EqualTo(0));
        }

        [Test]
        public void Solve_List_SimpleConnectedGraph_ReturnsCorrectMst()
        {
            var graph = new Dictionary<int, float>[3];
            graph[0] = new Dictionary<int, float> { { 1, 1f }, { 2, 3f } };
            graph[1] = new Dictionary<int, float> { { 0, 1f }, { 2, 2f } };
            graph[2] = new Dictionary<int, float> { { 0, 3f }, { 1, 2f } };

            var expectedMst = new Dictionary<int, float>[3];
            expectedMst[0] = new Dictionary<int, float> { { 1, 1f } };
            expectedMst[1] = new Dictionary<int, float> { { 0, 1f }, { 2, 2f } };
            expectedMst[2] = new Dictionary<int, float> { { 1, 2f } };

            var actualMst = Kruskal.Solve(graph);

            AssertAdjacencyListsAreEqual(expectedMst, actualMst);
            Assert.That(CalculateTotalWeight(actualMst), Is.EqualTo(3f).Within((float)Delta));
            Assert.That(CountEdges(actualMst), Is.EqualTo(2));
        }

        [Test]
        public void Solve_List_KentStateExampleGraph_ReturnsCorrectMst()
        {
            var graph = new Dictionary<int, float>[5];
            graph[0] = new Dictionary<int, float> { { 1, 10f }, { 2, 20f } };
            graph[1] = new Dictionary<int, float> { { 0, 10f }, { 3, 30f }, { 4, 25f } };
            graph[2] = new Dictionary<int, float> { { 0, 20f }, { 3, 15f } };
            graph[3] = new Dictionary<int, float> { { 1, 30f }, { 2, 15f }, { 4, 5f } };
            graph[4] = new Dictionary<int, float> { { 1, 25f }, { 3, 5f } };

            var expectedMst = new Dictionary<int, float>[5];
            expectedMst[0] = new Dictionary<int, float> { { 1, 10f }, { 2, 20f } };
            expectedMst[1] = new Dictionary<int, float> { { 0, 10f } };
            expectedMst[2] = new Dictionary<int, float> { { 0, 20f }, { 3, 15f } };
            expectedMst[3] = new Dictionary<int, float> { { 2, 15f }, { 4, 5f } };
            expectedMst[4] = new Dictionary<int, float> { { 3, 5f } };

            var actualMst = Kruskal.Solve(graph);

            AssertAdjacencyListsAreEqual(expectedMst, actualMst);
            Assert.That(CalculateTotalWeight(actualMst), Is.EqualTo(50f).Within((float)Delta));
            Assert.That(CountEdges(actualMst), Is.EqualTo(4));
        }

        [Test]
        public void Solve_List_DisconnectedGraph_ReturnsCorrectMsf()
        {
            var graph = new Dictionary<int, float>[4];
            graph[0] = new Dictionary<int, float> { { 1, 1f } };
            graph[1] = new Dictionary<int, float> { { 0, 1f } };
            graph[2] = new Dictionary<int, float> { { 3, 5f } };
            graph[3] = new Dictionary<int, float> { { 2, 5f } };

            var expectedMsf = new Dictionary<int, float>[4];
            expectedMsf[0] = new Dictionary<int, float> { { 1, 1f } };
            expectedMsf[1] = new Dictionary<int, float> { { 0, 1f } };
            expectedMsf[2] = new Dictionary<int, float> { { 3, 5f } };
            expectedMsf[3] = new Dictionary<int, float> { { 2, 5f } };

            var actualMst = Kruskal.Solve(graph);

            AssertAdjacencyListsAreEqual(expectedMsf, actualMst);
            Assert.That(CalculateTotalWeight(actualMst), Is.EqualTo(6f).Within((float)Delta));
            Assert.That(CountEdges(actualMst), Is.EqualTo(2));
        }

        [Test]
        public void Solve_List_GraphWithNoEdges_ReturnsCorrectMsf()
        {
            var graph = new Dictionary<int, float>[2];
            graph[0] = new Dictionary<int, float>();
            graph[1] = new Dictionary<int, float>();

            var expectedMsf = new Dictionary<int, float>[2];
            expectedMsf[0] = new Dictionary<int, float>();
            expectedMsf[1] = new Dictionary<int, float>();

            var actualMst = Kruskal.Solve(graph);

            AssertAdjacencyListsAreEqual(expectedMsf, actualMst);
            Assert.That(CalculateTotalWeight(actualMst), Is.EqualTo(0f).Within((float)Delta));
            Assert.That(CountEdges(actualMst), Is.EqualTo(0));
        }

        [Test]
        public void Solve_List_AllEdgesSameWeight_ReturnsValidMst()
        {
            var graph = new Dictionary<int, float>[3];
            graph[0] = new Dictionary<int, float> { { 1, 1f }, { 2, 1f } };
            graph[1] = new Dictionary<int, float> { { 0, 1f }, { 2, 1f } };
            graph[2] = new Dictionary<int, float> { { 0, 1f }, { 1, 1f } };

            var actualMst = Kruskal.Solve(graph);

            Assert.That(CalculateTotalWeight(actualMst), Is.EqualTo(2f).Within((float)Delta));
            Assert.That(CountEdges(actualMst), Is.EqualTo(2));
        }

        [Test]
        public void ValidateGraph_List_NonUndirected_MissingReverseEdge_ThrowsArgumentException()
        {
            var graph = new Dictionary<int, float>[2];
            graph[0] = new Dictionary<int, float> { { 1, 1f } };
            graph[1] = new Dictionary<int, float>(); // Missing edge from 1 to 0

            Assert.Throws<ArgumentException>(() => Kruskal.Solve(graph));
        }

        [Test]
        public void ValidateGraph_List_NonUndirected_DifferentWeights_ThrowsArgumentException()
        {
            var graph = new Dictionary<int, float>[2];
            graph[0] = new Dictionary<int, float> { { 1, 1f } };
            graph[1] = new Dictionary<int, float> { { 0, 2f } }; // Different weight for 1 to 0

            Assert.Throws<ArgumentException>(() => Kruskal.Solve(graph));
        }

        [Test]
        public void ValidateGraph_List_NonUndirected_ReferenceToNonExistentNode_ThrowsArgumentException()
        {
            var graph = new Dictionary<int, float>[2];
            graph[0] = new Dictionary<int, float> { { 1, 1f } };
            graph[1] = new Dictionary<int, float> { /* no key 0 */ };

            Assert.Throws<ArgumentException>(() => Kruskal.Solve(graph));
        }

        #endregion
    }
}
