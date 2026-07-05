using UnityEngine;

public class SafeAreaFitter : MonoBehaviour
{
    Rect applied;

    void Awake()
    {
        Apply();
    }

    void Update()
    {
        if (Screen.safeArea != applied)
            Apply();
    }

    void Apply()
    {
        applied = Screen.safeArea;
        var rt = (RectTransform)transform;
        rt.anchorMin = new Vector2(0f, applied.yMin / Screen.height);
        rt.anchorMax = new Vector2(1f, applied.yMax / Screen.height);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
