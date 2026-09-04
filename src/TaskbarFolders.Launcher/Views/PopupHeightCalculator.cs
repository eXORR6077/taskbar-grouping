using System;

namespace TaskbarFolders.Launcher.Views;

/// <summary>
/// Pure popup-height math extracted from <see cref="PopupWindow"/> so strip-aware
/// sizing can be unit-tested without constructing a <see cref="System.Windows.Window"/>.
/// </summary>
public static class PopupHeightCalculator
{
    /// <summary>
    /// Computes the popup window height from the tile grid plus an optional launch-failure
    /// strip, then clamps to <paramref name="bounds"/>.
    /// </summary>
    /// <param name="rows">Grid row count; values below 1 are treated as 1 (banner-only layout).</param>
    /// <param name="tilePx">Tile width/height in DIPs.</param>
    /// <param name="paddingPx">Outer chrome padding on each side in DIPs.</param>
    /// <param name="stripHeight">
    /// Measured height of the error strip including its margin, in DIPs.
    /// Pass 0 when the strip is collapsed — otherwise a docked strip would steal
    /// space from the fixed-height tiles after initial sizing.
    /// </param>
    /// <param name="bounds">Inclusive min/max height clamp (typically the Window's Min/MaxHeight).</param>
    /// <returns>Height in DIPs suitable for <c>Window.Height</c>.</returns>
    public static double CalculatePopupHeight(
        int rows,
        int tilePx,
        int paddingPx,
        double stripHeight,
        PopupHeightBounds bounds)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(tilePx);
        ArgumentOutOfRangeException.ThrowIfNegative(paddingPx);
        ArgumentOutOfRangeException.ThrowIfNegative(stripHeight);
        if (bounds.Min < 0 || bounds.Max < bounds.Min || double.IsNaN(bounds.Min) || double.IsNaN(bounds.Max)
            || double.IsInfinity(bounds.Min) || double.IsInfinity(bounds.Max))
        {
            throw new ArgumentOutOfRangeException(nameof(bounds), bounds, "Min and Max must be finite with Max >= Min >= 0.");
        }

        // Grid alone was historically the whole Height; the strip docks below it and must
        // be added or MaxHeight clamping happens after the tiles have already been clipped.
        var gridHeight = Math.Max(rows, 1) * (double)tilePx + (2.0 * paddingPx);
        var total = gridHeight + stripHeight;
        return Math.Clamp(total, bounds.Min, bounds.Max);
    }
}

/// <summary>Inclusive height clamp for <see cref="PopupHeightCalculator.CalculatePopupHeight"/>.</summary>
/// <param name="Min">Lower bound (typically <c>Window.MinHeight</c>).</param>
/// <param name="Max">Upper bound (typically <c>Window.MaxHeight</c>).</param>
public readonly record struct PopupHeightBounds(double Min, double Max);
