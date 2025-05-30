using NUnit.Framework;
using Algorithms.Other;
using SkiaSharp;
using System;

namespace Algorithms.Tests.Other
{
    [TestFixture]
    public class MandelbrotTests
    {
        private const byte ExpectedAlpha = 255; // Matches the private Alpha in Mandelbrot.cs
#pragma warning disable IDE1006 // Naming Styles
        private readonly SKColor Black = new SKColor(0, 0, 0, ExpectedAlpha);
        private readonly SKColor White = new SKColor(255, 255, 255, ExpectedAlpha);
        // For maxStep = 1, GetDistance returns NaN. ColorCodedColorMap(NaN) results in Red.
        private readonly SKColor RedForNaN = new SKColor(255, 0, 0, ExpectedAlpha);
#pragma warning restore IDE1006 // Naming Styles

        [Test]
        public void GetBitmap_WithDefaultParameters_ReturnsBitmapWithCorrectDimensions()
        {
            var bitmap = Mandelbrot.GetBitmap();
            Assert.That(bitmap, Is.Not.Null);
            Assert.That(bitmap.Width, Is.EqualTo(800));
            Assert.That(bitmap.Height, Is.EqualTo(600));
        }

        [Test]
        public void GetBitmap_WithCustomParameters_ReturnsBitmapWithCorrectDimensions()
        {
            var bitmap = Mandelbrot.GetBitmap(bitmapWidth: 100, bitmapHeight: 50, figureWidth: 3.0, maxStep: 10);
            Assert.That(bitmap, Is.Not.Null);
            Assert.That(bitmap.Width, Is.EqualTo(100));
            Assert.That(bitmap.Height, Is.EqualTo(50));
        }

        [TestCase(0, 100, 10)]
        [TestCase(-1, 100, 10)]
        public void GetBitmap_InvalidBitmapWidth_ThrowsArgumentOutOfRangeException(int width, int height, int maxStep)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Mandelbrot.GetBitmap(bitmapWidth: width, bitmapHeight: height, maxStep: maxStep));
            Assert.That(ex!.ParamName, Is.EqualTo("bitmapWidth"));
        }

        [TestCase(100, 0, 10)]
        [TestCase(100, -1, 10)]
        public void GetBitmap_InvalidBitmapHeight_ThrowsArgumentOutOfRangeException(int width, int height, int maxStep)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Mandelbrot.GetBitmap(bitmapWidth: width, bitmapHeight: height, maxStep: maxStep));
            Assert.That(ex!.ParamName, Is.EqualTo("bitmapHeight"));
        }

        [TestCase(100, 100, 0)]
        [TestCase(100, 100, -1)]
        public void GetBitmap_InvalidMaxStep_ThrowsArgumentOutOfRangeException(int width, int height, int maxStep)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Mandelbrot.GetBitmap(bitmapWidth: width, bitmapHeight: height, maxStep: maxStep));
            Assert.That(ex!.ParamName, Is.EqualTo("maxStep"));
        }

        private SKColor GetPixelForComplexPoint(double cx, double cy, int maxStep, bool useDistanceColorCoding)
        {
            // Use a 1x1 bitmap. The figure coordinates will be calculated for bitmapX=0, bitmapY=0.
            // figureX = figureCenterX + ((0.0 / 1.0) - 0.5) * figureWidth = figureCenterX - 0.5 * figureWidth
            // To make (figureX, figureY) effectively (cx, cy), we set figureCenterX = cx, figureCenterY = cy,
            // and use a very small figureWidth. figureHeight is derived from figureWidth.
            var bitmap = Mandelbrot.GetBitmap(
                bitmapWidth: 1,
                bitmapHeight: 1,
                figureCenterX: cx,
                figureCenterY: cy,
                figureWidth: 1e-9, // Very small width to focus on the (cx, cy) point
                maxStep: maxStep,
                useDistanceColorCoding: useDistanceColorCoding);
            return bitmap.GetPixel(0, 0);
        }

        [Test]
        public void GetBitmap_PointInSet_0_0_BlackAndWhite_IsBlack()
        {
            // c = 0 + 0i is in the Mandelbrot set. Distance should be 1.0 (for maxStep > 1)
            var color = GetPixelForComplexPoint(cx: 0, cy: 0, maxStep: 50, useDistanceColorCoding: false);
            Assert.That(color, Is.EqualTo(Black));
        }

        [Test]
        public void GetBitmap_PointOutsideSet_2_0_BlackAndWhite_IsWhite()
        {
            // c = 2 + 0i is outside. Distance should be 0.0 (for maxStep > 1)
            var color = GetPixelForComplexPoint(cx: 2, cy: 0, maxStep: 50, useDistanceColorCoding: false);
            Assert.That(color, Is.EqualTo(White));
        }

        [Test]
        public void GetBitmap_PointInSet_0_0_ColorCoded_IsBlack()
        {
            // c = 0 + 0i is in the set. Distance 1.0 (for maxStep > 1)
            var color = GetPixelForComplexPoint(cx: 0, cy: 0, maxStep: 50, useDistanceColorCoding: true);
            Assert.That(color, Is.EqualTo(Black));
        }

        [Test]
        public void GetBitmap_PointOutsideSet_2_0_ColorCoded_IsRed()
        {
            // c = 2 + 0i is outside. Distance 0.0 (for maxStep > 1)
            // ColorCodedColorMap(0.0) -> Red
            var color = GetPixelForComplexPoint(cx: 2, cy: 0, maxStep: 50, useDistanceColorCoding: true);
            var expectedColorForD0 = new SKColor(255, 0, 0, ExpectedAlpha); // Red
            Assert.That(color, Is.EqualTo(expectedColorForD0));
        }

        [Test]
        public void GetBitmap_Point_m1_0_InSet_ColorCoded_IsBlack()
        {
            // c = -1 + 0i is in the set (oscillates -1, 0). Distance 1.0 (for maxStep > 1)
            var color = GetPixelForComplexPoint(cx: -1, cy: 0, maxStep: 50, useDistanceColorCoding: true);
            Assert.That(color, Is.EqualTo(Black));
        }

        [Test]
        public void GetBitmap_Point_1_0_Escapes_ColorCoded_IsSpecificColor()
        {
            // c = 1 + 0i.
            // In Mandelbrot.GetDistance, for this point, currentStep becomes 1 when it escapes.
            // For maxStep=50, distance = 1.0 / (50-1.0) = 1.0/49.0.
            int maxStep = 50;
            double cx = 1;
            double cy = 0;
            var color = GetPixelForComplexPoint(cx, cy, maxStep, useDistanceColorCoding: true);

            double distance = 1.0 / (maxStep - 1.0);
            SKColor expectedColor = CalculateExpectedColorCodedColor(distance);
            Assert.That(color, Is.EqualTo(expectedColor));
        }

        // Tests for maxStep = 1 behavior (GetDistance returns NaN due to division by zero)
        [Test]
        public void GetBitmap_MaxStepIsOne_PointInSet_BlackAndWhite_IsWhite_DueToNaN()
        {
            // c = 0 + 0i. With maxStep = 1, GetDistance returns NaN.
            // BlackAndWhiteColorMap(NaN) -> white (NaN >= 1 is false)
            var color = GetPixelForComplexPoint(cx: 0, cy: 0, maxStep: 1, useDistanceColorCoding: false);
            Assert.That(color, Is.EqualTo(White));
        }

        [Test]
        public void GetBitmap_MaxStepIsOne_PointOutsideSet_BlackAndWhite_IsWhite_DueToNaN()
        {
            // c = 2 + 0i. With maxStep = 1, GetDistance returns NaN.
            var color = GetPixelForComplexPoint(cx: 2, cy: 0, maxStep: 1, useDistanceColorCoding: false);
            Assert.That(color, Is.EqualTo(White));
        }

        [Test]
        public void GetBitmap_MaxStepIsOne_PointInSet_ColorCoded_IsRed_DueToNaN()
        {
            // c = 0 + 0i. With maxStep = 1, GetDistance returns NaN.
            // ColorCodedColorMap(NaN) -> Red
            var color = GetPixelForComplexPoint(cx: 0, cy: 0, maxStep: 1, useDistanceColorCoding: true);
            Assert.That(color, Is.EqualTo(RedForNaN));
        }

        [Test]
        public void GetBitmap_MaxStepIsOne_PointOutsideSet_ColorCoded_IsRed_DueToNaN()
        {
            // c = 2 + 0i. With maxStep = 1, GetDistance returns NaN.
            var color = GetPixelForComplexPoint(cx: 2, cy: 0, maxStep: 1, useDistanceColorCoding: true);
            Assert.That(color, Is.EqualTo(RedForNaN));
        }

        // Helper to replicate ColorCodedColorMap logic for testing specific colors
        private SKColor CalculateExpectedColorCodedColor(double distance)
        {
            if (distance >= 1) return Black; // Matches Mandelbrot.cs

            var hue = 360 * distance;
            double saturation = 1;
            double val = 255;
            var hi = (int)Math.Floor(hue / 60) % 6;
            var f = hue / 60 - Math.Floor(hue / 60);

            var v_byte = (byte)val;
            const byte p_byte = 0;
            var q_byte = (byte)(val * (1 - f * saturation));
            var t_byte = (byte)(val * (1 - (1 - f) * saturation));

            switch (hi)
            {
                case 0: return new SKColor(v_byte, t_byte, p_byte, ExpectedAlpha);
                case 1: return new SKColor(q_byte, v_byte, p_byte, ExpectedAlpha);
                case 2: return new SKColor(p_byte, v_byte, t_byte, ExpectedAlpha);
                case 3: return new SKColor(p_byte, q_byte, v_byte, ExpectedAlpha);
                case 4: return new SKColor(t_byte, p_byte, v_byte, ExpectedAlpha);
                default: return new SKColor(v_byte, p_byte, q_byte, ExpectedAlpha);
            }
        }
    }
}
