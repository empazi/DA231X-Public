using System;
using System.Collections.Generic;
using Algorithms.Graph.MinimumSpanningTree;
using NUnit.Framework;
using FluentAssertions;

namespace Algorithms.Tests.Graph;

public class KruskalTests
{
    [Test]
    public void Solve_AdjacencyMatrix_SimpleGraph()
    {
        // Arrange
        var adjacencyMatrix = new[,]
        {
            { float.PositiveInfinity, 2, float.PositiveInfinity, 6, float.PositiveInfinity },
            { 2, float.PositiveInfinity, 3, 8, 5 },
            { float.PositiveInfinity, 3, float.PositiveInfinity, float.PositiveInfinity, 7 },
            { 6, 8, float.PositiveInfinity, float.PositiveInfinity, 9 },
            { float.PositiveInfinity, 5, 7, 9, float.PositiveInfinity },
        };

        var expectedMst = new[,]
        {
            { float.PositiveInfinity, 2, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity },
            { 2, float.PositiveInfinity, 3, float.PositiveInfinity, 5 },
            { float.PositiveInfinity, 3, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity },
            { float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity }, // Node 3 (index) is not directly connected in this MST representation after choosing (0,1), (1,2), (1,4)
            { float.PositiveInfinity, 5, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity },
        };
        // Correcting expected based on Kruskal's output for the given graph:
        // Edges sorted by weight:
        // (0,1) - 2
        // (1,2) - 3
        // (1,4) - 5
        // (0,3) - 6 (forms cycle with 0-1-2-?-3, or 0-1-4-?-3, skip if it forms cycle with existing edges)
        // (2,4) - 7 (forms cycle 1-2-4)
        // (1,3) - 8 (forms cycle)
        // (3,4) - 9 (forms cycle)
        // MST edges: (0,1), (1,2), (1,4). Node 3 is isolated if we only consider these.
        // However, the problem asks for a spanning tree/forest. If node 3 is part of the original graph, it should be connected if possible.
        // The edge (0,3) with weight 6 would connect node 3.
        // Let's re-evaluate the expected MST:
        // Edges: (0,1,2), (1,2,3), (1,4,5), (0,3,6)
        // Sorted: (0,1,2), (1,2,3), (1,4,5), (0,3,6)
        // 1. Add (0,1) weight 2. Sets: {0,1}, {2}, {3}, {4}
        // 2. Add (1,2) weight 3. Sets: {0,1,2}, {3}, {4}
        // 3. Add (1,4) weight 5. Sets: {0,1,2,4}, {3}
        // 4. Add (0,3) weight 6. Sets: {0,1,2,3,4}
        // MST Edges: (0,1), (1,2), (1,4), (0,3)

        var expectedMstCorrected = new float[5, 5];
        for (int i = 0; i < 5; i++) for (int j = 0; j < 5; j++) expectedMstCorrected[i, j] = float.PositiveInfinity;
        expectedMstCorrected[0, 1] = 2; expectedMstCorrected[1, 0] = 2;
        expectedMstCorrected[1, 2] = 3; expectedMstCorrected[2, 1] = 3;
        expectedMstCorrected[1, 4] = 5; expectedMstCorrected[4, 1] = 5;
        expectedMstCorrected[0, 3] = 6; expectedMstCorrected[3, 0] = 6;


        // Act
        var mst = Kruskal.Solve(adjacencyMatrix);

        // Assert
        mst.Should().BeEquivalentTo(expectedMstCorrected);
    }

    [Test]
    public void Solve_AdjacencyMatrix_DisconnectedGraph()
    {
        // Arrange
        var adjacencyMatrix = new[,]
        {
            { float.PositiveInfinity, 1, float.PositiveInfinity, float.PositiveInfinity },
            { 1, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity },
            { float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, 2 },
            { float.PositiveInfinity, float.PositiveInfinity, 2, float.PositiveInfinity },
        };

        var expectedMst = new[,]
        {
            { float.PositiveInfinity, 1, float.PositiveInfinity, float.PositiveInfinity },
            { 1, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity },
            { float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, 2 },
            { float.PositiveInfinity, float.PositiveInfinity, 2, float.PositiveInfinity },
        };

        // Act
        var mst = Kruskal.Solve(adjacencyMatrix);

        // Assert
        mst.Should().BeEquivalentTo(expectedMst);
    }

    [Test]
    public void Solve_AdjacencyMatrix_ThrowsOnNonSquareMatrix()
    {
        var nonSquareMatrix = new float[2, 3];
        Action act = () => Kruskal.Solve(nonSquareMatrix);
        act.Should().Throw<ArgumentException>().WithMessage("Matrix must be square!");
    }

    [Test]
    public void Solve_AdjacencyMatrix_ThrowsOnNonSymmetricMatrix()
    {
        var nonSymmetricMatrix = new[,]
        {
            { float.PositiveInfinity, 1 },
            { 2, float.PositiveInfinity },
        };
        Action act = () => Kruskal.Solve(nonSymmetricMatrix);
        act.Should().Throw<ArgumentException>().WithMessage("Matrix must be symmetric!");
    }

    [Test]
    public void Solve_AdjacencyList_SimpleGraph()
    {
        // Arrange
        var adjacencyList = new Dictionary<int, float>[]
        {
            new() { { 1, 2 }, { 3, 6 } },
            new() { { 0, 2 }, { 2, 3 }, { 4, 5 } },
            new() { { 1, 3 } },
            new() { { 0, 6 } },
            new() { { 1, 5 } },
        };

        var expectedMst = new Dictionary<int, float>[]
        {
            new() { { 1, 2 }, {3, 6} }, // Edge (0,1) and (0,3)
            new() { { 0, 2 }, { 2, 3 }, { 4, 5 } }, // Edge (1,0), (1,2), (1,4)
            new() { { 1, 3 } }, // Edge (2,1)
            new() { { 0, 6 } }, // Edge (3,0)
            new() { { 1, 5 } }, // Edge (4,1)
        };

        // Expected MST edges: (0,1,2), (1,2,3), (1,4,5), (0,3,6)
        var expectedMstCorrected = new Dictionary<int, float>[5];
        for (int i = 0; i < 5; i++) expectedMstCorrected[i] = new Dictionary<int, float>();
        expectedMstCorrected[0].Add(1, 2); expectedMstCorrected[1].Add(0, 2);
        expectedMstCorrected[1].Add(2, 3); expectedMstCorrected[2].Add(1, 3);
        expectedMstCorrected[1].Add(4, 5); expectedMstCorrected[4].Add(1, 5);
        expectedMstCorrected[0].Add(3, 6); expectedMstCorrected[3].Add(0, 6);

        // Act
        var mst = Kruskal.Solve(adjacencyList);

        // Assert
        for (int i = 0; i < mst.Length; i++)
        {
            mst[i].Should().BeEquivalentTo(expectedMstCorrected[i]);
        }
    }

    [Test]
    public void Solve_AdjacencyList_DisconnectedGraph()
    {
        // Arrange
        var adjacencyList = new Dictionary<int, float>[]
        {
            new() { { 1, 1 } },
            new() { { 0, 1 } },
            new() { { 3, 2 } },
            new() { { 2, 2 } },
        };

        var expectedMst = new Dictionary<int, float>[]
        {
            new() { { 1, 1 } },
            new() { { 0, 1 } },
            new() { { 3, 2 } },
            new() { { 2, 2 } },
        };

        // Act
        var mst = Kruskal.Solve(adjacencyList);

        // Assert
        for (int i = 0; i < mst.Length; i++)
        {
            mst[i].Should().BeEquivalentTo(expectedMst[i]);
        }
    }

    [Test]
    public void Solve_AdjacencyList_ThrowsOnNonUndirectedGraph()
    {
        var nonUndirectedList = new Dictionary<int, float>[]
        {
            new() { { 1, 1 } }, // 0 -> 1
            new(), // 1 has no edge to 0
        };
        Action act = () => Kruskal.Solve(nonUndirectedList);
        act.Should().Throw<ArgumentException>().WithMessage("Graph must be undirected!");
    }
}
