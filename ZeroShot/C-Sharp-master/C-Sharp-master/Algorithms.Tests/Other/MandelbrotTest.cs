using NUnit.Framework;
using Algorithms.Other; // Assuming Mandelbrot.cs is in this namespace
using SkiaSharp;
using System;

namespace Algorithms.Tests.Other
{
    [TestFixture]
    public class MandelbrotTests
    {
        private const byte ExpectedAlpha = 255;

        // Helper method to get the figure coordinates for pixel (0,0) in a 1x1 bitmap
        // such that pixel (0,0) corresponds to (targetFigureX, targetFigureY).
        private (double centerX, double centerY, double width) GetParamsForSinglePointTest(double targetFigureX, double targetFigureY, double figureWidth = 0.01)
        {
            double centerX = targetFigureX + 0.5 * figureWidth;
            double centerY = targetFigureY + 0.5 * figureWidth;
            return (centerX, centerY, figureWidth);
        }

        [Test]
        public void GetBitmap_DefaultParameters_ReturnsBitmap()
        {
            using var bitmap = Mandelbrot.GetBitmap();
            Assert.That(bitmap, Is.Not.Null);
            Assert.That(bitmap.Width, Is.EqualTo(800));
            Assert.That(bitmap.Height, Is.EqualTo(600));
        }

        [Test]
        public void GetBitmap_CustomDimensions_ReturnsCorrectlySizedBitmap()
        {
            using var bitmap = Mandelbrot.GetBitmap(bitmapWidth: 100, bitmapHeight: 50);
            Assert.That(bitmap, Is.Not.Null);
            Assert.That(bitmap.Width, Is.EqualTo(100));
            Assert.That(bitmap.Height, Is.EqualTo(50));
        }

        [Test]
        public void GetBitmap_InvalidBitmapWidth_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Mandelbrot.GetBitmap(bitmapWidth: 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Mandelbrot.GetBitmap(bitmapWidth: -1));
        }

        [Test]
        public void GetBitmap_InvalidBitmapHeight_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Mandelbrot.GetBitmap(bitmapHeight: 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Mandelbrot.GetBitmap(bitmapHeight: -1));
        }

        [Test]
        public void GetBitmap_InvalidMaxStep_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Mandelbrot.GetBitmap(maxStep: 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Mandelbrot.GetBitmap(maxStep: -1));
        }

        [Test]
        public void GetBitmap_PointInsideSet_0_0_BlackAndWhite_IsBlack()
        {
            // For c = 0+0i, z_0 = c = 0.
            // z_1 = 0^2 + 0 = 0. |z_1|^2 = 0.
            // ... never escapes. currentStep = maxStep - 1. distance = 1. Black.
            var (centerX, centerY, width) = GetParamsForSinglePointTest(0, 0);
            using var bitmap = Mandelbrot.GetBitmap(
                bitmapWidth: 1, bitmapHeight: 1,
                figureCenterX: centerX, figureCenterY: centerY, figureWidth: width,
                maxStep: 10, useDistanceColorCoding: false);

            SKColor pixelColor = bitmap.GetPixel(0, 0);
            Assert.That(pixelColor, Is.EqualTo(new SKColor(0, 0, 0, ExpectedAlpha)));
        }

        [Test]
        public void GetBitmap_PointOutsideSet_2_0_BlackAndWhite_IsWhite()
        {
            // For c = 2+0i, z_0 = c = 2.
            // z_1 = 2^2 + 2 = 6. |z_1|^2 = 36 > 4. Escapes at step=0. currentStep=0.
            // distance = 0 / (10-1) = 0. White.
            var (centerX, centerY, width) = GetParamsForSinglePointTest(2, 0);
            using var bitmap = Mandelbrot.GetBitmap(
                bitmapWidth: 1, bitmapHeight: 1,
                figureCenterX: centerX, figureCenterY: centerY, figureWidth: width,
                maxStep: 10, useDistanceColorCoding: false);

            SKColor pixelColor = bitmap.GetPixel(0, 0);
            Assert.That(pixelColor, Is.EqualTo(new SKColor(255, 255, 255, ExpectedAlpha)));
        }

        [Test]
        public void GetBitmap_PointInsideSet_0_0_ColorCoded_IsBlack()
        {
            // Same logic as BlackAndWhite test for c=0,0. distance = 1. Black.
            var (centerX, centerY, width) = GetParamsForSinglePointTest(0, 0);
            using var bitmap = Mandelbrot.GetBitmap(
                bitmapWidth: 1, bitmapHeight: 1,
                figureCenterX: centerX, figureCenterY: centerY, figureWidth: width,
                maxStep: 10, useDistanceColorCoding: true);

            SKColor pixelColor = bitmap.GetPixel(0, 0);
            Assert.That(pixelColor, Is.EqualTo(new SKColor(0, 0, 0, ExpectedAlpha)));
        }

        [Test]
        public void GetBitmap_PointOutsideSet_1_0_ColorCoded_IsCorrectColor()
        {
            // For c = 1+0i, maxStep = 10, z_0 = c = 1+0i.
            // z_0 = 1.
            // step 0: z_1 = 1^2 + 1 = 2. |z_1|^2 = 4. Not > 4. currentStep = 0.
            // step 1: z_2 = 2^2 + 1 = 5. |z_2|^2 = 25 > 4. Escapes. currentStep = 1.
            // distance = 1 / (10 - 1) = 1/9.
            // hue = 360 * (1/9) = 40.
            // hi = 0, f = 2/3. Color(v,t,p) = (255, 170, 0).
            var expectedColor = new SKColor(255, 170, 0, ExpectedAlpha); // Was #ffffaa00

            var (centerX, centerY, width) = GetParamsForSinglePointTest(1, 0);
            using var bitmap = Mandelbrot.GetBitmap(
                bitmapWidth: 1, bitmapHeight: 1,
                figureCenterX: centerX, figureCenterY: centerY, figureWidth: width,
                maxStep: 10, useDistanceColorCoding: true);

            SKColor pixelColor = bitmap.GetPixel(0, 0);
            Assert.That(pixelColor, Is.EqualTo(expectedColor));
        }

        [Test]
        public void GetBitmap_PointOutsideSet_2_0_ColorCoded_IsCorrectColor()
        {
            // For c = 2+0i, maxStep = 10, z_0 = c = 2+0i.
            // z_0 = 2.
            // step 0: z_1 = 2^2 + 2 = 6. |z_1|^2 = 36 > 4. Escapes. currentStep = 0.
            // distance = 0 / (10 - 1) = 0.
            // hue = 360 * 0 = 0.
            // hi = 0, f = 0. Color(v,t,p) = (255, 0, 0).
            var expectedColor = new SKColor(255, 0, 0, ExpectedAlpha); // Was #ffff0000

            var (centerX, centerY, width) = GetParamsForSinglePointTest(2, 0);
            using var bitmap = Mandelbrot.GetBitmap(
                bitmapWidth: 1, bitmapHeight: 1,
                figureCenterX: centerX, figureCenterY: centerY, figureWidth: width,
                maxStep: 10, useDistanceColorCoding: true);

            SKColor pixelColor = bitmap.GetPixel(0, 0);
            Assert.That(pixelColor, Is.EqualTo(expectedColor));
        }

        [Test]
        public void GetBitmap_DifferentMaxSteps_ProduceDifferentColorsForEscapingPoint()
        {
            var (centerX, centerY, width) = GetParamsForSinglePointTest(1, 0); // c = 1+0i

            // maxStep = 10, for c = 1+0i (z_0 = c):
            // currentStep = 1, distance = 1/9. Color (255, 170, 0)
            using var bitmap1 = Mandelbrot.GetBitmap(
                bitmapWidth: 1, bitmapHeight: 1,
                figureCenterX: centerX, figureCenterY: centerY, figureWidth: width,
                maxStep: 10, useDistanceColorCoding: true);
            SKColor color1 = bitmap1.GetPixel(0, 0);
            Assert.That(color1, Is.EqualTo(new SKColor(255, 170, 0, ExpectedAlpha))); // Was (170, 255, 85)

            // maxStep = 50, for c = 1+0i (z_0 = c):
            // currentStep = 1, distance = 1 / (50 - 1) = 1/49.
            // hue = 360 * (1/49) approx 7.347.
            // hi = 0, f approx 0.1224. Color(v,t,p) = (255, 31, 0) (approx values)
            using var bitmap2 = Mandelbrot.GetBitmap(
                bitmapWidth: 1, bitmapHeight: 1,
                figureCenterX: centerX, figureCenterY: centerY, figureWidth: width,
                maxStep: 50, useDistanceColorCoding: true);
            SKColor color2 = bitmap2.GetPixel(0, 0);
            // Recalculated: hue=7.3469, f=0.1224489, q=223, t=31. Color(255,31,0)
            Assert.That(color2, Is.EqualTo(new SKColor(255, 31, 0, ExpectedAlpha))); // Was (255, 62, 0)

            Assert.That(color1, Is.Not.EqualTo(color2));
        }

        [Test]
        public void GetBitmap_MaxStep1_PointInsideSet_BlackAndWhite_IsWhite_DueToNaNDistance()
        {
            var (centerX, centerY, width) = GetParamsForSinglePointTest(0, 0);
            using var bitmap = Mandelbrot.GetBitmap(
                bitmapWidth: 1, bitmapHeight: 1,
                figureCenterX: centerX, figureCenterY: centerY, figureWidth: width,
                maxStep: 1, useDistanceColorCoding: false);

            SKColor pixelColor = bitmap.GetPixel(0, 0);
            Assert.That(pixelColor, Is.EqualTo(new SKColor(255, 255, 255, ExpectedAlpha)), "Expected white for point in set with maxStep=1 (B&W) due to NaN distance behavior.");
        }

        [Test]
        public void GetBitmap_MaxStep1_PointOutsideSet_BlackAndWhite_IsWhite_DueToNaNDistance()
        {
            var (centerX, centerY, width) = GetParamsForSinglePointTest(3, 0);
            using var bitmap = Mandelbrot.GetBitmap(
                bitmapWidth: 1, bitmapHeight: 1,
                figureCenterX: centerX, figureCenterY: centerY, figureWidth: width,
                maxStep: 1, useDistanceColorCoding: false);

            SKColor pixelColor = bitmap.GetPixel(0, 0);
            Assert.That(pixelColor, Is.EqualTo(new SKColor(255, 255, 255, ExpectedAlpha)), "Expected white for point outside set with maxStep=1 (B&W) due to NaN distance behavior.");
        }

        [Test]
        public void GetBitmap_MaxStep1_PointInsideSet_ColorCoded_IsRed_DueToNaNDistance()
        {
            var expectedColor = new SKColor(255, 0, 0, ExpectedAlpha);
            var (centerX, centerY, width) = GetParamsForSinglePointTest(0, 0);
            using var bitmap = Mandelbrot.GetBitmap(
                bitmapWidth: 1, bitmapHeight: 1,
                figureCenterX: centerX, figureCenterY: centerY, figureWidth: width,
                maxStep: 1, useDistanceColorCoding: true);

            SKColor pixelColor = bitmap.GetPixel(0, 0);
            Assert.That(pixelColor, Is.EqualTo(expectedColor), "Expected Red for point in set with maxStep=1 (Color) due to NaN distance behavior.");
        }

        [Test]
        public void GetBitmap_MaxStep1_PointOutsideSet_ColorCoded_IsRed_DueToNaNDistance()
        {
            var expectedColor = new SKColor(255, 0, 0, ExpectedAlpha);
            var (centerX, centerY, width) = GetParamsForSinglePointTest(3, 0);
            using var bitmap = Mandelbrot.GetBitmap(
                bitmapWidth: 1, bitmapHeight: 1,
                figureCenterX: centerX, figureCenterY: centerY, figureWidth: width,
                maxStep: 1, useDistanceColorCoding: true);

            SKColor pixelColor = bitmap.GetPixel(0, 0);
            Assert.That(pixelColor, Is.EqualTo(expectedColor), "Expected Red for point outside set with maxStep=1 (Color) due to NaN distance behavior.");
        }
    }
}
