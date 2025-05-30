using Algorithms.Other;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Numerics;
using SkiaSharp;

namespace Algorithms.Tests.Other
{
    [TestFixture]
    public class KochSnowflakeTests
    {
        private const float Tolerance = 1e-5f;

        private static void AssertVector2AreEqual(Vector2 expected, Vector2 actual, float tolerance, string? message = null)
        {
            Assert.That(actual.X, Is.EqualTo(expected.X).Within(tolerance), $"X component mismatch. {message}");
            Assert.That(actual.Y, Is.EqualTo(expected.Y).Within(tolerance), $"Y component mismatch. {message}");
        }

        [Test]
        public void Iterate_ZeroSteps_ReturnsInitialVectors()
        {
            var initialVectors = new List<Vector2>
            {
                new Vector2(0, 0),
                new Vector2(10, 0),
                new Vector2(5, 10),
                new Vector2(0, 0)
            };
            var steps = 0;

            var result = KochSnowflake.Iterate(new List<Vector2>(initialVectors), steps);

            Assert.That(result.Count, Is.EqualTo(initialVectors.Count));
            for (int i = 0; i < initialVectors.Count; i++)
            {
                AssertVector2AreEqual(initialVectors[i], result[i], Tolerance, $"Vector at index {i} mismatch.");
            }
        }

        [Test]
        public void Iterate_OneStep_SingleLineSegment()
        {
            var initialVectors = new List<Vector2>
            {
                new Vector2(0, 0),
                new Vector2(3, 0)
            };
            var steps = 1;

            var result = KochSnowflake.Iterate(initialVectors, steps);

            var expectedVectors = new List<Vector2>
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1.5f, (float)Math.Sqrt(3) / 2f),
                new Vector2(2, 0),
                new Vector2(3, 0)
            };

            Assert.That(result.Count, Is.EqualTo(expectedVectors.Count), "Incorrect number of vectors.");
            for (int i = 0; i < expectedVectors.Count; i++)
            {
                AssertVector2AreEqual(expectedVectors[i], result[i], Tolerance, $"Vector at index {i} mismatch.");
            }
        }

        [Test]
        public void Iterate_OneStep_TwoSegments()
        {
            var initialVectors = new List<Vector2>
            {
                new Vector2(0, 0), // A
                new Vector2(3, 0), // B
                new Vector2(3, 3)  // C
            };
            var steps = 1;
            var result = KochSnowflake.Iterate(initialVectors, steps);

            var expectedVectors = new List<Vector2>
            {
                // From segment A-B
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1.5f, (float)Math.Sqrt(3) / 2f),
                new Vector2(2, 0),
                // From segment B-C (startVector B is added by IterationStep logic)
                new Vector2(3, 0),
                new Vector2(3, 1),
                new Vector2(3f - (float)Math.Sqrt(3) / 2f, 1.5f),
                new Vector2(3, 2),
                // Final point from original list
                new Vector2(3, 3)
            };

            Assert.That(result.Count, Is.EqualTo(expectedVectors.Count), "Incorrect number of vectors.");
            for (int i = 0; i < expectedVectors.Count; i++)
            {
                AssertVector2AreEqual(expectedVectors[i], result[i], Tolerance, $"Vector at index {i} mismatch.");
            }
        }

        [Test]
        public void Iterate_EmptyInitialVectors()
        {
            var initialVectors = new List<Vector2>();

            // Steps = 0 should return the empty list
            var resultZeroSteps = KochSnowflake.Iterate(new List<Vector2>(initialVectors), 0);
            Assert.That(resultZeroSteps, Is.Empty);

            // Steps > 0 will call IterationStep, which accesses vectors[^1]
            Assert.Throws<ArgumentOutOfRangeException>(() => KochSnowflake.Iterate(initialVectors, 1));
        }

        [Test]
        public void Iterate_SinglePointInitialVectors_ReturnsSinglePointUnchanged()
        {
            var singlePoint = new Vector2(5.5f, -2.1f);
            var initialVectors = new List<Vector2> { singlePoint };

            // Steps = 0
            var resultZeroSteps = KochSnowflake.Iterate(new List<Vector2>(initialVectors), 0);
            Assert.That(resultZeroSteps.Count, Is.EqualTo(1));
            AssertVector2AreEqual(singlePoint, resultZeroSteps[0], Tolerance);

            // Steps = 1
            var resultOneStep = KochSnowflake.Iterate(initialVectors, 1);
            Assert.That(resultOneStep.Count, Is.EqualTo(1));
            AssertVector2AreEqual(singlePoint, resultOneStep[0], Tolerance);

            // Steps = 5 (multiple steps)
            var resultMultipleSteps = KochSnowflake.Iterate(initialVectors, 5);
            Assert.That(resultMultipleSteps.Count, Is.EqualTo(1));
            AssertVector2AreEqual(singlePoint, resultMultipleSteps[0], Tolerance);
        }

        [Test]
        public void GetKochSnowflake_InvalidBitmapWidth_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => KochSnowflake.GetKochSnowflake(bitmapWidth: 0, steps: 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => KochSnowflake.GetKochSnowflake(bitmapWidth: -100, steps: 1));
        }

        [Test]
        public void GetKochSnowflake_ValidParameters_ReturnsBitmapWithCorrectDimensions()
        {
            var bitmapWidth = 250;
            var steps = 1;

            using var bitmap = KochSnowflake.GetKochSnowflake(bitmapWidth, steps);
            Assert.That(bitmap, Is.Not.Null);
            Assert.That(bitmap.Width, Is.EqualTo(bitmapWidth));
            Assert.That(bitmap.Height, Is.EqualTo(bitmapWidth));
        }

        [Test]
        public void GetKochSnowflake_DefaultParameters_ReturnsBitmapWithDefaultDimensions()
        {
            // Default width = 600, steps = 5
            using var bitmap = KochSnowflake.GetKochSnowflake();
            Assert.That(bitmap, Is.Not.Null);
            Assert.That(bitmap.Width, Is.EqualTo(600));
            Assert.That(bitmap.Height, Is.EqualTo(600));
        }
    }
}
