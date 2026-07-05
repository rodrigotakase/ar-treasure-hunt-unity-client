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
    public CrystalSpawner crystalSpawner;

    List<string> found = new List<string>();
    string nickname;
    string scannedTreasureId;
    CrystalBehaviour activeCrystal;
    bool collecting;

    public int Score => found.Count;
    public string Nickname => nickname;

    void Awake()
    {
        var savedFound = PlayerPrefs.GetString("treasureHunt.found", "");
        if (savedFound.Length > 0)
            found.AddRange(savedFound.Split(','));
        found.RemoveAll(id => System.Array.Find(GemCatalog.All, g => g.id == id) == null);
        nickname = PlayerPrefs.GetString("treasureHunt.nickname", "");
        if (string.IsNullOrEmpty(nickname))
        {
            nickname = GenerateNickname();
            Save();
        }
    }

    static string GenerateNickname()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        int length = Random.Range(4, 6);
        var suffix = "";
        for (int i = 0; i < length; i++)
            suffix += chars[Random.Range(0, chars.Length)];
        return "Player#" + suffix;
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
        if (activeCrystal != null)
            activeCrystal.gameObject.SetActive(tab == AppTab.Scan);
        bool idle = activeCrystal == null && !collecting;
        if (qrScanner != null)
        {
            if (tab == AppTab.Scan && idle)
                qrScanner.StartScanning();
            else if (tab != AppTab.Scan)
                qrScanner.StopScanning();
        }
        tabBar.Highlight(tab);
        if (tab == AppTab.Scan && idle)
            scanScreen.ArmScan();
        if (tab == AppTab.Treasures)
            treasuresScreen.Refresh();
        if (tab == AppTab.Ranks)
            ranksScreen.Refresh();
    }

    public Gem NextGem()
    {
        foreach (var gem in GemCatalog.All)
            if (!found.Contains(gem.id))
                return gem;
        return null;
    }

    void OnTreasureScanned(string treasureId)
    {
        collecting = true;
        api.GetTreasure(treasureId, info => SpawnCrystal(treasureId, info), error =>
        {
            Debug.LogWarning("Could not load treasure " + treasureId + ": " + error);
            collecting = false;
            qrScanner.StartScanning();
        });
    }

    void SpawnCrystal(string treasureId, TreasureInfo info)
    {
        scannedTreasureId = treasureId;
        activeCrystal = crystalSpawner.Spawn(info.color);
        if (activeCrystal == null)
        {
            Debug.LogWarning("Could not spawn crystal for treasure " + treasureId);
            collecting = false;
            qrScanner.StartScanning();
            return;
        }
        activeCrystal.Shattered += OnCrystalShattered;
        activeCrystal.Tapped += OnCrystalTapped;
        qrScanner.StopScanning();
        scanScreen.ShowTapUI(info.name, activeCrystal.tapsToShatter);
        Debug.Log("Crystal spawned for " + info.name + " at " + activeCrystal.transform.position);
    }

    void OnCrystalTapped(int taps)
    {
        if (activeCrystal != null)
            scanScreen.UpdateTapProgress(taps, activeCrystal.tapsToShatter);
    }

    void OnCrystalShattered()
    {
        activeCrystal = null;
        scanScreen.HideTapUI();
        api.Collect(scannedTreasureId, nickname, OnCollected, error =>
        {
            Debug.LogWarning("Could not collect treasure " + scannedTreasureId + ": " + error);
            collecting = false;
            scanScreen.ArmScan();
            qrScanner.StartScanning();
        });
    }

    void OnCollected(CollectResult result)
    {
        treasuresScreen.Refresh();
        ranksScreen.Refresh();
        collectPopup.Show(result.treasure, result.alreadyCollected);
    }

    public void OnPopupClosed()
    {
        collecting = false;
        ShowTab(AppTab.Treasures);
    }

    public void SetNickname(string value)
    {
        if (string.IsNullOrEmpty(value))
            return;
        nickname = value;
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
