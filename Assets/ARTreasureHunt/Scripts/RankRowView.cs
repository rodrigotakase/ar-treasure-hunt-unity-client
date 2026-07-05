using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankRowView : MonoBehaviour
{
    public Image borderImage;
    public Image fillImage;
    public TMP_Text rankText;
    public TMP_Text nameText;
    public TMP_Text scoreText;

    static readonly Color meFill = new Color32(32, 28, 19, 255);
    static readonly Color otherFill = new Color32(15, 15, 17, 255);
    static readonly Color meBorder = new Color(232f / 255f, 185f / 255f, 78f / 255f, 0.6f);
    static readonly Color otherBorder = new Color(244f / 255f, 239f / 255f, 228f / 255f, 0.08f);
    static readonly Color gold = new Color32(232, 185, 78, 255);
    static readonly Color ivory = new Color32(244, 239, 228, 255);

    public void Bind(int rank, string playerName, int score, bool me)
    {
        rankText.text = rank.ToString();
        nameText.text = playerName;
        scoreText.text = score.ToString();
        fillImage.color = me ? meFill : otherFill;
        borderImage.color = me ? meBorder : otherBorder;
        nameText.color = me ? gold : ivory;
    }
}
