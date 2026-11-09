using UnityEngine;

public static class HpColors
{
    public static readonly Color High = new Color32(0x5C, 0xD8, 0x5A, 0xFF);    // Green > 50%
    public static readonly Color Mid = new Color32(0xFF, 0xC6, 0x1A, 0xFF);     // Gold > 20%
    public static readonly Color Low = new Color32(0xE0, 0x40, 0x40, 0xFF);     // Red <= 20%

    public static Color For(float fillAmount)
    {
        if (fillAmount > 0.5f) return High;
        if (fillAmount > 0.2f) return Mid;
        return Low;
    }
}
