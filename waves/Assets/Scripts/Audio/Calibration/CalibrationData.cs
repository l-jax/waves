using System;
using UnityEngine;

[Serializable]
public class CalibrationData
{
    public float BackgroundVolume;
    public float MinVolume;
    public float MaxVolume;

    public bool IsValid() =>
        BackgroundVolume >= 0 &&
        MinVolume > BackgroundVolume &&
        MaxVolume > MinVolume;

    public void ApplyDefaults()
    {
        BackgroundVolume = 0.002f;
        MinVolume = 0.05f;
        MaxVolume = 0.2f;
    }
    
    public void Sanitize()
    {
        BackgroundVolume = Mathf.Max(0f, BackgroundVolume);
        MinVolume = Mathf.Max(BackgroundVolume * 1.5f, MinVolume);
        MaxVolume = Mathf.Max(MinVolume * 1.5f, MaxVolume);
    }
}