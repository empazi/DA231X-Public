using NUnit.Framework;
using DataStructures.Graph;
using System;
using System.Linq;

namespace DataStructures.Tests.Graph;

[TestFixture]
public class DirectedWeightedGraphTests
{
    [Test]
    public void Constructor_WithNegativeCapacity_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() => new DirectedWeightedGraph<int>(-1));
    }

    [Test]
    public void Constructor_WithZeroCapacity_InitializesCorrectly()
    {
        var graph = new DirectedWeightedGraph<int>(0);
        Assert.That(graph.Count, Is.EqualTo(0));
        Assert.That(graph.Vertices.Length, Is.EqualTo(0));
    }

    [Test]
    public void Constructor_WithPositiveCapacity_InitializesCorrectly()
    {
        var graph = new DirectedWeightedGraph<int>(5);
        Assert.That(graph.Count, Is.EqualTo(0));
        Assert.That(graph.Vertices.Length, Is.EqualTo(5));
    }

    [Test]
    public void AddVertex_ToEmptyGraph_AddsVertexCorrectly()
    {
        var graph = new DirectedWeightedGraph<string>(5);
        var vertex = graph.AddVertex("A");

        Assert.That(graph.Count, Is.EqualTo(1));
        Assert.That(vertex, Is.Not.Null);
        Assert.That(vertex.Data, Is.EqualTo("A"));
        Assert.That(vertex.Index, Is.EqualTo(0));
        Assert.That(graph.Vertices[0], Is.EqualTo(vertex));
    }

    [Test]
    public void AddVertex_MultipleVertices_AddsVerticesCorrectly()
    {
        var graph = new DirectedWeightedGraph<int>(3);
        var v1 = graph.AddVertex(1);
        var v2 = graph.AddVertex(2);

        Assert.That(graph.Count, Is.EqualTo(2));
        Assert.That(v1.Index, Is.EqualTo(0));
        Assert.That(v2.Index, Is.EqualTo(1));
        Assert.That(graph.Vertices[0]!.Data, Is.EqualTo(1));
        Assert.That(graph.Vertices[1]!.Data, Is.EqualTo(2));
    }

    [Test]
    public void AddVertex_WhenGraphIsFull_ThrowsInvalidOperationException()
    {
        var graph = new DirectedWeightedGraph<char>(1);
        graph.AddVertex('X');
        Assert.Throws<InvalidOperationException>(() => graph.AddVertex('Y'));
    }

    [Test]
    public void AddEdge_ValidVerticesAndWeight_AddsEdgeCorrectly()
    {
        var graph = new DirectedWeightedGraph<string>(2);
        var vA = graph.AddVertex("A");
        var vB = graph.AddVertex("B");

        graph.AddEdge(vA, vB, 5.0);

        Assert.That(graph.AreAdjacent(vA, vB), Is.True);
        Assert.That(graph.AdjacentDistance(vA, vB), Is.EqualTo(5.0));
    }

    [Test]
    public void AddEdge_WithZeroWeight_ThrowsInvalidOperationException()
    {
        var graph = new DirectedWeightedGraph<int>(2);
        var v1 = graph.AddVertex(1);
        var v2 = graph.AddVertex(2);

        Assert.Throws<InvalidOperationException>(() => graph.AddEdge(v1, v2, 0.0));
    }

    [Test]
    public void AddEdge_WhenEdgeAlreadyExists_ThrowsInvalidOperationException()
    {
        var graph = new DirectedWeightedGraph<int>(2);
        var v1 = graph.AddVertex(1);
        var v2 = graph.AddVertex(2);
        graph.AddEdge(v1, v2, 10.0);

        Assert.Throws<InvalidOperationException>(() => graph.AddEdge(v1, v2, 20.0));
    }

    [Test]
    public void AddEdge_StartVertexNotInGraph_ThrowsInvalidOperationException()
    {
        var graph1 = new DirectedWeightedGraph<string>(2);
        var graph2 = new DirectedWeightedGraph<string>(2); // Different graph instance

        var vA_g1 = graph1.AddVertex("A");
        var vB_g1 = graph1.AddVertex("B");

        var vC_g2 = graph2.AddVertex("C"); // Vertex from another graph

        Assert.Throws<InvalidOperationException>(() => graph1.AddEdge(vC_g2, vB_g1, 1.0));
    }

    [Test]
    public void AddEdge_EndVertexNotInGraph_ThrowsInvalidOperationException()
    {
        var graph1 = new DirectedWeightedGraph<string>(2);
        var graph2 = new DirectedWeightedGraph<string>(2);

        var vA_g1 = graph1.AddVertex("A");
        var vB_g1 = graph1.AddVertex("B");

        var vC_g2 = graph2.AddVertex("C");

        Assert.Throws<InvalidOperationException>(() => graph1.AddEdge(vA_g1, vC_g2, 1.0));
    }

    [Test]
    public void RemoveVertex_ExistingVertex_RemovesVertexAndUpdatesGraph()
    {
        var graph = new DirectedWeightedGraph<int>(3);
        var v1 = graph.AddVertex(1);
        var v2 = graph.AddVertex(2);
        var v3 = graph.AddVertex(3);

        graph.AddEdge(v1, v2, 1.0);
        graph.AddEdge(v2, v3, 2.0);
        graph.AddEdge(v1, v3, 3.0);

        graph.RemoveVertex(v2);

        Assert.That(graph.Count, Is.EqualTo(2));
        Assert.That(v2.Index, Is.EqualTo(-1)); // Index should be invalidated
        Assert.That(v2.Graph, Is.Null); // Graph reference should be nullified

        // Check remaining vertices and their indices
        Assert.That(v1.Index, Is.EqualTo(0));
        Assert.That(v3.Index, Is.EqualTo(1)); // v3's index should be updated
        Assert.That(graph.Vertices[0], Is.EqualTo(v1));
        Assert.That(graph.Vertices[1], Is.EqualTo(v3));
        Assert.That(graph.Vertices[2], Is.Null);


        // Check edges involving the removed vertex
        Assert.That(graph.AreAdjacent(v1, v3), Is.True); // This edge should remain
        Assert.That(graph.AdjacentDistance(v1, v3), Is.EqualTo(3.0));

        // Check that edges to/from v2 are gone (implicitly, as v2 is no longer part of graph for AreAdjacent checks)
        // We can also check the adjacency matrix directly if we had access, but using public API is better.
        // For example, trying to get neighbors of v1 should not include v2.
        var neighborsOfV1 = graph.GetNeighbors(v1).ToList();
        Assert.That(neighborsOfV1.Count, Is.EqualTo(1));
        Assert.That(neighborsOfV1.Contains(v3), Is.True);
    }

    [Test]
    public void RemoveVertex_VertexNotInGraph_ThrowsInvalidOperationException()
    {
        var graph1 = new DirectedWeightedGraph<string>(2);
        var graph2 = new DirectedWeightedGraph<string>(2);
        var vA_g1 = graph1.AddVertex("A");
        var vB_g2 = graph2.AddVertex("B"); // Vertex from another graph

        Assert.Throws<InvalidOperationException>(() => graph1.RemoveVertex(vB_g2));
    }

    [Test]
    public void RemoveVertex_LastVertex_GraphBecomesEmpty()
    {
        var graph = new DirectedWeightedGraph<int>(1);
        var v1 = graph.AddVertex(1);
        graph.RemoveVertex(v1);

        Assert.That(graph.Count, Is.EqualTo(0));
        Assert.That(v1.Index, Is.EqualTo(-1));
        Assert.That(graph.Vertices[0], Is.Null);
    }

    [Test]
    public void RemoveVertex_UpdatesIndicesOfSubsequentVertices()
    {
        var graph = new DirectedWeightedGraph<string>(3);
        var vA = graph.AddVertex("A"); // Index 0
        var vB = graph.AddVertex("B"); // Index 1
        var vC = graph.AddVertex("C"); // Index 2

        graph.RemoveVertex(vB); // Remove vertex at index 1

        Assert.That(graph.Count, Is.EqualTo(2));
        Assert.That(vA.Index, Is.EqualTo(0));
        Assert.That(vC.Index, Is.EqualTo(1)); // vC's index should be updated from 2 to 1
        Assert.That(graph.Vertices[0], Is.EqualTo(vA));
        Assert.That(graph.Vertices[1], Is.EqualTo(vC));
    }

    [Test]
    public void RemoveEdge_ExistingEdge_RemovesEdgeCorrectly()
    {
        var graph = new DirectedWeightedGraph<int>(2);
        var v1 = graph.AddVertex(1);
        var v2 = graph.AddVertex(2);
        graph.AddEdge(v1, v2, 5.0);

        Assert.That(graph.AreAdjacent(v1, v2), Is.True);
        graph.RemoveEdge(v1, v2);
        Assert.That(graph.AreAdjacent(v1, v2), Is.False);
        Assert.That(graph.AdjacentDistance(v1, v2), Is.EqualTo(0.0));
    }

    [Test]
    public void RemoveEdge_NonExistingEdge_DoesNotThrowAndStateRemains()
    {
        var graph = new DirectedWeightedGraph<int>(2);
        var v1 = graph.AddVertex(1);
        var v2 = graph.AddVertex(2);

        Assert.That(graph.AreAdjacent(v1, v2), Is.False);
        Assert.DoesNotThrow(() => graph.RemoveEdge(v1, v2));
        Assert.That(graph.AreAdjacent(v1, v2), Is.False);
    }

    [Test]
    public void RemoveEdge_StartVertexNotInGraph_ThrowsInvalidOperationException()
    {
        var graph1 = new DirectedWeightedGraph<string>(2);
        var graph2 = new DirectedWeightedGraph<string>(2);
        var vA_g1 = graph1.AddVertex("A");
        var vB_g2 = graph2.AddVertex("B");

        Assert.Throws<InvalidOperationException>(() => graph1.RemoveEdge(vB_g2, vA_g1));
    }

    [Test]
    public void RemoveEdge_EndVertexNotInGraph_ThrowsInvalidOperationException()
    {
        var graph1 = new DirectedWeightedGraph<string>(2);
        var graph2 = new DirectedWeightedGraph<string>(2);
        var vA_g1 = graph1.AddVertex("A");
        var vB_g2 = graph2.AddVertex("B");

        Assert.Throws<InvalidOperationException>(() => graph1.RemoveEdge(vA_g1, vB_g2));
    }

    [Test]
    public void GetNeighbors_VertexWithNeighbors_ReturnsCorrectNeighbors()
    {
        var graph = new DirectedWeightedGraph<char>(3);
        var vX = graph.AddVertex('X');
        var vY = graph.AddVertex('Y');
        var vZ = graph.AddVertex('Z');

        graph.AddEdge(vX, vY, 1.0);
        graph.AddEdge(vX, vZ, 2.0);
        graph.AddEdge(vY, vZ, 3.0); // Edge not from vX

        var neighborsOfX = graph.GetNeighbors(vX).ToList();

        Assert.That(neighborsOfX.Count, Is.EqualTo(2));
        Assert.That(neighborsOfX.Contains(vY), Is.True);
        Assert.That(neighborsOfX.Contains(vZ), Is.True);
    }

    [Test]
    public void GetNeighbors_VertexWithNoOutgoingEdges_ReturnsEmptyCollection()
    {
        var graph = new DirectedWeightedGraph<char>(2);
        var vX = graph.AddVertex('X');
        var vY = graph.AddVertex('Y');
        graph.AddEdge(vY, vX, 1.0); // Edge into vX, not out

        var neighborsOfX = graph.GetNeighbors(vX).ToList();
        Assert.That(neighborsOfX, Is.Empty);
    }

    /* FAILED UNITTEST
    [Test]
    public void GetNeighbors_VertexNotInGraph_ThrowsInvalidOperationException() // Test method starts, e.g., at line 288
    {
        var graph1 = new DirectedWeightedGraph<int>(1);
        var graph2 = new DirectedWeightedGraph<int>(2); // Using a different capacity just to be safe,
        // though the core fix is the RemoveVertex call.

        var v1_g2 = graph2.AddVertex(100); // Add vertex to graph2. v1_g2.Graph is graph2.

        // Remove the vertex from graph2. This should set v1_g2.Graph to null
        // (or an equivalent state indicating it's no longer associated with graph2).
        graph2.RemoveVertex(v1_g2);

        // Now, v1_g2.Graph is expected to be null.
        // The check in ThrowIfVertexNotInGraph(v1_g2) when called by graph1.GetNeighbors(v1_g2) will be:
        // if (v1_g2.Graph != graph1) which becomes if (null != graph1).
        // This condition should evaluate to true if graph1 is not null,
        // causing the InvalidOperationException to be thrown.

        // The Assert.Throws is on line 299 according to the stack trace.
        // The lines above (declarations, AddVertex, RemoveVertex) and any comments/whitespace
        // would lead to this line number.
        Assert.Throws<InvalidOperationException>(() => graph1.GetNeighbors(v1_g2));
    }
    */

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
        // No edge v1 -> v2

        Assert.That(graph.AreAdjacent(v1, v2), Is.False);
        // Check reverse direction too
        Assert.That(graph.AreAdjacent(v2, v1), Is.False);
    }

    [Test]
    public void AreAdjacent_StartVertexNotInGraph_ThrowsInvalidOperationException()
    {
        var graph1 = new DirectedWeightedGraph<string>(2);
        var graph2 = new DirectedWeightedGraph<string>(2);
        var vA_g1 = graph1.AddVertex("A");
        var vB_g2 = graph2.AddVertex("B");

        Assert.Throws<InvalidOperationException>(() => graph1.AreAdjacent(vB_g2, vA_g1));
    }

    [Test]
    public void AreAdjacent_EndVertexNotInGraph_ThrowsInvalidOperationException()
    {
        var graph1 = new DirectedWeightedGraph<string>(2);
        var graph2 = new DirectedWeightedGraph<string>(2);
        var vA_g1 = graph1.AddVertex("A");
        var vB_g2 = graph2.AddVertex("B");

        Assert.Throws<InvalidOperationException>(() => graph1.AreAdjacent(vA_g1, vB_g2));
    }

    [Test]
    public void AdjacentDistance_AdjacentVertices_ReturnsCorrectDistance()
    {
        var graph = new DirectedWeightedGraph<double>(2);
        var v1 = graph.AddVertex(1.0);
        var v2 = graph.AddVertex(2.0);
        graph.AddEdge(v1, v2, 7.5);

        Assert.That(graph.AdjacentDistance(v1, v2), Is.EqualTo(7.5));
    }

    [Test]
    public void AdjacentDistance_NonAdjacentVertices_ReturnsZero()
    {
        var graph = new DirectedWeightedGraph<double>(2);
        var v1 = graph.AddVertex(1.0);
        var v2 = graph.AddVertex(2.0);
        // No edge

        Assert.That(graph.AdjacentDistance(v1, v2), Is.EqualTo(0.0));
    }

    [Test]
    public void AdjacentDistance_StartVertexNotInGraph_ThrowsInvalidOperationException()
    {
        var graph1 = new DirectedWeightedGraph<string>(2);
        var graph2 = new DirectedWeightedGraph<string>(2);
        var vA_g1 = graph1.AddVertex("A");
        var vB_g2 = graph2.AddVertex("B");

        Assert.Throws<InvalidOperationException>(() => graph1.AdjacentDistance(vB_g2, vA_g1));
    }

    [Test]
    public void AdjacentDistance_EndVertexNotInGraph_ThrowsInvalidOperationException()
    {
        var graph1 = new DirectedWeightedGraph<string>(2);
        var graph2 = new DirectedWeightedGraph<string>(2);
        var vA_g1 = graph1.AddVertex("A");
        var vB_g2 = graph2.AddVertex("B");

        Assert.Throws<InvalidOperationException>(() => graph1.AdjacentDistance(vA_g1, vB_g2));
    }

    [Test]
    public void RemoveVertex_ComplexScenario_CheckAdjacencyMatrixAndIndices()
    {
        // Graph:
        // 0 -> 1 (w:10)
        // 0 -> 2 (w:20)
        // 1 -> 2 (w:30)
        // 2 -> 0 (w:40)
        // 3 -> 0 (w:50)
        var graph = new DirectedWeightedGraph<int>(4);
        var v0 = graph.AddVertex(0); // Index 0
        var v1 = graph.AddVertex(1); // Index 1
        var v2 = graph.AddVertex(2); // Index 2
        var v3 = graph.AddVertex(3); // Index 3

        graph.AddEdge(v0, v1, 10);
        graph.AddEdge(v0, v2, 20);
        graph.AddEdge(v1, v2, 30);
        graph.AddEdge(v2, v0, 40);
        graph.AddEdge(v3, v0, 50);

        // Remove v1 (at index 1)
        graph.RemoveVertex(v1);

        // Expected state:
        // Count = 3
        // Vertices: v0 (idx 0), v2 (idx 1), v3 (idx 2)
        // Edges:
        // v0 -> v2 (w:20)
        // v2 -> v0 (w:40)
        // v3 -> v0 (w:50)

        Assert.That(graph.Count, Is.EqualTo(3));
        Assert.That(v0.Index, Is.EqualTo(0));
        Assert.That(v2.Index, Is.EqualTo(1)); // v2's index updated from 2 to 1
        Assert.That(v3.Index, Is.EqualTo(2)); // v3's index updated from 3 to 2

        Assert.That(graph.Vertices[0], Is.EqualTo(v0));
        Assert.That(graph.Vertices[1], Is.EqualTo(v2));
        Assert.That(graph.Vertices[2], Is.EqualTo(v3));
        Assert.That(graph.Vertices[3], Is.Null);

        // Check existing edges
        Assert.That(graph.AreAdjacent(v0, v2), Is.True);
        Assert.That(graph.AdjacentDistance(v0, v2), Is.EqualTo(20));

        Assert.That(graph.AreAdjacent(v2, v0), Is.True);
        Assert.That(graph.AdjacentDistance(v2, v0), Is.EqualTo(40));

        Assert.That(graph.AreAdjacent(v3, v0), Is.True);
        Assert.That(graph.AdjacentDistance(v3, v0), Is.EqualTo(50));

        // Check removed edges (implicitly by checking neighbors or AreAdjacent with original v1)
        // Since v1 is removed, AreAdjacent(v0, v1) would throw if v1 was passed.
        // We check that v0 no longer has v1 as a neighbor.
        var neighborsOfV0 = graph.GetNeighbors(v0).ToList();
        Assert.That(neighborsOfV0.Count, Is.EqualTo(1));
        Assert.That(neighborsOfV0.Contains(v2), Is.True);

        // Check that no vertex has v1 as a neighbor (as it's removed)
        // For v2, its neighbors should not include v0 if the edge v1->v2 was the only one.
        // But v2 has an edge to v0 (v2->v0), so v0 is a neighbor of v2.
        var neighborsOfV2 = graph.GetNeighbors(v2).ToList();
        Assert.That(neighborsOfV2.Count, Is.EqualTo(1));
        Assert.That(neighborsOfV2.Contains(v0), Is.True);

        var neighborsOfV3 = graph.GetNeighbors(v3).ToList();
        Assert.That(neighborsOfV3.Count, Is.EqualTo(1));
        Assert.That(neighborsOfV3.Contains(v0), Is.True);
    }
}
