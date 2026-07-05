using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TreasuresScreenController : MonoBehaviour
{
    public TMP_Text headerText;
    public ScrollRect scrollRect;
    public GemRowView rowTemplate;

    TreasureHuntApp app;
    GemRowView[] rows;

    public void Init(TreasureHuntApp app)
    {
        this.app = app;
        rows = new GemRowView[GemCatalog.All.Length];
        for (int i = 0; i < rows.Length; i++)
        {
            rows[i] = Instantiate(rowTemplate, rowTemplate.transform.parent);
            rows[i].gameObject.SetActive(true);
        }
        Refresh();
    }

    public void Refresh()
    {
        if (rows == null)
            return;
        headerText.text = "TREASURES - " + app.Score + " / " + GemCatalog.All.Length;
        for (int i = 0; i < rows.Length; i++)
            rows[i].Bind(GemCatalog.All[i], app.IsFound(GemCatalog.All[i].id), app.showHints);
    }

    void OnEnable()
    {
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }
}
