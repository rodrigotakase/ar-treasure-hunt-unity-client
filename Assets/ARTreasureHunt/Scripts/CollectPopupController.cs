using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectPopupController : MonoBehaviour
{
    public Image gemImage;
    public TMP_Text titleText;
    public TMP_Text nameText;
    public TMP_Text locationText;
    public GameObject pointsBadge;
    public Button continueButton;

    TreasureHuntApp app;

    public void Init(TreasureHuntApp app)
    {
        this.app = app;
        continueButton.onClick.AddListener(Close);
    }

    public void Show(CollectedTreasure treasure, bool alreadyCollected)
    {
        if (ColorUtility.TryParseHtmlString(treasure.color, out var color))
            gemImage.color = color;
        titleText.text = alreadyCollected ? "ALREADY COLLECTED" : "GEM COLLECTED";
        nameText.text = treasure.name;
        locationText.text = string.IsNullOrEmpty(treasure.location) ? treasure.hunt_name : treasure.location;
        pointsBadge.SetActive(!alreadyCollected);
        gameObject.SetActive(true);
    }

    void Close()
    {
        gameObject.SetActive(false);
        app.OnPopupClosed();
    }
}
