using NUnit.Framework;
using Algorithms.Other;
using System;

namespace Algorithms.Tests.Other
{
    [TestFixture]
    public class RgbHsvConversionTests
    {
        // Tolerance for comparing Saturation and Value components
        private const double DeltaSV = 0.002; // Max error approx 0.5/255

        // Tolerance for comparing Hue component. Can be larger due to RGB byte rounding and chroma influence.
        private const double DeltaH = 0.7; // Max error approx 60 * (1/255) / chroma_min for S=1, V=1

        static object[] HsvToRgb_KnownColors_TestCases =
        {
            // HSV input (h, s, v), Expected RGB output (r, g, b)
            new object[] { 0.0, 0.0, 0.0, (byte)0, (byte)0, (byte)0 },     // Black
            new object[] { 0.0, 0.0, 1.0, (byte)255, (byte)255, (byte)255 }, // White
            new object[] { 180.0, 0.0, 0.5, (byte)128, (byte)128, (byte)128 },// Gray (H can be anything for S=0)
            new object[] { 0.0, 1.0, 1.0, (byte)255, (byte)0, (byte)0 },     // Red
            new object[] { 60.0, 1.0, 1.0, (byte)255, (byte)255, (byte)0 },  // Yellow
            new object[] { 120.0, 1.0, 1.0, (byte)0, (byte)255, (byte)0 },   // Green
            new object[] { 180.0, 1.0, 1.0, (byte)0, (byte)255, (byte)255 }, // Cyan
            new object[] { 240.0, 1.0, 1.0, (byte)0, (byte)0, (byte)255 },   // Blue
            new object[] { 300.0, 1.0, 1.0, (byte)255, (byte)0, (byte)255 }, // Magenta
            new object[] { 360.0, 1.0, 1.0, (byte)255, (byte)0, (byte)0 },   // Red (Hue 360)
            // Specific hue sections (S=1, V=1)
            new object[] { 30.0, 1.0, 1.0, (byte)255, (byte)128, (byte)0 },  // Orange
            new object[] { 90.0, 1.0, 1.0, (byte)128, (byte)255, (byte)0 },  // Chartreuse
            new object[] { 150.0, 1.0, 1.0, (byte)0, (byte)255, (byte)128 }, // Spring Green
            new object[] { 210.0, 1.0, 1.0, (byte)0, (byte)128, (byte)255 }, // Azure
            new object[] { 270.0, 1.0, 1.0, (byte)128, (byte)0, (byte)255 }, // Violet
            new object[] { 330.0, 1.0, 1.0, (byte)255, (byte)0, (byte)128 }, // Rose
            // Saturation = 0
            new object[] { 150.0, 0.0, 0.8, (byte)204, (byte)204, (byte)204 }, // Gray, 0.8*255 = 204.5 -> 205. Let's check: Math.Round(255 * 0.8) = Math.Round(204.0) = 204. Correct.
            // Value = 0
            new object[] { 150.0, 0.5, 0.0, (byte)0, (byte)0, (byte)0 }      // Black (Value 0)
        };

        [TestCaseSource(nameof(HsvToRgb_KnownColors_TestCases))]
        public void HsvToRgb_KnownColors_ConvertsCorrectly(double h, double s, double v, byte expectedR, byte expectedG, byte expectedB)
        {
            var (actualR, actualG, actualB) = RgbHsvConversion.HsvToRgb(h, s, v);

            Assert.That(actualR, Is.EqualTo(expectedR), "Red component mismatch.");
            Assert.That(actualG, Is.EqualTo(expectedG), "Green component mismatch.");
            Assert.That(actualB, Is.EqualTo(expectedB), "Blue component mismatch.");
        }

        [Test]
        public void HsvToRgb_HueOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            Assert.That(() => RgbHsvConversion.HsvToRgb(-0.1, 1, 1),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("hue"));
            Assert.That(() => RgbHsvConversion.HsvToRgb(360.1, 1, 1),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("hue"));
        }

        [Test]
        public void HsvToRgb_SaturationOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            Assert.That(() => RgbHsvConversion.HsvToRgb(180, -0.1, 1),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("saturation"));
            Assert.That(() => RgbHsvConversion.HsvToRgb(180, 1.1, 1),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("saturation"));
        }

        [Test]
        public void HsvToRgb_ValueOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            Assert.That(() => RgbHsvConversion.HsvToRgb(180, 1, -0.1),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("value"));
            Assert.That(() => RgbHsvConversion.HsvToRgb(180, 1, 1.1),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("value"));
        }


        static object[] RgbToHsv_KnownColors_TestCases =
        {
            // RGB input (r, g, b), Expected HSV output (h, s, v)
            new object[] { (byte)0, (byte)0, (byte)0, 0.0, 0.0, 0.0 },         // Black
            new object[] { (byte)255, (byte)255, (byte)255, 0.0, 0.0, 1.0 },     // White (Hue is 0 by convention when S=0)
            new object[] { (byte)128, (byte)128, (byte)128, 0.0, 0.0, 128.0/255.0 }, // Gray
            new object[] { (byte)255, (byte)0, (byte)0, 0.0, 1.0, 1.0 },         // Red
            new object[] { (byte)255, (byte)255, (byte)0, 60.0, 1.0, 1.0 },      // Yellow
            new object[] { (byte)0, (byte)255, (byte)0, 120.0, 1.0, 1.0 },       // Green
            new object[] { (byte)0, (byte)255, (byte)255, 180.0, 1.0, 1.0 },     // Cyan
            new object[] { (byte)0, (byte)0, (byte)255, 240.0, 1.0, 1.0 },       // Blue
            new object[] { (byte)255, (byte)0, (byte)255, 300.0, 1.0, 1.0 },     // Magenta
            
            // Colors derived from HsvToRgb results (testing precision)
            // HsvToRgb(30,1,1) -> (255,128,0). RgbToHsv(255,128,0) -> H = 60*(128/255)/1 = 30.1176...
            new object[] { (byte)255, (byte)128, (byte)0, 30.11764705882353, 1.0, 1.0 }, // Orange
            // HsvToRgb(90,1,1) -> (128,255,0). RgbToHsv(128,255,0) -> H = 60*(2+(0-128/255)/( (255-0)/255 )) = 60*(2-128/255) = 89.8823...
            new object[] { (byte)128, (byte)255, (byte)0, 89.88235294117647, 1.0, 1.0 }, // Chartreuse
            // HsvToRgb(150,1,1) -> (0,255,128). RgbToHsv(0,255,128) -> H = 60*(2+(128/255-0)/1) = 150.1176...
            new object[] { (byte)0, (byte)255, (byte)128, 150.11764705882354, 1.0, 1.0 },// Spring Green
            // HsvToRgb(210,1,1) -> (0,128,255). RgbToHsv(0,128,255) -> H = 60*(4+(0-128/255)/((255-0)/255)) = 60*(4-128/255) = 209.8823...
            new object[] { (byte)0, (byte)128, (byte)255, 209.88235294117646, 1.0, 1.0 },// Azure
            // HsvToRgb(270,1,1) -> (128,0,255). RgbToHsv(128,0,255) -> H = 60*(4+(128/255-0)/1) = 270.1176...
            new object[] { (byte)128, (byte)0, (byte)255, 270.11764705882354, 1.0, 1.0 },// Violet
            // HsvToRgb(330,1,1) -> (255,0,128). RgbToHsv(255,0,128) -> H = 60*(0+(0-128/255)/1) + 360 = 329.8823...
            new object[] { (byte)255, (byte)0, (byte)128, 329.88235294117646, 1.0, 1.0 },// Rose

            // A less saturated color: RGB(100, 150, 200)
            // dR=100/255, dG=150/255, dB=200/255. V=200/255. Chroma=(200-100)/255=100/255. S= (100/255)/(200/255) = 0.5
            // H = 60*(4 + (100/255 - 150/255) / (100/255) ) = 60*(4 + (-50/255)/(100/255)) = 60*(4 - 50/100) = 60*(4-0.5) = 60*3.5 = 210
            new object[] { (byte)100, (byte)150, (byte)200, 210.0, 0.5, 200.0/255.0 }
        };

        [TestCaseSource(nameof(RgbToHsv_KnownColors_TestCases))]
        public void RgbToHsv_KnownColors_ConvertsCorrectly(byte r, byte g, byte b, double expectedH, double expectedS, double expectedV)
        {
            var (actualH, actualS, actualV) = RgbHsvConversion.RgbToHsv(r, g, b);

            Assert.That(actualH, Is.EqualTo(expectedH).Within(DeltaH), "Hue component mismatch.");
            Assert.That(actualS, Is.EqualTo(expectedS).Within(DeltaSV), "Saturation component mismatch.");
            Assert.That(actualV, Is.EqualTo(expectedV).Within(DeltaSV), "Value component mismatch.");
        }

        [Test]
        public void RgbToHsv_WhenChromaIsZero_HueIsZero()
        {
            // Grayscale colors should have Hue = 0, Saturation = 0
            var (h1, s1, v1) = RgbHsvConversion.RgbToHsv(50, 50, 50);
            Assert.That(h1, Is.EqualTo(0.0).Within(DeltaH));
            Assert.That(s1, Is.EqualTo(0.0).Within(DeltaSV));
            Assert.That(v1, Is.EqualTo(50.0 / 255.0).Within(DeltaSV));

            var (h2, s2, v2) = RgbHsvConversion.RgbToHsv(0, 0, 0); // Black
            Assert.That(h2, Is.EqualTo(0.0).Within(DeltaH));
            Assert.That(s2, Is.EqualTo(0.0).Within(DeltaSV));
            Assert.That(v2, Is.EqualTo(0.0).Within(DeltaSV));

            var (h3, s3, v3) = RgbHsvConversion.RgbToHsv(255, 255, 255); // White
            Assert.That(h3, Is.EqualTo(0.0).Within(DeltaH));
            Assert.That(s3, Is.EqualTo(0.0).Within(DeltaSV));
            Assert.That(v3, Is.EqualTo(1.0).Within(DeltaSV));
        }

        [Test]
        public void RgbToHsv_HueCalculation_HandlesNegativeIntermediateCorrectly()
        {
            // Test case where (dGreen - dBlue) or similar term is negative,
            // requiring the (hue + 360) % 360 adjustment.
            // Example: RGB(255, 0, 128) (Rose/Deep Pink)
            // dR=1, dG=0, dB=128/255. Max=dR. Chroma=1.
            // Hue_intermediate = 60 * ( (dG - dB) / Chroma ) = 60 * ( (0 - 128/255)/1 ) = 60 * (-128/255) = -30.117...
            // Hue_final = (-30.117... + 360) % 360 = 329.882...
            byte r = 255, g = 0, b = 128;
            double expectedH = 329.88235294117646;
            double expectedS = 1.0;
            double expectedV = 1.0;

            var (actualH, actualS, actualV) = RgbHsvConversion.RgbToHsv(r, g, b);

            Assert.That(actualH, Is.EqualTo(expectedH).Within(DeltaH));
            Assert.That(actualS, Is.EqualTo(expectedS).Within(DeltaSV));
            Assert.That(actualV, Is.EqualTo(expectedV).Within(DeltaSV));
        }

        [Test]
        [Description("Tests if converting HSV to RGB and back to HSV yields similar HSV values.")]
        public void HsvToRgbToHsv_RoundTrip_IsConsistent()
        {
            double originalH = 135;
            double originalS = 0.75;
            double originalV = 0.6;

            var (r, g, b) = RgbHsvConversion.HsvToRgb(originalH, originalS, originalV);
            var (finalH, finalS, finalV) = RgbHsvConversion.RgbToHsv(r, g, b);

            // For round trips, hue can differ more significantly if saturation is low or due to RGB quantization.
            // Here, saturation is reasonably high (0.75).
            // A slightly wider tolerance for hue in round trip might be necessary.
            Assert.That(finalH, Is.EqualTo(originalH).Within(DeltaH * 2), "Hue round trip failed.");
            Assert.That(finalS, Is.EqualTo(originalS).Within(DeltaSV * 2), "Saturation round trip failed.");
            Assert.That(finalV, Is.EqualTo(originalV).Within(DeltaSV * 2), "Value round trip failed.");
        }

        [Test]
        [Description("Tests if converting RGB to HSV and back to RGB yields RGB values within +/-1 tolerance due to rounding.")]
        public void RgbToHsvToRgb_RoundTrip_IsConsistent()
        {
            byte originalR = 100;
            byte originalG = 150;
            byte originalB = 200;

            var (h, s, v) = RgbHsvConversion.RgbToHsv(originalR, originalG, originalB);
            var (finalR, finalG, finalB) = RgbHsvConversion.HsvToRgb(h, s, v);

            // Due to Math.Round in HsvToRgb, RGB values might differ by +/- 1 after round trip.
            Assert.That(finalR, Is.EqualTo(originalR).Within(1), "Red round trip failed.");
            Assert.That(finalG, Is.EqualTo(originalG).Within(1), "Green round trip failed.");
            Assert.That(finalB, Is.EqualTo(originalB).Within(1), "Blue round trip failed.");
        }
    }
}
