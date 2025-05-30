using NUnit.Framework;
using DataStructures.Graph; // Assumes Vertex<T> and IDirectedWeightedGraph<T> are in this namespace
using System;
using System.Linq;
using System.Collections.Generic;

namespace DataStructures.Tests.Graph
{
    [TestFixture]
    public class DirectedWeightedGraphTests
    {
        // Helper to create a vertex associated with a different graph instance,
        // useful for testing "vertex not in graph" scenarios.
        private Vertex<T> CreateForeignVertex<T>(DirectedWeightedGraph<T> owningGraph, T data)
        {
            var anotherGraph = new DirectedWeightedGraph<T>(1); // Create a distinct graph instance
            // Ensure it's truly a different instance, though with new it always will be.
            if (ReferenceEquals(owningGraph, anotherGraph))
            {
                // This case should ideally not happen with `new` but as a safeguard:
                anotherGraph = new DirectedWeightedGraph<T>(2);
            }
            return anotherGraph.AddVertex(data);
        }

        // Constructor Tests
        [Test]
        public void Constructor_WithNegativeCapacity_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => new DirectedWeightedGraph<int>(-1),
                "Graph capacity should always be a non-negative integer.");
        }

        [Test]
        public void Constructor_WithZeroCapacity_InitializesCorrectly()
        {
            var graph = new DirectedWeightedGraph<int>(0);
            Assert.That(graph.Count, Is.EqualTo(0));
            Assert.That(graph.Vertices, Is.Empty); // Vertices array should be of size 0
        }

        [Test]
        public void Constructor_WithPositiveCapacity_InitializesCorrectly()
        {
            const int capacity = 5;
            var graph = new DirectedWeightedGraph<int>(capacity);
            Assert.That(graph.Count, Is.EqualTo(0));
            Assert.That(graph.Vertices.Length, Is.EqualTo(capacity));
            Assert.That(graph.Vertices.All(v => v == null), Is.True);
        }

        // AddVertex Tests
        [Test]
        public void AddVertex_ToEmptyGraph_AddsVertexAndIncrementsCount()
        {
            var graph = new DirectedWeightedGraph<int>(5);
            var vertex = graph.AddVertex(10);

            Assert.Multiple(() =>
            {
                Assert.That(graph.Count, Is.EqualTo(1));
                Assert.That(graph.Vertices[0], Is.SameAs(vertex));
                Assert.That(vertex.Data, Is.EqualTo(10));
                Assert.That(vertex.Index, Is.EqualTo(0));
                Assert.That(vertex.Graph, Is.SameAs(graph));
            });
        }

        [Test]
        public void AddVertex_ToNonEmptyGraph_AddsVertexAndIncrementsCount()
        {
            var graph = new DirectedWeightedGraph<int>(5);
            graph.AddVertex(10); // v0
            var vertex2 = graph.AddVertex(20); // v1

            Assert.Multiple(() =>
            {
                Assert.That(graph.Count, Is.EqualTo(2));
                Assert.That(graph.Vertices[1], Is.SameAs(vertex2));
                Assert.That(vertex2.Data, Is.EqualTo(20));
                Assert.That(vertex2.Index, Is.EqualTo(1));
                Assert.That(vertex2.Graph, Is.SameAs(graph));
            });
        }

        [Test]
        public void AddVertex_WhenGraphIsFull_ThrowsInvalidOperationException()
        {
            var graph = new DirectedWeightedGraph<int>(1);
            graph.AddVertex(10);
            Assert.Throws<InvalidOperationException>(() => graph.AddVertex(20), "Graph overflow.");
        }

        [Test]
        public void AddVertex_ToGraphWithZeroCapacity_ThrowsInvalidOperationException()
        {
            var graph = new DirectedWeightedGraph<int>(0);
            Assert.Throws<InvalidOperationException>(() => graph.AddVertex(10), "Graph overflow.");
        }

        // AddEdge Tests
        [Test]
        public void AddEdge_ValidVerticesAndWeight_AddsEdgeSuccessfully()
        {
            var graph = new DirectedWeightedGraph<int>(2);
            var v1 = graph.AddVertex(1);
            var v2 = graph.AddVertex(2);

            graph.AddEdge(v1, v2, 1.5);

            Assert.That(graph.AreAdjacent(v1, v2), Is.True);
            Assert.That(graph.AdjacentDistance(v1, v2), Is.EqualTo(1.5));
        }

        [Test]
        public void AddEdge_StartVertexNotInGraph_ThrowsInvalidOperationException()
        {
            var graph = new DirectedWeightedGraph<int>(2);
            var v1 = graph.AddVertex(1); // Belongs to 'graph'
            var foreignVertex = CreateForeignVertex(graph, 99); // Belongs to a different graph

            Assert.Throws<InvalidOperationException>(() => graph.AddEdge(foreignVertex, v1, 1.0),
                $"Vertex does not belong to graph: {foreignVertex}.");
        }

        [Test]
        public void AddEdge_EndVertexNotInGraph_ThrowsInvalidOperationException()
        {
            var graph = new DirectedWeightedGraph<int>(2);
            var v1 = graph.AddVertex(1);
            var foreignVertex = CreateForeignVertex(graph, 99);

            Assert.Throws<InvalidOperationException>(() => graph.AddEdge(v1, foreignVertex, 1.0),
                $"Vertex does not belong to graph: {foreignVertex}.");
        }

        [Test]
        public void AddEdge_WithZeroWeight_ThrowsInvalidOperationException()
        {
            var graph = new DirectedWeightedGraph<int>(2);
            var v1 = graph.AddVertex(1);
            var v2 = graph.AddVertex(2);

            Assert.Throws<InvalidOperationException>(() => graph.AddEdge(v1, v2, 0.0),
                "Edge weight cannot be zero.");
        }

        [Test]
        public void AddEdge_EdgeAlreadyExists_ThrowsInvalidOperationException()
        {
            var graph = new DirectedWeightedGraph<int>(2);
            var v1 = graph.AddVertex(1);
            var v2 = graph.AddVertex(2);
            graph.AddEdge(v1, v2, 1.5);

            Assert.Throws<InvalidOperationException>(() => graph.AddEdge(v1, v2, 2.0),
                $"Vertex already exists: {1.5}"); // Message includes existing weight
        }

        // RemoveVertex Tests
        [Test]
        public void RemoveVertex_ExistingVertex_RemovesVertexAndDecrementsCount()
        {
            var graph = new DirectedWeightedGraph<int>(3);
            var v0 = graph.AddVertex(0);
            var v1 = graph.AddVertex(1);
            var v2Original = graph.AddVertex(2); // Will be shifted

            graph.RemoveVertex(v1);

            Assert.Multiple(() =>
            {
                Assert.That(graph.Count, Is.EqualTo(2));
                Assert.That(v1.Index, Is.EqualTo(-1), "Removed vertex index should be -1.");
                Assert.That(v1.Graph, Is.Null, "Removed vertex graph ref should be null.");
                Assert.That(graph.Vertices.Contains(v1), Is.False, "Vertices array should not contain removed vertex.");
                Assert.That(graph.Vertices[0], Is.SameAs(v0), "v0 should be unaffected at index 0.");
                Assert.That(graph.Vertices[1], Is.SameAs(v2Original), "v2Original should now be at index 1.");
                Assert.That(graph.Vertices[2], Is.Null, "Last element in Vertices array should be null.");
            });
        }

        [Test]
        public void RemoveVertex_UpdatesIndicesOfSubsequentVertices()
        {
            var graph = new DirectedWeightedGraph<int>(3);
            var v0 = graph.AddVertex(0);
            var v1 = graph.AddVertex(1); // Vertex to remove
            var v2 = graph.AddVertex(2);

            graph.RemoveVertex(v1);

            Assert.Multiple(() =>
            {
                Assert.That(v0.Index, Is.EqualTo(0));
                Assert.That(v2.Index, Is.EqualTo(1), "v2's index should be updated from 2 to 1.");
            });
        }

        [Test]
        public void RemoveVertex_VertexNotInGraph_ThrowsInvalidOperationException()
        {
            var graph = new DirectedWeightedGraph<int>(1);
            graph.AddVertex(1);
            var foreignVertex = CreateForeignVertex(graph, 99);

            Assert.Throws<InvalidOperationException>(() => graph.RemoveVertex(foreignVertex),
                $"Vertex does not belong to graph: {foreignVertex}.");
        }

        [Test]
        public void RemoveVertex_FromGraphWithOneVertex_GraphBecomesEmpty()
        {
            var graph = new DirectedWeightedGraph<int>(1);
            var v0 = graph.AddVertex(0);

            graph.RemoveVertex(v0);

            Assert.Multiple(() =>
            {
                Assert.That(graph.Count, Is.EqualTo(0));
                Assert.That(v0.Index, Is.EqualTo(-1));
                Assert.That(v0.Graph, Is.Null);
                Assert.That(graph.Vertices[0], Is.Null);
            });
        }

        [Test]
        public void RemoveVertex_UpdatesAdjacencyMatrixCorrectly_RemoveMiddleVertex()
        {
            var graph = new DirectedWeightedGraph<int>(3);
            var v0 = graph.AddVertex(0);
            var v1 = graph.AddVertex(1); // Vertex to remove
            var v2 = graph.AddVertex(2);

            graph.AddEdge(v0, v1, 1.0); graph.AddEdge(v0, v2, 2.0);
            graph.AddEdge(v1, v0, 3.0); graph.AddEdge(v1, v2, 4.0);
            graph.AddEdge(v2, v0, 5.0); graph.AddEdge(v2, v1, 6.0);

            graph.RemoveVertex(v1); // v1 was at index 1

            Assert.Multiple(() =>
            {
                Assert.That(graph.Count, Is.EqualTo(2));
                Assert.That(v0.Index, Is.EqualTo(0));
                Assert.That(v2.Index, Is.EqualTo(1)); // v2 shifted from index 2 to 1

                // Check remaining edges (v0,v2) and (v2,v0)
                Assert.That(graph.AreAdjacent(v0, v2), Is.True, "Edge v0->v2 should exist.");
                Assert.That(graph.AdjacentDistance(v0, v2), Is.EqualTo(2.0));
                Assert.That(graph.AreAdjacent(v2, v0), Is.True, "Edge v2->v0 should exist.");
                Assert.That(graph.AdjacentDistance(v2, v0), Is.EqualTo(5.0));

                // Check that other potential edges are not present
                Assert.That(graph.AreAdjacent(v0, v0), Is.False);
                Assert.That(graph.AreAdjacent(v2, v2), Is.False);
            });
        }

        [Test]
        public void RemoveVertex_UpdatesAdjacencyMatrixCorrectly_RemoveFirstVertex()
        {
            var graph = new DirectedWeightedGraph<int>(3);
            var v0 = graph.AddVertex(0); // Vertex to remove
            var v1 = graph.AddVertex(1);
            var v2 = graph.AddVertex(2);

            graph.AddEdge(v0, v1, 1.0); graph.AddEdge(v1, v2, 2.0); graph.AddEdge(v2, v0, 3.0);

            graph.RemoveVertex(v0); // v0 was at index 0

            Assert.Multiple(() =>
            {
                Assert.That(graph.Count, Is.EqualTo(2));
                Assert.That(v1.Index, Is.EqualTo(0)); // v1 shifted from index 1 to 0
                Assert.That(v2.Index, Is.EqualTo(1)); // v2 shifted from index 2 to 1

                // Check remaining edge (v1,v2)
                Assert.That(graph.AreAdjacent(v1, v2), Is.True, "Edge v1->v2 should exist.");
                Assert.That(graph.AdjacentDistance(v1, v2), Is.EqualTo(2.0));

                Assert.That(graph.AreAdjacent(v1, v1), Is.False);
                Assert.That(graph.AreAdjacent(v2, v1), Is.False);
                Assert.That(graph.AreAdjacent(v2, v2), Is.False);
            });
        }

        [Test]
        public void RemoveVertex_UpdatesAdjacencyMatrixCorrectly_RemoveLastVertex()
        {
            var graph = new DirectedWeightedGraph<int>(3);
            var v0 = graph.AddVertex(0);
            var v1 = graph.AddVertex(1);
            var v2 = graph.AddVertex(2); // Vertex to remove

            graph.AddEdge(v0, v1, 1.0); graph.AddEdge(v1, v2, 2.0); graph.AddEdge(v2, v0, 3.0);

            graph.RemoveVertex(v2); // v2 was at index 2

            Assert.Multiple(() =>
            {
                Assert.That(graph.Count, Is.EqualTo(2));
                Assert.That(v0.Index, Is.EqualTo(0));
                Assert.That(v1.Index, Is.EqualTo(1));

                // Check remaining edge (v0,v1)
                Assert.That(graph.AreAdjacent(v0, v1), Is.True, "Edge v0->v1 should exist.");
                Assert.That(graph.AdjacentDistance(v0, v1), Is.EqualTo(1.0));

                Assert.That(graph.AreAdjacent(v0, v0), Is.False);
                Assert.That(graph.AreAdjacent(v1, v0), Is.False);
                Assert.That(graph.AreAdjacent(v1, v1), Is.False);
            });
        }

        // RemoveEdge Tests
        [Test]
        public void RemoveEdge_ExistingEdge_RemovesEdgeSuccessfully()
        {
            var graph = new DirectedWeightedGraph<int>(2);
            var v1 = graph.AddVertex(1);
            var v2 = graph.AddVertex(2);
            graph.AddEdge(v1, v2, 1.5);

            graph.RemoveEdge(v1, v2);

            Assert.That(graph.AreAdjacent(v1, v2), Is.False);
            Assert.That(graph.AdjacentDistance(v1, v2), Is.EqualTo(0.0));
        }

        [Test]
        public void RemoveEdge_NonExistingEdge_DoesNotThrowAndMatrixRemainsZero()
        {
            var graph = new DirectedWeightedGraph<int>(2);
            var v1 = graph.AddVertex(1);
            var v2 = graph.AddVertex(2);

            Assert.DoesNotThrow(() => graph.RemoveEdge(v1, v2));
            Assert.That(graph.AreAdjacent(v1, v2), Is.False);
        }

        [Test]
        public void RemoveEdge_StartVertexNotInGraph_ThrowsInvalidOperationException()
        {
            var graph = new DirectedWeightedGraph<int>(1);
            var v1 = graph.AddVertex(1);
            var foreignVertex = CreateForeignVertex(graph, 99);

            Assert.Throws<InvalidOperationException>(() => graph.RemoveEdge(foreignVertex, v1));
        }

        [Test]
        public void RemoveEdge_EndVertexNotInGraph_ThrowsInvalidOperationException()
        {
            var graph = new DirectedWeightedGraph<int>(1);
            var v1 = graph.AddVertex(1);
            var foreignVertex = CreateForeignVertex(graph, 99);

            Assert.Throws<InvalidOperationException>(() => graph.RemoveEdge(v1, foreignVertex));
        }

        // GetNeighbors Tests
        [Test]
        public void GetNeighbors_VertexWithNeighbors_ReturnsCorrectNeighbors()
        {
            var graph = new DirectedWeightedGraph<int>(3);
            var v0 = graph.AddVertex(0);
            var v1 = graph.AddVertex(1);
            var v2 = graph.AddVertex(2);

            graph.AddEdge(v0, v1, 1.0);
            graph.AddEdge(v0, v2, 2.0);
            graph.AddEdge(v1, v2, 3.0); // Edge not from v0

            var neighbors = graph.GetNeighbors(v0).ToList();

            Assert.That(neighbors.Count, Is.EqualTo(2));
            Assert.That(neighbors, Does.Contain(v1));
            Assert.That(neighbors, Does.Contain(v2));
        }

        [Test]
        public void GetNeighbors_VertexWithNoOutgoingEdges_ReturnsEmptyCollection()
        {
            var graph = new DirectedWeightedGraph<int>(2);
            var v0 = graph.AddVertex(0);
            graph.AddVertex(1); // v1, no edge from v0

            var neighbors = graph.GetNeighbors(v0).ToList();
            Assert.That(neighbors, Is.Empty);
        }

        [Test]
        public void GetNeighbors_VertexWithSelfLoop_ReturnsSelfAsNeighbor()
        {
            var graph = new DirectedWeightedGraph<int>(1);
            var v0 = graph.AddVertex(0);
            graph.AddEdge(v0, v0, 1.0);

            var neighbors = graph.GetNeighbors(v0).ToList();

            Assert.That(neighbors.Count, Is.EqualTo(1));
            Assert.That(neighbors, Does.Contain(v0));
        }

        [Test]
        public void GetNeighbors_VertexNotInGraph_ThrowsInvalidOperationException()
        {
            var graph = new DirectedWeightedGraph<int>(1);
            graph.AddVertex(0);
            var foreignVertex = CreateForeignVertex(graph, 99);

            Assert.Throws<InvalidOperationException>(() => graph.GetNeighbors(foreignVertex).ToList());
        }

        // AreAdjacent Tests
        [Test]
        public void AreAdjacent_AdjacentVertices_ReturnsTrue()
        {
            var graph = new DirectedWeightedGraph<int>(2);
            var v1 = graph.AddVertex(1);
            var v2 = graph.AddVertex(2);
            graph.AddEdge(v1, v2, 1.0);

            Assert.That(graph.AreAdjacent(v1, v2), Is.True);
        }

        [Test]
        public void AreAdjacent_NonAdjacentVertices_ReturnsFalse()
        {
            var graph = new DirectedWeightedGraph<int>(2);
            var v1 = graph.AddVertex(1);
            var v2 = graph.AddVertex(2);

            Assert.That(graph.AreAdjacent(v1, v2), Is.False);
            graph.AddEdge(v2, v1, 1.0); // Add edge in reverse, v1->v2 should still be false
            Assert.That(graph.AreAdjacent(v1, v2), Is.False);
        }

        [Test]
        public void AreAdjacent_SelfLoop_ReturnsTrue()
        {
            var graph = new DirectedWeightedGraph<int>(1);
            var v0 = graph.AddVertex(0);
            graph.AddEdge(v0, v0, 1.0);
            Assert.That(graph.AreAdjacent(v0, v0), Is.True);
        }

        [Test]
        public void AreAdjacent_StartVertexNotInGraph_ThrowsInvalidOperationException()
        {
            var graph = new DirectedWeightedGraph<int>(1);
            var v1 = graph.AddVertex(1);
            var foreignVertex = CreateForeignVertex(graph, 99);

            Assert.Throws<InvalidOperationException>(() => graph.AreAdjacent(foreignVertex, v1));
        }

        [Test]
        public void AreAdjacent_EndVertexNotInGraph_ThrowsInvalidOperationException()
        {
            var graph = new DirectedWeightedGraph<int>(1);
            var v1 = graph.AddVertex(1);
            var foreignVertex = CreateForeignVertex(graph, 99);

            Assert.Throws<InvalidOperationException>(() => graph.AreAdjacent(v1, foreignVertex));
        }

        // AdjacentDistance Tests
        [Test]
        public void AdjacentDistance_AdjacentVertices_ReturnsCorrectWeight()
        {
            var graph = new DirectedWeightedGraph<int>(2);
            var v1 = graph.AddVertex(1);
            var v2 = graph.AddVertex(2);
            graph.AddEdge(v1, v2, 2.5);

            Assert.That(graph.AdjacentDistance(v1, v2), Is.EqualTo(2.5));
        }

        [Test]
        public void AdjacentDistance_NonAdjacentVertices_ReturnsZero()
        {
            var graph = new DirectedWeightedGraph<int>(2);
            var v1 = graph.AddVertex(1);
            var v2 = graph.AddVertex(2);

            Assert.That(graph.AdjacentDistance(v1, v2), Is.EqualTo(0.0));
        }

        [Test]
        public void AdjacentDistance_SelfLoop_ReturnsCorrectWeight()
        {
            var graph = new DirectedWeightedGraph<int>(1);
            var v0 = graph.AddVertex(0);
            graph.AddEdge(v0, v0, 3.5);
            Assert.That(graph.AdjacentDistance(v0, v0), Is.EqualTo(3.5));
        }

        [Test]
        public void AdjacentDistance_StartVertexNotInGraph_ThrowsInvalidOperationException()
        {
            var graph = new DirectedWeightedGraph<int>(1);
            var v1 = graph.AddVertex(1);
            var foreignVertex = CreateForeignVertex(graph, 99);

            Assert.Throws<InvalidOperationException>(() => graph.AdjacentDistance(foreignVertex, v1));
        }

        [Test]
        public void AdjacentDistance_EndVertexNotInGraph_ThrowsInvalidOperationException()
        {
            var graph = new DirectedWeightedGraph<int>(1);
            var v1 = graph.AddVertex(1);
            var foreignVertex = CreateForeignVertex(graph, 99);

            Assert.Throws<InvalidOperationException>(() => graph.AdjacentDistance(v1, foreignVertex));
        }
    }
}
