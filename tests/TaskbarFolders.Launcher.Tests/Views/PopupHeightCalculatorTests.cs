using FluentAssertions;
using TaskbarFolders.Launcher.Views;
using Xunit;

namespace TaskbarFolders.Launcher.Tests.Views;

/// <summary>
/// Pure-math coverage for <see cref="PopupHeightCalculator"/> — with and without the
/// launch-failure strip that previously clipped fixed-height tiles.
/// </summary>
public class PopupHeightCalculatorTests
{
    private const int TilePx = 96;
    private const int PaddingPx = 12;
    private static readonly PopupHeightBounds _defaultBounds = new(Min: 120, Max: 800);

    [Fact]
    public void CalculatePopupHeight_WithoutStrip_MatchesGridOnlySizing()
    {
        // 2 rows: 2*96 + 2*12 = 216
        var height = PopupHeightCalculator.CalculatePopupHeight(
            rows: 2, tilePx: TilePx, paddingPx: PaddingPx, stripHeight: 0, bounds: _defaultBounds);

        height.Should().Be(216);
    }

    [Fact]
    public void CalculatePopupHeight_WithStrip_GrowsByStripHeight()
    {
        const double stripHeight = 40;
        // 2 rows + strip: 216 + 40 = 256
        var height = PopupHeightCalculator.CalculatePopupHeight(
            rows: 2, tilePx: TilePx, paddingPx: PaddingPx, stripHeight: stripHeight, bounds: _defaultBounds);

        height.Should().Be(256);
    }

    [Fact]
    public void CalculatePopupHeight_ZeroRows_FallsBackToOneRowThenClampsToMinHeight()
    {
        // 1*96 + 24 = 120 → equals MinHeight
        var height = PopupHeightCalculator.CalculatePopupHeight(
            rows: 0, tilePx: TilePx, paddingPx: PaddingPx, stripHeight: 0, bounds: _defaultBounds);

        height.Should().Be(120);
    }

    [Fact]
    public void CalculatePopupHeight_RespectsMaxHeight()
    {
        // Many rows would exceed MaxHeight; clamp must win even when a strip is present.
        var height = PopupHeightCalculator.CalculatePopupHeight(
            rows: 20, tilePx: TilePx, paddingPx: PaddingPx, stripHeight: 50, bounds: _defaultBounds);

        height.Should().Be(800);
    }

    [Fact]
    public void CalculatePopupHeight_RejectsNegativeStripHeight()
    {
        var act = () => PopupHeightCalculator.CalculatePopupHeight(
            rows: 1, tilePx: TilePx, paddingPx: PaddingPx, stripHeight: -1, bounds: _defaultBounds);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
