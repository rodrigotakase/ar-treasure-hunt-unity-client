using UnityEngine;

public class CrystalSpawner : MonoBehaviour
{
    public Camera arCamera;
    public GameObject crystalPrefab;
    public float spawnDistance = 1f;

    public CrystalBehaviour Spawn(string colorHex)
    {
        if (crystalPrefab == null || arCamera == null)
            return null;

        var position = arCamera.transform.position + arCamera.transform.forward * spawnDistance;
        var toCamera = arCamera.transform.position - position;
        toCamera.y = 0f;
        var rotation = toCamera.sqrMagnitude > 0.001f ? Quaternion.LookRotation(toCamera) : Quaternion.identity;

        var instance = Instantiate(crystalPrefab, position, rotation);
        var crystal = instance.GetComponent<CrystalBehaviour>();
        if (crystal == null)
        {
            Destroy(instance);
            return null;
        }
        crystal.SetEmissionColor(colorHex);
        return crystal;
    }
}
