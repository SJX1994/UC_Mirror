using UnityEngine;

/// <summary>
/// 颜色工具类
/// </summary>
public static class ColorUtils
{
    /// <summary>
    /// Color转Hex
    /// </summary>
    /// alpha:是否有透明度
    public static string Color2Hex(Color color, bool alpha = true)
    {
        string hex;
        if (alpha)
        {
            hex = ColorUtility.ToHtmlStringRGBA(color);
        }
        else
        {
            hex = ColorUtility.ToHtmlStringRGB(color);
        }
        return hex;
    }

    /// <summary>
    /// Hex转Color
    /// </summary>
    /// Hex：#000000
    public static Color HexRGB2Color(string hexRGB)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hexRGB, out color);
        return color;
    }

    /// <summary>
    /// Color转HSV
    /// </summary>
    public static void Color2HSV(Color color, out float h, out float s, out float v)
    {
        Color.RGBToHSV(color, out h, out s, out v);
    }

    /// <summary>
    /// HSV转Color
    /// </summary>
    public static Color HSV2Color(float h, float s, float v)
    {
        Color color = Color.HSVToRGB(h, s, v);
        return color;
    }

    /// <summary>
    /// 获取特定颜色
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static Color GetColor(EventType.UnitColor info)
    {
        var color = new Color();

        if (info == EventType.UnitColor.red)
        {
            color = ColorUtils.HexRGB2Color("#ff0e0e");
        }

        if (info == EventType.UnitColor.green)
        {
            color = ColorUtils.HexRGB2Color("#3bd218");
        }

        if (info == EventType.UnitColor.blue)
        {
            color = ColorUtils.HexRGB2Color("#272fd5"); 
        }

        if (info == EventType.UnitColor.purple)
        {
            color = ColorUtils.HexRGB2Color("#8645FF");
        }

        return color;
    }
}