using DataStructures.Graph;
using NUnit.Framework;
using FluentAssertions;
using System;
using System.Linq;

namespace DataStructures.Tests.Graph;

[TestFixture]
public class DirectedWeightedGraphTests
{
    [Test]
    public void Constructor_WithNegativeCapacity_ThrowsInvalidOperationException()
    {
        Action act = () => new DirectedWeightedGraph<int>(-1);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Graph capacity should always be a non-negative integer.");
    }

    [Test]
    public void AddVertex_IncreasesCountAndVertexIsInGraph()
    {
        var graph = new DirectedWeightedGraph<string>(5);
        var vertexA = graph.AddVertex("A");

        graph.Count.Should().Be(1);
        graph.Vertices.Should().Contain(vertexA);
        vertexA.Data.Should().Be("A");
        vertexA.Index.Should().Be(0);
    }

    [Test]
    public void AddVertex_WhenGraphIsFull_ThrowsInvalidOperationException()
    {
        var graph = new DirectedWeightedGraph<string>(1);
        graph.AddVertex("A");

        Action act = () => graph.AddVertex("B");
        act.Should().Throw<InvalidOperationException>().WithMessage("Graph overflow.");
    }

    [Test]
    public void AddEdge_ValidVerticesAndWeight_EdgeIsCreated()
    {
        var graph = new DirectedWeightedGraph<char>(2);
        var vertexX = graph.AddVertex('X');
        var vertexY = graph.AddVertex('Y');

        graph.AddEdge(vertexX, vertexY, 5.0);

        graph.AreAdjacent(vertexX, vertexY).Should().BeTrue();
        graph.AdjacentDistance(vertexX, vertexY).Should().Be(5.0);
    }

    [Test]
    public void AddEdge_WithZeroWeight_ThrowsInvalidOperationException()
    {
        var graph = new DirectedWeightedGraph<char>(2);
        var vertexX = graph.AddVertex('X');
        var vertexY = graph.AddVertex('Y');

        Action act = () => graph.AddEdge(vertexX, vertexY, 0.0);
        act.Should().Throw<InvalidOperationException>().WithMessage("Edge weight cannot be zero.");
    }

    [Test]
    public void AddEdge_StartVertexNotInGraph_ThrowsInvalidOperationException()
    {
        var graph = new DirectedWeightedGraph<int>(2);
        var graph2 = new DirectedWeightedGraph<int>(1);
        var vertex1 = graph.AddVertex(1);
        var vertex2 = graph.AddVertex(2);
        var alienVertex = graph2.AddVertex(100);

        Action act = () => graph.AddEdge(alienVertex, vertex2, 1.0);
        act.Should().Throw<InvalidOperationException>().WithMessage($"Vertex does not belong to graph: {alienVertex}.");
    }

    [Test]
    public void AddEdge_EndVertexNotInGraph_ThrowsInvalidOperationException()
    {
        var graph = new DirectedWeightedGraph<int>(2);
        var graph2 = new DirectedWeightedGraph<int>(1);
        var vertex1 = graph.AddVertex(1);
        var vertex2 = graph.AddVertex(2);
        var alienVertex = graph2.AddVertex(100);

        Action act = () => graph.AddEdge(vertex1, alienVertex, 1.0);
        act.Should().Throw<InvalidOperationException>().WithMessage($"Vertex does not belong to graph: {alienVertex}.");
    }

    [Test]
    public void AddEdge_EdgeAlreadyExists_ThrowsInvalidOperationException()
    {
        var graph = new DirectedWeightedGraph<char>(2);
        var vertexX = graph.AddVertex('X');
        var vertexY = graph.AddVertex('Y');
        graph.AddEdge(vertexX, vertexY, 5.0);

        Action act = () => graph.AddEdge(vertexX, vertexY, 10.0);
        act.Should().Throw<InvalidOperationException>().WithMessage("Vertex already exists: 5");
    }

    [Test]
    public void RemoveVertex_VertexExists_RemovesVertexAndAdjustsIndicesAndEdges()
    {
        var graph = new DirectedWeightedGraph<string>(3);
        var vA = graph.AddVertex("A"); // Index 0
        var vB = graph.AddVertex("B"); // Index 1
        var vC = graph.AddVertex("C"); // Index 2

        graph.AddEdge(vA, vB, 1);
        graph.AddEdge(vB, vC, 2);
        graph.AddEdge(vA, vC, 3);

        graph.RemoveVertex(vB);

        graph.Count.Should().Be(2);
        graph.Vertices.Should().NotContain(vB);
        graph.Vertices.Should().Contain(vA);
        graph.Vertices.Should().Contain(vC);

        vA.Index.Should().Be(0);
        vC.Index.Should().Be(1); // Index should be updated

        graph.AreAdjacent(vA, vC).Should().BeTrue();
        graph.AdjacentDistance(vA, vC).Should().Be(3);

        // Check that edges involving vB are gone
        graph.GetNeighbors(vA).Should().Contain(vC);
        graph.GetNeighbors(vA).Should().NotContain(vB);

        Action actVA = () => graph.AreAdjacent(vA, vB); // vB is no longer valid
        actVA.Should().Throw<InvalidOperationException>();

        Action actVC = () => graph.AreAdjacent(vC, vB); // vB is no longer valid
        actVC.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void RemoveVertex_VertexNotInGraph_ThrowsInvalidOperationException()
    {
        var graph = new DirectedWeightedGraph<int>(1);
        var graph2 = new DirectedWeightedGraph<int>(1);
        graph.AddVertex(1);
        var alienVertex = graph2.AddVertex(100);

        Action act = () => graph.RemoveVertex(alienVertex);
        act.Should().Throw<InvalidOperationException>().WithMessage($"Vertex does not belong to graph: {alienVertex}.");
    }

    [Test]
    public void RemoveEdge_EdgeExists_RemovesEdge()
    {
        var graph = new DirectedWeightedGraph<char>(2);
        var vertexX = graph.AddVertex('X');
        var vertexY = graph.AddVertex('Y');
        graph.AddEdge(vertexX, vertexY, 5.0);

        graph.RemoveEdge(vertexX, vertexY);

        graph.AreAdjacent(vertexX, vertexY).Should().BeFalse();
        graph.AdjacentDistance(vertexX, vertexY).Should().Be(0);
    }

    [Test]
    public void RemoveEdge_StartVertexNotInGraph_ThrowsInvalidOperationException()
    {
        var graph = new DirectedWeightedGraph<int>(1);
        var graph2 = new DirectedWeightedGraph<int>(1);
        var vertex1 = graph.AddVertex(1);
        var alienVertex = graph2.AddVertex(100);

        Action act = () => graph.RemoveEdge(alienVertex, vertex1);
        act.Should().Throw<InvalidOperationException>().WithMessage($"Vertex does not belong to graph: {alienVertex}.");
    }

    [Test]
    public void GetNeighbors_ReturnsCorrectNeighbors()
    {
        var graph = new DirectedWeightedGraph<string>(3);
        var vA = graph.AddVertex("A");
        var vB = graph.AddVertex("B");
        var vC = graph.AddVertex("C");

        graph.AddEdge(vA, vB, 1.0);
        graph.AddEdge(vA, vC, 2.0);

        var neighborsOfA = graph.GetNeighbors(vA).ToList();
        neighborsOfA.Should().HaveCount(2);
        neighborsOfA.Should().Contain(vB);
        neighborsOfA.Should().Contain(vC);

        var neighborsOfB = graph.GetNeighbors(vB).ToList();
        neighborsOfB.Should().BeEmpty();
    }

    [Test]
    public void AreAdjacent_And_AdjacentDistance_WorkCorrectly()
    {
        var graph = new DirectedWeightedGraph<int>(3);
        var v1 = graph.AddVertex(1);
        var v2 = graph.AddVertex(2);
        var v3 = graph.AddVertex(3);

        graph.AddEdge(v1, v2, 10.5);

        graph.AreAdjacent(v1, v2).Should().BeTrue();
        graph.AdjacentDistance(v1, v2).Should().Be(10.5);

        graph.AreAdjacent(v2, v1).Should().BeFalse(); // Directed graph
        graph.AdjacentDistance(v2, v1).Should().Be(0);

        graph.AreAdjacent(v1, v3).Should().BeFalse();
        graph.AdjacentDistance(v1, v3).Should().Be(0);
    }
}
