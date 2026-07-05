using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TreasuresScreenController : MonoBehaviour
{
    public TMP_Text headerText;
    public ScrollRect scrollRect;
    public GemRowView rowTemplate;

    TreasureHuntApp app;
    readonly List<GemRowView> rows = new List<GemRowView>();

    public void Init(TreasureHuntApp app)
    {
        this.app = app;
        Refresh();
    }

    public void Refresh()
    {
        if (app == null)
            return;
        app.api.GetHuntTreasures(app.huntId, OnLoaded, error =>
        {
            Debug.LogWarning("Could not load treasures: " + error);
        });
    }

    void OnLoaded(HuntTreasures data)
    {
        foreach (var row in rows)
            Destroy(row.gameObject);
        rows.Clear();

        int collected = 0;
        foreach (var treasure in data.treasures)
        {
            var row = Instantiate(rowTemplate, rowTemplate.transform.parent);
            row.gameObject.SetActive(true);
            row.Bind(treasure, app.showHints);
            rows.Add(row);
            if (treasure.collected)
                collected++;
        }
        headerText.text = "TREASURES - " + collected + " / " + data.treasures.Length;
        app.UpdateScore(collected);
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    void OnEnable()
    {
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }
}
