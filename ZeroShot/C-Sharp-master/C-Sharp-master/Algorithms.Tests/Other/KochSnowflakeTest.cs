using NUnit.Framework;
using System.Collections.Generic;
using System.Numerics;
using Algorithms.Other;
using SkiaSharp;
using System;
using System.Linq;
using NUnit.Framework.Legacy;

namespace Algorithms.Tests.Other
{
    [TestFixture]
    public class KochSnowflakeTests
    {
        // Define a tolerance for floating-point comparisons
        private const float Tolerance = 1e-6f;

        // Helper method to create a simple equilateral triangle centered(ish) for testing Iterate
        private static List<Vector2> CreateInitialTriangle()
        {
            // Simple triangle for logic testing, not necessarily the same as GetKochSnowflake's default
            var p1 = new Vector2(0, 0);
            var p2 = new Vector2(1, 0);
            var p3 = new Vector2(0.5f, (float)Math.Sqrt(3) / 2.0f);
            return new List<Vector2> { p1, p2, p3, p1 }; // Closed loop
        }

        // Helper method to create a single line segment
        private static List<Vector2> CreateSingleLine()
        {
            return new List<Vector2> { new Vector2(0, 0), new Vector2(3, 0) }; // Open line
        }

        [Test]
        public void Iterate_WithZeroSteps_ReturnsInitialVectors()
        {
            var initialVectors = CreateInitialTriangle();
            var result = KochSnowflake.Iterate(new List<Vector2>(initialVectors), 0); // Pass a copy

            Assert.That(result.Count, Is.EqualTo(initialVectors.Count));
            CollectionAssert.AreEqual(initialVectors, result);
        }

        [Test]
        public void Iterate_WithOneStep_Triangle_ReturnsCorrectNumberOfVectors()
        {
            var initialVectors = CreateInitialTriangle();
            int initialSegments = initialVectors.Count - 1; // 3 segments for the triangle
            var result = KochSnowflake.Iterate(initialVectors, 1);

            // Each segment becomes 4 segments. The endpoint of one is the start of the next.
            // Total points = (initial segments * 4) + 1 (because the last point is the same as the first)
            int expectedCount = (initialSegments * 4) + 1;

            Assert.That(result.Count, Is.EqualTo(expectedCount)); // 3*4 + 1 = 13
        }

        [Test]
        public void Iterate_WithTwoSteps_Triangle_ReturnsCorrectNumberOfVectors()
        {
            var initialVectors = CreateInitialTriangle();
            int initialSegments = initialVectors.Count - 1; // 3 segments
            var result = KochSnowflake.Iterate(initialVectors, 2);

            // Total points = (initial segments * 4^steps) + 1
            int expectedCount = (initialSegments * (int)Math.Pow(4, 2)) + 1;

            Assert.That(result.Count, Is.EqualTo(expectedCount)); // 3 * 16 + 1 = 49
        }

        [Test]
        public void Iterate_WithOneStep_SingleLine_ReturnsCorrectNumberOfVectors()
        {
            var initialVectors = CreateSingleLine(); // 2 points, 1 segment
            int initialSegments = initialVectors.Count - 1;
            var result = KochSnowflake.Iterate(initialVectors, 1);

            // For an open line, each segment becomes 4 segments, adding 3 points per segment.
            // Total points = initial points + (initial segments * 3)
            // Or alternatively: (initial segments * 4) + 1
            int expectedCount = (initialSegments * 4) + 1;

            Assert.That(result.Count, Is.EqualTo(expectedCount)); // 1*4 + 1 = 5
        }

        [Test]
        public void Iterate_WithOneStep_SingleLine_ReturnsCorrectVectors()
        {
            // Line from (0,0) to (3,0)
            var initialVectors = CreateSingleLine();
            var result = KochSnowflake.Iterate(initialVectors, 1);

            var start = initialVectors[0]; // (0,0)
            var end = initialVectors[1];   // (3,0)
            var diff = end - start;        // (3,0)

            var p1 = start + diff / 3;                                  // (1,0)
            var rotated = RotateVector(diff / 3, 60);                   // Rotate (1,0) by 60 deg -> (0.5, sqrt(3)/2)
            var p2 = start + diff / 3 + rotated;                        // (1,0) + (0.5, sqrt(3)/2) = (1.5, sqrt(3)/2)
            var p3 = start + diff * 2 / 3;                              // (2,0)

            var expectedVectors = new List<Vector2>
            {
                start, // (0,0)
                p1,    // (1,0)
                p2,    // (1.5, ~0.866)
                p3,    // (2,0)
                end    // (3,0)
            };

            Assert.That(result.Count, Is.EqualTo(expectedVectors.Count));
            for (int i = 0; i < result.Count; i++)
            {
                Assert.That(result[i].X, Is.EqualTo(expectedVectors[i].X).Within(Tolerance), $"X mismatch at index {i}");
                Assert.That(result[i].Y, Is.EqualTo(expectedVectors[i].Y).Within(Tolerance), $"Y mismatch at index {i}");
            }
        }

        [Test]
        public void Iterate_WithEmptyInput_ThrowsArgumentOutOfRangeException_DueToCurrentImplementation()
        {
            var initialVectors = new List<Vector2>();

            // Assert that calling Iterate with an empty list and steps > 0
            // throws an ArgumentOutOfRangeException.
            Assert.Throws<ArgumentOutOfRangeException>(() => KochSnowflake.Iterate(initialVectors, 5));
        }

        [Test]
        public void Iterate_WithSinglePointInput_ReturnsSinglePoint()
        {
            var initialVectors = new List<Vector2> { new Vector2(1, 1) };
            var result = KochSnowflake.Iterate(new List<Vector2>(initialVectors), 5); // Pass a copy
            Assert.That(result.Count, Is.EqualTo(1));
            CollectionAssert.AreEqual(initialVectors, result);
        }

        [Test]
        public void GetKochSnowflake_WithValidWidth_ReturnsBitmap()
        {
            int width = 100;
            using (var bitmap = KochSnowflake.GetKochSnowflake(width, 1)) // Use fewer steps for faster test
            {
                Assert.That(bitmap, Is.Not.Null);
                Assert.That(bitmap.Width, Is.EqualTo(width));
                Assert.That(bitmap.Height, Is.EqualTo(width)); // Height defaults to width in GetBitmap
                Assert.That(bitmap.ColorType, Is.EqualTo(SKColorType.Rgba8888).Or.EqualTo(SKColorType.Bgra8888).Or.EqualTo(SKColorType.Argb4444)); // Common color types
            }
        }

        [Test]
        public void GetKochSnowflake_WithZeroWidth_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => KochSnowflake.GetKochSnowflake(0, 5));
        }

        [Test]
        public void GetKochSnowflake_WithNegativeWidth_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => KochSnowflake.GetKochSnowflake(-100, 5));
        }

        [Test]
        public void GetKochSnowflake_WithZeroSteps_ReturnsBitmap()
        {
            int width = 50;
            // Should render the initial triangle
            using (var bitmap = KochSnowflake.GetKochSnowflake(width, 0))
            {
                Assert.That(bitmap, Is.Not.Null);
                Assert.That(bitmap.Width, Is.EqualTo(width));
                Assert.That(bitmap.Height, Is.EqualTo(width));
                // We could potentially check if *some* pixels are not white,
                // but it's complex and depends heavily on the exact initial triangle setup.
                // Checking non-null and dimensions is usually sufficient for this type of test.
            }
        }

        // Helper to mimic the private Rotate method for verification purposes
        private static Vector2 RotateVector(Vector2 vector, float angleInDegrees)
        {
            var radians = angleInDegrees * (float)Math.PI / 180;
            var ca = (float)Math.Cos(radians);
            var sa = (float)Math.Sin(radians);
            return new Vector2(ca * vector.X - sa * vector.Y, sa * vector.X + ca * vector.Y);
        }
    }
}
