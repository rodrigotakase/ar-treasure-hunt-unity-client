using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabBarController : MonoBehaviour
{
    [Serializable]
    public class Tab
    {
        public Button button;
        public CanvasGroup group;
        public TMP_Text label;
        public Graphic[] icons;
        public Image frame;
    }

    public Tab treasures;
    public Tab scan;
    public Tab ranks;

    static readonly Color active = new Color32(232, 185, 78, 255);
    static readonly Color inactive = new Color32(244, 239, 228, 255);
    static readonly Color frameOn = new Color(232f / 255f, 185f / 255f, 78f / 255f, 0.08f);

    TreasureHuntApp app;

    public void Init(TreasureHuntApp app)
    {
        this.app = app;
        treasures.button.onClick.AddListener(() => this.app.ShowTab(AppTab.Treasures));
        scan.button.onClick.AddListener(() => this.app.ShowTab(AppTab.Scan));
        ranks.button.onClick.AddListener(() => this.app.ShowTab(AppTab.Ranks));
    }

    public void Highlight(AppTab tab)
    {
        Apply(treasures, tab == AppTab.Treasures);
        Apply(scan, tab == AppTab.Scan);
        Apply(ranks, tab == AppTab.Ranks);
    }

    void Apply(Tab tab, bool on)
    {
        var color = on ? active : inactive;
        tab.group.alpha = on ? 1f : 0.42f;
        tab.label.color = color;
        foreach (var icon in tab.icons)
            icon.color = color;
        if (tab.frame != null)
            tab.frame.color = on ? frameOn : Color.clear;
    }
}
