using NUnit.Framework;
using Algorithms.Other;
using System.Collections.Generic;
using System.Numerics;
using SkiaSharp;
using System;
using NUnit.Framework.Legacy;

namespace Algorithms.Tests.Other
{
    [TestFixture]
    public class KochSnowflakeTests
    {
        private const float Tolerance = 1e-5f;

        [Test]
        public void Iterate_WithZeroSteps_ReturnsInitialVectors()
        {
            var initialVectors = new List<Vector2>
            {
                new Vector2(0, 0),
                new Vector2(100, 0),
                new Vector2(50, 100),
                new Vector2(0, 0) // Closed triangle
            };

            // Iterate returns the list instance it was given if steps = 0.
            // We pass a copy to Iterate and compare its content against the original initialVectors.

            var result = KochSnowflake.Iterate(new List<Vector2>(initialVectors), 0);

            Assert.That(result, Is.EqualTo(initialVectors)); // Compares content element-wise

        }

        [Test]
        public void Iterate_WithOneStep_ReturnsCorrectNumberOfVectors()
        {
            // Triangle: A, B, C, A (4 points, 3 segments)
            var initialVectors = new List<Vector2>
            {
                new Vector2(0, 0),
                new Vector2(100, 0),
                new Vector2(50, 100),
                new Vector2(0, 0)
            };
            int initialPointCount = initialVectors.Count; // N = 4
            // Expected count for N points, M=N-1 segments: 1 + 4 * M = 1 + 4 * (N-1)
            int expectedCount = 1 + 4 * (initialPointCount - 1); // 1 + 4 * 3 = 13

            var result = KochSnowflake.Iterate(new List<Vector2>(initialVectors), 1);

            Assert.That(result.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void Iterate_WithMultipleSteps_ReturnsCorrectNumberOfVectors()
        {
            // Open line: A, B, C (3 points, 2 segments)
            var initialVectors = new List<Vector2>
            {
                new Vector2(0,0),
                new Vector2(1,0),
                new Vector2(2,0)
            };

            int steps = 2;
            int currentPointCount = initialVectors.Count; // N0 = 3

            // Step 0: N0 = 3
            // Step 1: N1 = 1 + 4 * (N0 - 1) = 1 + 4 * (3 - 1) = 1 + 4 * 2 = 9
            // Step 2: N2 = 1 + 4 * (N1 - 1) = 1 + 4 * (9 - 1) = 1 + 4 * 8 = 33
            int expectedCount = currentPointCount;
            for (int i = 0; i < steps; i++)
            {
                expectedCount = 1 + 4 * (expectedCount - 1);
            }

            var result = KochSnowflake.Iterate(new List<Vector2>(initialVectors), steps);
            Assert.That(result.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void Iterate_WithOneStep_CalculatesPointsCorrectly_SimpleHorizontalLine()
        {
            // Line segment P0-P1
            var initialVectors = new List<Vector2>
            {
                new Vector2(0, 0), // P0
                new Vector2(3, 0)  // P1
            };

            var result = KochSnowflake.Iterate(new List<Vector2>(initialVectors), 1);

            var expectedVectors = new List<Vector2>
            {
                new Vector2(0, 0),                                  // P0
                new Vector2(1, 0),                                  // P0 + diff/3
                new Vector2(1.5f, (float)Math.Sqrt(3) / 2f),        // P0 + diff/3 + rotated_diff/3
                new Vector2(2, 0),                                  // P0 + 2*diff/3
                new Vector2(3, 0)                                   // P1
            };

            Assert.That(result.Count, Is.EqualTo(expectedVectors.Count), "Number of vectors should match.");

            for (int i = 0; i < expectedVectors.Count; i++)
            {
                Assert.That(result[i].X, Is.EqualTo(expectedVectors[i].X).Within(Tolerance), $"X-coordinate mismatch at index {i}");
                Assert.That(result[i].Y, Is.EqualTo(expectedVectors[i].Y).Within(Tolerance), $"Y-coordinate mismatch at index {i}");
            }
        }

        [Test]
        public void Iterate_WithEmptyListAndSteps_ThrowsArgumentOutOfRangeException()
        {
            var initialVectors = new List<Vector2>();
            // IterationStep tries to access vectors[^1] (vectors[vectors.Count - 1]).
            // If vectors is empty, this becomes vectors[-1], throwing ArgumentOutOfRangeException.
            Assert.That(() => KochSnowflake.Iterate(initialVectors, 1), Throws.TypeOf<ArgumentOutOfRangeException>());

        }

        [Test]
        public void Iterate_WithEmptyListAndZeroSteps_ReturnsEmptyList()
        {
            var initialVectors = new List<Vector2>();
            var result = KochSnowflake.Iterate(initialVectors, 0);
            Assert.That(result, Is.Empty);
            Assert.That(result, Is.SameAs(initialVectors)); // Iterate returns the original list instance if steps = 0

        }

        [Test]
        public void Iterate_WithSinglePointList_ReturnsSinglePointList()
        {
            var initialVectors = new List<Vector2> { new Vector2(1, 1) };
            var result = KochSnowflake.Iterate(new List<Vector2>(initialVectors), 3); // Any steps > 0
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0], Is.EqualTo(initialVectors[0])); // Vector2.Equals performs member-wise comparison

        }

        [Test]
        public void GetKochSnowflake_DefaultParameters_ReturnsBitmap()
        {
            SKBitmap? bitmap = null; // Declare as nullable
            try
            {
                // This test relies on SkiaSharp native libraries being available.
                bitmap = KochSnowflake.GetKochSnowflake();
                Assert.That(bitmap, Is.Not.Null);
                Assert.That(bitmap.Width, Is.EqualTo(600));
                Assert.That(bitmap.Height, Is.EqualTo(600)); // GetBitmap is called with (width, width)

            }
            finally
            {
                bitmap?.Dispose();
            }
        }

        [Test]
        public void GetKochSnowflake_CustomParameters_ReturnsBitmapWithCorrectSize()
        {
            SKBitmap? bitmap = null; // Declare as nullable
            try
            {
                // This test relies on SkiaSharp native libraries being available.
                bitmap = KochSnowflake.GetKochSnowflake(bitmapWidth: 300, steps: 1);
                Assert.That(bitmap, Is.Not.Null);
                Assert.That(bitmap.Width, Is.EqualTo(300));
                Assert.That(bitmap.Height, Is.EqualTo(300));
            }
            finally
            {
                bitmap?.Dispose();
            }
        }

        [Test]
        public void GetKochSnowflake_InvalidBitmapWidth_ThrowsArgumentOutOfRangeException()
        {
            Assert.That(() => KochSnowflake.GetKochSnowflake(bitmapWidth: 0),
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Property("ParamName").EqualTo("bitmapWidth"));
            Assert.That(() => KochSnowflake.GetKochSnowflake(bitmapWidth: -100),
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Property("ParamName").EqualTo("bitmapWidth"));
        }
    }
}
