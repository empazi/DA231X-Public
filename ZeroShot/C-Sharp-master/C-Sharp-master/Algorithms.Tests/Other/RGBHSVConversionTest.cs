using Algorithms.Other;
using NUnit.Framework;
using System;

namespace Algorithms.Tests.Other
{
    [TestFixture]
    public class RGBHSVConversionTests
    {
        [Test]
        public void HsvToRgb_ValidInput_ReturnsCorrectRgb()
        {
            // Test cases with known HSV to RGB conversions
            (byte Red, byte Green, byte Blue) rgb = RgbHsvConversion.HsvToRgb(0, 1, 1);
            Assert.That(rgb, Is.EqualTo((255, 0, 0)), "Test Case 1 Failed: Red");

            rgb = RgbHsvConversion.HsvToRgb(120, 1, 1);
            Assert.That(rgb, Is.EqualTo((0, 255, 0)), "Test Case 2 Failed: Green");

            rgb = RgbHsvConversion.HsvToRgb(240, 1, 1);
            Assert.That(rgb, Is.EqualTo((0, 0, 255)), "Test Case 3 Failed: Blue");

            rgb = RgbHsvConversion.HsvToRgb(0, 0, 1);
            Assert.That(rgb, Is.EqualTo((255, 255, 255)), "Test Case 4 Failed: White");

            rgb = RgbHsvConversion.HsvToRgb(0, 0, 0);
            Assert.That(rgb, Is.EqualTo((0, 0, 0)), "Test Case 5 Failed: Black");

            rgb = RgbHsvConversion.HsvToRgb(30, 1, 1);
            Assert.That(rgb, Is.EqualTo((255, 128, 0)), "Test Case 6 Failed: Orange"); // Note: 255 * 0.5 = 127.5, rounds to 128.

            rgb = RgbHsvConversion.HsvToRgb(60, 1, 1);
            Assert.That(rgb, Is.EqualTo((255, 255, 0)), "Test Case 7 Failed: Yellow");

            rgb = RgbHsvConversion.HsvToRgb(180, 1, 1);
            Assert.That(rgb, Is.EqualTo((0, 255, 255)), "Test Case 8 Failed: Cyan");

            rgb = RgbHsvConversion.HsvToRgb(300, 1, 1);
            Assert.That(rgb, Is.EqualTo((255, 0, 255)), "Test Case 9 Failed: Magenta");

            rgb = RgbHsvConversion.HsvToRgb(210, 0.5, 0.75);
            // chroma = 0.75 * 0.5 = 0.375
            // hueSection = 210 / 60 = 3.5
            // secondLargestComponent = 0.375 * (1 - Math.Abs(3.5 % 2 - 1)) = 0.375 * (1 - Math.Abs(1.5 - 1)) = 0.375 * (1 - 0.5) = 0.375 * 0.5 = 0.1875
            // matchValue = 0.75 - 0.375 = 0.375
            // Section 3.5 (hueSection > 3 && hueSection <= 4):
            // R = matchValue = 0.375 -> 255 * 0.375 = 95.625 -> 96
            // G = secondLargestComponent + matchValue = 0.1875 + 0.375 = 0.5625 -> 255 * 0.5625 = 143.4375 -> 143 (or 144 if rounding rule is different)
            // B = chroma + matchValue = 0.375 + 0.375 = 0.75 -> 255 * 0.75 = 191.25 -> 191
            // The original test expected (96, 144, 191). Let's re-verify ConvertToByte: (byte)Math.Round(255 * input)
            // G: Math.Round(255 * 0.5625) = Math.Round(143.4375) = 143.
            // The original expected (96, 144, 191) might have a slight discrepancy or different rounding for G.
            // With Math.Round: (96, 143, 191)
            // Let's assume the implementation's Math.Round is the source of truth.
            Assert.That(rgb, Is.EqualTo((96, 143, 191)), "Test Case 10 Failed: Light Blue");

            // Test with hue = 360 (boundary)
            rgb = RgbHsvConversion.HsvToRgb(360, 1, 1);
            Assert.That(rgb, Is.EqualTo((255, 0, 0)), "Test Case 11 Failed: Red (Hue 360)");
        }

        [Test]
        public void HsvToRgb_InvalidHue_ThrowsArgumentOutOfRangeException()
        {
            Assert.That(() => RgbHsvConversion.HsvToRgb(-1, 1, 1),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("hue"));
            Assert.That(() => RgbHsvConversion.HsvToRgb(360.1, 1, 1), // hue > 360
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("hue"));
        }

        [Test]
        public void HsvToRgb_InvalidSaturation_ThrowsArgumentOutOfRangeException()
        {
            Assert.That(() => RgbHsvConversion.HsvToRgb(0, -0.1, 1),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("saturation"));
            Assert.That(() => RgbHsvConversion.HsvToRgb(0, 1.1, 1),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("saturation"));
        }

        [Test]
        public void HsvToRgb_InvalidValue_ThrowsArgumentOutOfRangeException()
        {
            Assert.That(() => RgbHsvConversion.HsvToRgb(0, 1, -0.1),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("value"));
            Assert.That(() => RgbHsvConversion.HsvToRgb(0, 1, 1.1),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("value"));
        }

        [Test]
        public void RgbToHsv_ValidInput_ReturnsCorrectHsv()
        {
            // Test cases with known RGB to HSV conversions
            // Using a tolerance for floating point comparisons in HSV results
            const double tolerance = 0.0001; // General tolerance for H, S, V
            const double hueTolerance = 0.5; // Hue can sometimes have larger variance due to % 360 and chroma near zero

            (double h, double s, double v) hsv = RgbHsvConversion.RgbToHsv(255, 0, 0); // Red
            Assert.That(hsv.h, Is.EqualTo(0).Within(hueTolerance), "Test Case 1 Failed: Red (Hue)");
            Assert.That(hsv.s, Is.EqualTo(1).Within(tolerance), "Test Case 1 Failed: Red (Saturation)");
            Assert.That(hsv.v, Is.EqualTo(1).Within(tolerance), "Test Case 1 Failed: Red (Value)");

            hsv = RgbHsvConversion.RgbToHsv(0, 255, 0); // Green
            Assert.That(hsv.h, Is.EqualTo(120).Within(hueTolerance), "Test Case 2 Failed: Green (Hue)");
            Assert.That(hsv.s, Is.EqualTo(1).Within(tolerance), "Test Case 2 Failed: Green (Saturation)");
            Assert.That(hsv.v, Is.EqualTo(1).Within(tolerance), "Test Case 2 Failed: Green (Value)");

            hsv = RgbHsvConversion.RgbToHsv(0, 0, 255); // Blue
            Assert.That(hsv.h, Is.EqualTo(240).Within(hueTolerance), "Test Case 3 Failed: Blue (Hue)");
            Assert.That(hsv.s, Is.EqualTo(1).Within(tolerance), "Test Case 3 Failed: Blue (Saturation)");
            Assert.That(hsv.v, Is.EqualTo(1).Within(tolerance), "Test Case 3 Failed: Blue (Value)");

            hsv = RgbHsvConversion.RgbToHsv(255, 255, 255); // White
            Assert.That(hsv.h, Is.EqualTo(0).Within(hueTolerance), "Test Case 4 Failed: White (Hue)"); // Hue is 0 when chroma is 0
            Assert.That(hsv.s, Is.EqualTo(0).Within(tolerance), "Test Case 4 Failed: White (Saturation)");
            Assert.That(hsv.v, Is.EqualTo(1).Within(tolerance), "Test Case 4 Failed: White (Value)");

            hsv = RgbHsvConversion.RgbToHsv(0, 0, 0); // Black
            Assert.That(hsv.h, Is.EqualTo(0).Within(hueTolerance), "Test Case 5 Failed: Black (Hue)"); // Hue is 0 when chroma is 0
            Assert.That(hsv.s, Is.EqualTo(0).Within(tolerance), "Test Case 5 Failed: Black (Saturation)");
            Assert.That(hsv.v, Is.EqualTo(0).Within(tolerance), "Test Case 5 Failed: Black (Value)");

            hsv = RgbHsvConversion.RgbToHsv(255, 128, 0); // Orange (R=255, G=128, B=0)
            // dR=1, dG=128/255=0.50196, dB=0. V=1. Chroma=1-0=1.
            // H = 60 * ( (dG-dB)/Chroma ) = 60 * (0.50196 / 1) = 30.1176
            Assert.That(hsv.h, Is.EqualTo(30.1176).Within(hueTolerance), "Test Case 6 Failed: Orange (Hue)");
            Assert.That(hsv.s, Is.EqualTo(1).Within(tolerance), "Test Case 6 Failed: Orange (Saturation)");
            Assert.That(hsv.v, Is.EqualTo(1).Within(tolerance), "Test Case 6 Failed: Orange (Value)");

            hsv = RgbHsvConversion.RgbToHsv(255, 255, 0); // Yellow
            Assert.That(hsv.h, Is.EqualTo(60).Within(hueTolerance), "Test Case 7 Failed: Yellow (Hue)");
            Assert.That(hsv.s, Is.EqualTo(1).Within(tolerance), "Test Case 7 Failed: Yellow (Saturation)");
            Assert.That(hsv.v, Is.EqualTo(1).Within(tolerance), "Test Case 7 Failed: Yellow (Value)");

            hsv = RgbHsvConversion.RgbToHsv(0, 255, 255); // Cyan
            Assert.That(hsv.h, Is.EqualTo(180).Within(hueTolerance), "Test Case 8 Failed: Cyan (Hue)");
            Assert.That(hsv.s, Is.EqualTo(1).Within(tolerance), "Test Case 8 Failed: Cyan (Saturation)");
            Assert.That(hsv.v, Is.EqualTo(1).Within(tolerance), "Test Case 8 Failed: Cyan (Value)");

            hsv = RgbHsvConversion.RgbToHsv(255, 0, 255); // Magenta
            Assert.That(hsv.h, Is.EqualTo(300).Within(hueTolerance), "Test Case 9 Failed: Magenta (Hue)");
            Assert.That(hsv.s, Is.EqualTo(1).Within(tolerance), "Test Case 9 Failed: Magenta (Saturation)");
            Assert.That(hsv.v, Is.EqualTo(1).Within(tolerance), "Test Case 9 Failed: Magenta (Value)");

            hsv = RgbHsvConversion.RgbToHsv(96, 143, 191); // Light Blue (using the re-calculated values from HsvToRgb)
            // R=96/255=0.37647, G=143/255=0.56078, B=191/255=0.74902
            // V = 0.74902 (B)
            // Min = 0.37647 (R)
            // Chroma = V - Min = 0.74902 - 0.37647 = 0.37255
            // S = Chroma / V = 0.37255 / 0.74902 = 0.49738
            // H = 60 * (4 + (dR - dG) / Chroma) = 60 * (4 + (0.37647 - 0.56078) / 0.37255)
            // H = 60 * (4 + (-0.18431) / 0.37255) = 60 * (4 - 0.49472) = 60 * 3.50528 = 210.3168
            Assert.That(hsv.h, Is.EqualTo(210.3168).Within(hueTolerance), "Test Case 10 Failed: Light Blue (Hue)");
            Assert.That(hsv.s, Is.EqualTo(0.49738).Within(tolerance), "Test Case 10 Failed: Light Blue (Saturation)"); // Original was 0.5
            Assert.That(hsv.v, Is.EqualTo(0.74902).Within(tolerance), "Test Case 10 Failed: Light Blue (Value)"); // Original was 0.75
        }
    }
}
