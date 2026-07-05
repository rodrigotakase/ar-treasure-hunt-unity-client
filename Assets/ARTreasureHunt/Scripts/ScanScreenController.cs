using TMPro;
using UnityEngine;

public class ScanScreenController : MonoBehaviour
{
    public GameObject scanningGroup;
    public GameObject tapGroup;
    public RectTransform scanLine;
    public TMP_Text markerLabel;
    public TMP_Text tapLabel;
    public RectTransform tapFill;

    TreasureHuntApp app;

    public void Init(TreasureHuntApp app)
    {
        this.app = app;
    }

    public void ArmScan()
    {
        bool allFound = app.NextGem() == null && GemCatalog.All.Length > 0;
        scanningGroup.SetActive(!allFound);
        if (tapGroup != null)
            tapGroup.SetActive(false);
    }

    public void ShowTapUI(string treasureName, int needed)
    {
        scanningGroup.SetActive(false);
        tapGroup.SetActive(true);
        markerLabel.text = (treasureName ?? "").ToUpper();
        UpdateTapProgress(0, needed);
    }

    public void UpdateTapProgress(int taps, int needed)
    {
        tapLabel.text = "TAP TO FRACTURE · " + taps + "/" + needed;
        float w = 190f * Mathf.Clamp01((float)taps / needed);
        tapFill.sizeDelta = new Vector2(w, 4f);
        tapFill.anchoredPosition = new Vector2(-95f + w * 0.5f, 0f);
    }

    public void HideTapUI()
    {
        if (tapGroup != null)
            tapGroup.SetActive(false);
    }

    void Update()
    {
        if (scanningGroup.activeSelf)
        {
            float t = Mathf.PingPong(Time.time * 170f, 204f);
            scanLine.anchoredPosition = new Vector2(0f, 102f - t);
        }
    }
}
