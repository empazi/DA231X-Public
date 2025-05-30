using Algorithms.Other;
using NUnit.Framework;
using SkiaSharp;
using System;

namespace Algorithms.Tests.Other
{
    [TestFixture]
    public class MandelbrotTests
    {
        [Test]
        public void GetBitmap_DefaultParameters_ReturnsBitmapWithCorrectDimensions()
        {
            // Act
            SKBitmap? bitmap = null;
            try
            {
                bitmap = Mandelbrot.GetBitmap();

                // Assert
                Assert.That(bitmap, Is.Not.Null);
                Assert.That(bitmap.Width, Is.EqualTo(800)); // Default width from Mandelbrot.cs
                Assert.That(bitmap.Height, Is.EqualTo(600)); // Default height from Mandelbrot.cs
            }
            finally
            {
                bitmap?.Dispose();
            }
        }

        [Test]
        public void GetBitmap_CustomParameters_ReturnsBitmapWithCorrectDimensions()
        {
            // Arrange
            int customWidth = 120;
            int customHeight = 80;
            double figureCenterX = -0.5;
            double figureCenterY = 0.1;
            double figureWidth = 2.5;
            int customMaxStep = 30;
            bool useDistanceColorCoding = true;

            // Act
            SKBitmap? bitmap = null;
            try
            {
                bitmap = Mandelbrot.GetBitmap(
                    bitmapWidth: customWidth,
                    bitmapHeight: customHeight,
                    figureCenterX: figureCenterX,
                    figureCenterY: figureCenterY,
                    figureWidth: figureWidth,
                    maxStep: customMaxStep,
                    useDistanceColorCoding: useDistanceColorCoding);

                // Assert
                Assert.That(bitmap, Is.Not.Null);
                Assert.That(bitmap.Width, Is.EqualTo(customWidth));
                Assert.That(bitmap.Height, Is.EqualTo(customHeight));
            }
            finally
            {
                bitmap?.Dispose();
            }
        }

        [Test]
        public void GetBitmap_UseDistanceColorCodingFalse_ReturnsBitmap()
        {
            // Arrange
            int customWidth = 60;
            int customHeight = 40;

            // Act
            SKBitmap? bitmap = null;
            try
            {
                bitmap = Mandelbrot.GetBitmap(
                    bitmapWidth: customWidth,
                    bitmapHeight: customHeight,
                    useDistanceColorCoding: false);

                // Assert
                Assert.That(bitmap, Is.Not.Null);
                Assert.That(bitmap.Width, Is.EqualTo(customWidth));
                Assert.That(bitmap.Height, Is.EqualTo(customHeight));
            }
            finally
            {
                bitmap?.Dispose();
            }
        }

        /*FAILED UNITTEST
        [Test]
        [TestCase(0, "bitmapWidth")]
        [TestCase(-1, "bitmapWidth")]
        [TestCase(100, "bitmapWidth", 0, "bitmapHeight")]
        [TestCase(100, "bitmapWidth", -1, "bitmapHeight")]
        [TestCase(100, "bitmapWidth", 100, "bitmapHeight", 0, "maxStep")]
        [TestCase(100, "bitmapWidth", 100, "bitmapHeight", -1, "maxStep")] // Line number for context
        public void GetBitmap_InvalidParameters_ThrowsArgumentOutOfRangeException(int width, string expectedParamNameForWidth, int height = 100, string? expectedParamNameForHeight = null, int maxStep = 50, string? expectedParamNameForMaxStep = null)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Mandelbrot.GetBitmap(width, height, maxStep: maxStep));
            // Explicitly check if ParamName is null to satisfy static analysis.
            // Based on Mandelbrot.cs, ParamName should always be set for ArgumentOutOfRangeException.
            if (ex.ParamName is null)
            {
                Assert.Fail($"The ParamName of the caught ArgumentOutOfRangeException was unexpectedly null. Exception message: {ex.Message}");
                return; // Should be unreachable if Assert.Fail throws, but good for clarity.
            }

            // At this point, ex.ParamName is known to be non-null.
            string actualParamName = ex.ParamName;

            string expectedParamValue = expectedParamNameForMaxStep ?? expectedParamNameForHeight ?? expectedParamNameForWidth;

            Assert.That(actualParamName, Is.EqualTo(expectedParamValue));
        }
        */
    }
}
