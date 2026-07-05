using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public enum AppTab { Treasures, Scan, Ranks }

public class TreasureHuntApp : MonoBehaviour
{
    public bool showHints = true;
    public int huntId = 1;

    public TreasureHuntApi api;
    public TMP_Text scoreText;
    public ScanScreenController scanScreen;
    public TreasuresScreenController treasuresScreen;
    public RanksScreenController ranksScreen;
    public TabBarController tabBar;
    public CollectPopupController collectPopup;
    public ARSession arSession;
    public QrScanner qrScanner;

    List<string> found = new List<string>();
    string nickname = "Nickname";

    public int Score => found.Count;
    public string Nickname => nickname;

    void Awake()
    {
        var savedFound = PlayerPrefs.GetString("treasureHunt.found", "");
        if (savedFound.Length > 0)
            found.AddRange(savedFound.Split(','));
        found.RemoveAll(id => System.Array.Find(GemCatalog.All, g => g.id == id) == null);
        nickname = PlayerPrefs.GetString("treasureHunt.nickname", "You");
    }

    void Start()
    {
        tabBar.Init(this);
        scanScreen.Init(this);
        treasuresScreen.Init(this);
        ranksScreen.Init(this);
        collectPopup.Init(this);
        if (qrScanner != null)
            qrScanner.TreasureFound += OnTreasureScanned;
        RefreshScore();
        ShowTab(AppTab.Scan);
        scanScreen.ArmScan();
    }

    public void ShowTab(AppTab tab)
    {
        scanScreen.gameObject.SetActive(tab == AppTab.Scan);
        treasuresScreen.gameObject.SetActive(tab == AppTab.Treasures);
        ranksScreen.gameObject.SetActive(tab == AppTab.Ranks);
        if (arSession != null)
            arSession.enabled = tab == AppTab.Scan;
        if (qrScanner != null)
        {
            if (tab == AppTab.Scan)
                qrScanner.StartScanning();
            else
                qrScanner.StopScanning();
        }
        tabBar.Highlight(tab);
        if (tab == AppTab.Scan)
            scanScreen.ArmScan();
        if (tab == AppTab.Treasures)
            treasuresScreen.Refresh();
        if (tab == AppTab.Ranks)
            ranksScreen.Refresh();
    }

    public bool IsFound(string id)
    {
        return found.Contains(id);
    }

    public Gem NextGem()
    {
        foreach (var gem in GemCatalog.All)
            if (!found.Contains(gem.id))
                return gem;
        return null;
    }

    public void Collect(Gem gem)
    {
        if (!found.Contains(gem.id))
            found.Add(gem.id);
        Save();
        RefreshScore();
        treasuresScreen.Refresh();
        ranksScreen.Refresh();
        collectPopup.Show(gem);
    }

    public void OnPopupClosed()
    {
        scanScreen.ArmScan();
    }

    void OnTreasureScanned(string treasureId)
    {
        Debug.Log("Scanned treasure " + treasureId);
    }

    public void SetNickname(string value)
    {
        nickname = string.IsNullOrEmpty(value) ? "Nickname" : value;
        Save();
        ranksScreen.Refresh();
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    void RefreshScore()
    {
        scoreText.text = Score.ToString();
    }

    void Save()
    {
        PlayerPrefs.SetString("treasureHunt.found", string.Join(",", found));
        PlayerPrefs.SetString("treasureHunt.nickname", nickname);
        PlayerPrefs.Save();
    }
}
