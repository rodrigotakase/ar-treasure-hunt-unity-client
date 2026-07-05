using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RanksScreenController : MonoBehaviour
{
    public TMP_InputField nicknameField;
    public ScrollRect scrollRect;
    public RankRowView rowTemplate;

    TreasureHuntApp app;
    readonly List<RankRowView> rows = new List<RankRowView>();

    public void Init(TreasureHuntApp app)
    {
        this.app = app;
        nicknameField.text = app.Nickname;
        nicknameField.onEndEdit.AddListener(app.SetNickname);
        Refresh();
    }

    public void Refresh()
    {
        if (app == null)
            return;
        app.api.GetLeaderboard(app.huntId, OnLoaded, error =>
        {
            Debug.LogWarning("Could not load leaderboard: " + error);
        });
    }

    void OnLoaded(Leaderboard data)
    {
        foreach (var row in rows)
            Destroy(row.gameObject);
        rows.Clear();

        foreach (var entry in data.leaderboard)
        {
            var row = Instantiate(rowTemplate, rowTemplate.transform.parent);
            row.gameObject.SetActive(true);
            string name = string.IsNullOrEmpty(entry.nickname) ? "Anonymous" : entry.nickname;
            bool me = !string.IsNullOrEmpty(entry.nickname) && entry.nickname == app.Nickname;
            row.Bind(entry.rank, name, entry.collected, me);
            rows.Add(row);
        }
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    void OnEnable()
    {
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }
}
