using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GemRowView : MonoBehaviour
{
    public Image borderImage;
    public Image fillImage;
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text subText;
    public GameObject foundBadge;

    static readonly Color foundFill = new Color32(18, 18, 20, 255);
    static readonly Color lockedFill = new Color32(15, 15, 17, 255);
    static readonly Color foundBorder = new Color(232f / 255f, 185f / 255f, 78f / 255f, 0.35f);
    static readonly Color lockedBorder = new Color(244f / 255f, 239f / 255f, 228f / 255f, 0.08f);
    static readonly Color lockedIcon = new Color32(51, 51, 58, 255);
    static readonly Color ivory = new Color32(244, 239, 228, 255);
    static readonly Color muted = new Color32(156, 149, 133, 255);
    static readonly Color dim = new Color32(110, 106, 95, 255);
    static readonly Color dimmer = new Color32(84, 81, 74, 255);

    public void Bind(HuntTreasure treasure, bool showHints)
    {
        bool found = treasure.collected;
        nameText.text = treasure.name;
        if (found)
            subText.text = string.IsNullOrEmpty(treasure.location) ? "Found" : "Found · " + treasure.location;
        else if (showHints && !string.IsNullOrEmpty(treasure.hint))
            subText.text = "Hint: " + treasure.hint;
        else
            subText.text = "Hint: · · ·";

        Color treasureColor;
        if (!ColorUtility.TryParseHtmlString(treasure.color, out treasureColor))
            treasureColor = ivory;
        icon.color = found ? treasureColor : lockedIcon;
        fillImage.color = found ? foundFill : lockedFill;
        borderImage.color = found ? foundBorder : lockedBorder;
        nameText.color = found ? ivory : dim;
        subText.color = found ? muted : dimmer;
        foundBadge.SetActive(found);
    }
}
