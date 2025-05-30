using Algorithms.Other;
using NUnit.Framework;
using System;

namespace Algorithms.Tests.Other;

[TestFixture]
public class RgbHsvConversionTests
{
    // Test cases for HsvToRgb

    [Test]
    public void HsvToRgb_Black()
    {
        var (r, g, b) = RgbHsvConversion.HsvToRgb(0, 0, 0);
        Assert.That(r, Is.EqualTo(0));
        Assert.That(g, Is.EqualTo(0));
        Assert.That(b, Is.EqualTo(0));
    }

    [Test]
    public void HsvToRgb_White()
    {
        var (r, g, b) = RgbHsvConversion.HsvToRgb(0, 0, 1);
        Assert.That(r, Is.EqualTo(255));
        Assert.That(g, Is.EqualTo(255));
        Assert.That(b, Is.EqualTo(255));
    }

    [Test]
    public void HsvToRgb_Red()
    {
        var (r, g, b) = RgbHsvConversion.HsvToRgb(0, 1, 1);
        Assert.That(r, Is.EqualTo(255));
        Assert.That(g, Is.EqualTo(0));
        Assert.That(b, Is.EqualTo(0));
    }

    [Test]
    public void HsvToRgb_Green()
    {
        var (r, g, b) = RgbHsvConversion.HsvToRgb(120, 1, 1);
        Assert.That(r, Is.EqualTo(0));
        Assert.That(g, Is.EqualTo(255));
        Assert.That(b, Is.EqualTo(0));
    }

    [Test]
    public void HsvToRgb_Blue()
    {
        var (r, g, b) = RgbHsvConversion.HsvToRgb(240, 1, 1);
        Assert.That(r, Is.EqualTo(0));
        Assert.That(g, Is.EqualTo(0));
        Assert.That(b, Is.EqualTo(255));
    }

    [Test]
    public void HsvToRgb_Yellow()
    {
        var (r, g, b) = RgbHsvConversion.HsvToRgb(60, 1, 1);
        Assert.That(r, Is.EqualTo(255));
        Assert.That(g, Is.EqualTo(255));
        Assert.That(b, Is.EqualTo(0));
    }

    [Test]
    public void HsvToRgb_Cyan()
    {
        var (r, g, b) = RgbHsvConversion.HsvToRgb(180, 1, 1);
        Assert.That(r, Is.EqualTo(0));
        Assert.That(g, Is.EqualTo(255));
        Assert.That(b, Is.EqualTo(255));
    }

    [Test]
    public void HsvToRgb_Magenta()
    {
        var (r, g, b) = RgbHsvConversion.HsvToRgb(300, 1, 1);
        Assert.That(r, Is.EqualTo(255));
        Assert.That(g, Is.EqualTo(0));
        Assert.That(b, Is.EqualTo(255));
    }

    [Test]
    public void HsvToRgb_Gray()
    {
        var (r, g, b) = RgbHsvConversion.HsvToRgb(0, 0, 0.5);
        Assert.That(r, Is.EqualTo(128)); // Mid-gray
        Assert.That(g, Is.EqualTo(128));
        Assert.That(b, Is.EqualTo(128));
    }

    [Test]
    public void HsvToRgb_ThrowsException_WhenHueIsOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => RgbHsvConversion.HsvToRgb(-1, 0.5, 0.5));
        Assert.Throws<ArgumentOutOfRangeException>(() => RgbHsvConversion.HsvToRgb(361, 0.5, 0.5));
    }

    [Test]
    public void HsvToRgb_ThrowsException_WhenSaturationIsOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => RgbHsvConversion.HsvToRgb(180, -0.1, 0.5));
        Assert.Throws<ArgumentOutOfRangeException>(() => RgbHsvConversion.HsvToRgb(180, 1.1, 0.5));
    }

    [Test]
    public void HsvToRgb_ThrowsException_WhenValueIsOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => RgbHsvConversion.HsvToRgb(180, 0.5, -0.1));
        Assert.Throws<ArgumentOutOfRangeException>(() => RgbHsvConversion.HsvToRgb(180, 0.5, 1.1));
    }

    // Test cases for RgbToHsv

    [Test]
    public void RgbToHsv_Black()
    {
        var (h, s, v) = RgbHsvConversion.RgbToHsv(0, 0, 0);
        Assert.That(h, Is.EqualTo(0).Within(0.001));
        Assert.That(s, Is.EqualTo(0).Within(0.001));
        Assert.That(v, Is.EqualTo(0).Within(0.001));
    }

    [Test]
    public void RgbToHsv_White()
    {
        var (h, s, v) = RgbHsvConversion.RgbToHsv(255, 255, 255);
        Assert.That(h, Is.EqualTo(0).Within(0.001));
        Assert.That(s, Is.EqualTo(0).Within(0.001));
        Assert.That(v, Is.EqualTo(1).Within(0.001));
    }

    [Test]
    public void RgbToHsv_Red()
    {
        var (h, s, v) = RgbHsvConversion.RgbToHsv(255, 0, 0);
        Assert.That(h, Is.EqualTo(0).Within(0.001));
        Assert.That(s, Is.EqualTo(1).Within(0.001));
        Assert.That(v, Is.EqualTo(1).Within(0.001));
    }

    [Test]
    public void RgbToHsv_Green()
    {
        var (h, s, v) = RgbHsvConversion.RgbToHsv(0, 255, 0);
        Assert.That(h, Is.EqualTo(120).Within(0.001));
        Assert.That(s, Is.EqualTo(1).Within(0.001));
        Assert.That(v, Is.EqualTo(1).Within(0.001));
    }

    [Test]
    public void RgbToHsv_Blue()
    {
        var (h, s, v) = RgbHsvConversion.RgbToHsv(0, 0, 255);
        Assert.That(h, Is.EqualTo(240).Within(0.001));
        Assert.That(s, Is.EqualTo(1).Within(0.001));
        Assert.That(v, Is.EqualTo(1).Within(0.001));
    }

    [Test]
    public void RgbToHsv_Gray()
    {
        var (h, s, v) = RgbHsvConversion.RgbToHsv(128, 128, 128);
        Assert.That(h, Is.EqualTo(0).Within(0.001)); // Hue is 0 for grays
        Assert.That(s, Is.EqualTo(0).Within(0.001));
        Assert.That(v, Is.EqualTo(128.0 / 255.0).Within(0.001));
    }
}
