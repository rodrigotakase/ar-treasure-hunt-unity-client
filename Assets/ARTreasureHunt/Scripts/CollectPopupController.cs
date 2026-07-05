using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectPopupController : MonoBehaviour
{
    public Image gemImage;
    public TMP_Text nameText;
    public TMP_Text locationText;
    public Button continueButton;

    TreasureHuntApp app;

    public void Init(TreasureHuntApp app)
    {
        this.app = app;
        continueButton.onClick.AddListener(Close);
    }

    public void Show(Gem gem)
    {
        gemImage.color = gem.color;
        nameText.text = gem.displayName;
        locationText.text = "Found in the " + gem.location;
        gameObject.SetActive(true);
    }

    void Close()
    {
        gameObject.SetActive(false);
        app.OnPopupClosed();
    }
}
