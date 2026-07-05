using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RanksScreenController : MonoBehaviour
{
    public TMP_InputField nicknameField;
    public ScrollRect scrollRect;
    public RankRowView rowTemplate;

    TreasureHuntApp app;
    RankRowView playerRow;

    public void Init(TreasureHuntApp app)
    {
        this.app = app;
        nicknameField.text = app.Nickname;
        nicknameField.onValueChanged.AddListener(app.SetNickname);
        playerRow = Instantiate(rowTemplate, rowTemplate.transform.parent);
        playerRow.gameObject.SetActive(true);
        Refresh();
    }

    public void Refresh()
    {
        if (playerRow == null)
            return;
        playerRow.Bind(1, app.Nickname, app.Score, true);
    }

    void OnEnable()
    {
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }
}
