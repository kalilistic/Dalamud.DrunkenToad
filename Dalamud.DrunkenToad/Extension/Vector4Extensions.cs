using System.Numerics;

using FFXIVClientStructs.FFXIV.Client.Graphics;

namespace Dalamud.DrunkenToad;

/// <summary>
/// Vector4 extensions.
/// </summary>
public static class Vector4Extensions
{
    /// <summary>
    /// Convert Vector4 to byte color struct.
    /// </summary>
    /// <param name="value">vector4 color.</param>
    /// <returns>bytecolor struct.</returns>
    public static ByteColor ToByteColor(this Vector4 value)
    {
        return new ByteColor { A = (byte)(value.W * 255), R = (byte)(value.X * 255), G = (byte)(value.Y * 255), B = (byte)(value.Z * 255) };
    }
}
