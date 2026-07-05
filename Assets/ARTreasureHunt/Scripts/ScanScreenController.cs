using UnityEngine;

public class ScanScreenController : MonoBehaviour
{
    public GameObject scanningGroup;
    public GameObject doneGroup;
    public RectTransform scanLine;

    TreasureHuntApp app;

    public void Init(TreasureHuntApp app)
    {
        this.app = app;
    }

    public void ArmScan()
    {
        bool allFound = app.NextGem() == null && GemCatalog.All.Length > 0;
        doneGroup.SetActive(allFound);
        scanningGroup.SetActive(!allFound);
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
