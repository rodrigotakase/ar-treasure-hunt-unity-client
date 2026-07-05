using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;

public class QrScanner : MonoBehaviour
{
    public ARCameraManager cameraManager;
    public float scanInterval = 0.5f;

    public event Action<string> TreasureFound;

    BarcodeReaderGeneric barcodeReader;
    Task<Result> decodeTask;
    float nextScan;
    bool scanning;

    void Awake()
    {
        barcodeReader = new BarcodeReaderGeneric
        {
            AutoRotate = true,
            Options =
            {
                TryHarder = false,
                PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }
            }
        };
    }

    public void StartScanning()
    {
        scanning = true;
    }

    public void StopScanning()
    {
        scanning = false;
    }

    void Update()
    {
        if (decodeTask != null && decodeTask.IsCompleted)
        {
            HandleDecodeResult();
            decodeTask = null;
        }
        if (!scanning || decodeTask != null || cameraManager == null || Time.time < nextScan)
            return;
        nextScan = Time.time + scanInterval;
        StartDecode();
    }

    void HandleDecodeResult()
    {
        if (decodeTask.IsFaulted)
        {
            Debug.LogWarning("QR decode failed: " + decodeTask.Exception.GetBaseException().Message);
            return;
        }
        var result = decodeTask.Result;
        if (result == null || !scanning)
            return;
        var treasureId = GetTreasureIdFromUrl(result.Text);
        if (string.IsNullOrEmpty(treasureId))
            return;
        StopScanning();
        TreasureFound?.Invoke(treasureId);
    }

    void StartDecode()
    {
        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
            return;
        byte[] pixels;
        int width;
        int height;
        RGBLuminanceSource.BitmapFormat bitmapFormat;
        using (image)
        {
            var textureFormat = TextureFormat.R8;
            bitmapFormat = RGBLuminanceSource.BitmapFormat.Gray8;
            if (!image.FormatSupported(textureFormat))
            {
                textureFormat = TextureFormat.RGBA32;
                bitmapFormat = RGBLuminanceSource.BitmapFormat.RGBA32;
            }
            var conversion = new XRCpuImage.ConversionParams
            {
                inputRect = new RectInt(0, 0, image.width, image.height),
                outputDimensions = new Vector2Int(image.width / 2, image.height / 2),
                outputFormat = textureFormat,
                transformation = XRCpuImage.Transformation.None
            };
            width = conversion.outputDimensions.x;
            height = conversion.outputDimensions.y;
            var buffer = new NativeArray<byte>(image.GetConvertedDataSize(conversion), Allocator.Temp);
            image.Convert(conversion, buffer);
            pixels = buffer.ToArray();
            buffer.Dispose();
        }
        decodeTask = Task.Run(() =>
        {
            var source = new RGBLuminanceSource(pixels, width, height, bitmapFormat);
            return barcodeReader.Decode(source);
        });
    }

    public static string GetTreasureIdFromUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return null;
        var parts = url.Split('=', '/', '?', '&');
        var last = parts[parts.Length - 1].Trim();
        return Guid.TryParse(last, out _) ? last : null;
    }
}
