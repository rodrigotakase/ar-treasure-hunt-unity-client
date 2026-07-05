using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrystalBehaviour : MonoBehaviour
{
    public int tapsToShatter = 5;
    public float spinSpeed = 45f;
    public float bobHeight = 0.08f;
    public float bobSpeed = 2f;
    public AudioClip tapClip;
    public AudioClip shatterClip;
    public GameObject shatterEffect;

    public event Action Shattered;
    public event Action<int> Tapped;

    int taps;
    bool shattering;
    Vector3 baseScale;
    Vector3 basePosition;
    Camera arCamera;

    void Start()
    {
        baseScale = transform.localScale;
        basePosition = transform.position;
        arCamera = Camera.main;
    }

    public void SetEmissionColor(string hex)
    {
        if (!ColorUtility.TryParseHtmlString(hex, out var color))
            return;
        var meshRenderer = GetComponentInChildren<MeshRenderer>();
        if (meshRenderer == null)
            return;
        var material = meshRenderer.material;
        material.SetColor("_EmissionColor", color);
        material.SetColor("_ColorTint1", Color.Lerp(color, Color.black, 0.3f));
        material.SetColor("_ColorTint2", Color.Lerp(color, Color.white, 0.3f));
    }

    void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
        transform.position = basePosition + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
        if (transform.localScale.x < baseScale.x)
        {
            float s = Mathf.MoveTowards(transform.localScale.x, baseScale.x, Time.deltaTime * baseScale.x * 1.5f);
            transform.localScale = new Vector3(s, s, s);
        }
        if (shattering || arCamera == null)
            return;
        var pointer = Pointer.current;
        if (pointer == null || !pointer.press.wasPressedThisFrame)
            return;
        var ray = arCamera.ScreenPointToRay(pointer.position.ReadValue());
        if (Physics.Raycast(ray, out var hit, 20f) && hit.transform == transform)
            Tap();
    }

    void Tap()
    {
        taps++;
        transform.localScale = baseScale * 0.85f;
        Tapped?.Invoke(taps);
        if (taps >= tapsToShatter)
        {
            shattering = true;
            Shatter();
        }
        else if (tapClip != null)
        {
            AudioSource.PlayClipAtPoint(tapClip, transform.position);
        }
    }

    void Shatter()
    {
        if (shatterEffect != null)
            Instantiate(shatterEffect, transform.position, Quaternion.identity);
        if (shatterClip != null)
            AudioSource.PlayClipAtPoint(shatterClip, transform.position);
        Shattered?.Invoke();
        Destroy(gameObject);
    }
}
