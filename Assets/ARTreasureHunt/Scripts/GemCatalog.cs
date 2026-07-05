using UnityEngine;

public class Gem
{
    public string id;
    public string displayName;
    public string location;
    public string hint;
    public Color color;

    public Gem(string id, string displayName, string location, string hint, string hex)
    {
        this.id = id;
        this.displayName = displayName;
        this.location = location;
        this.hint = hint;
        ColorUtility.TryParseHtmlString(hex, out color);
    }
}

public static class GemCatalog
{
    public static readonly Gem[] All =
    {
    };
}
