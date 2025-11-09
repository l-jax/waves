using System;
using UnityEngine;

[Serializable]
public class CalibrationData
{
    public float BackgroundVolume;
    public float MinVolume;
    public float MaxVolume;
    public float Speed;

    public bool IsValid() =>
        BackgroundVolume >= 0 &&
        MinVolume > BackgroundVolume &&
        MaxVolume > MinVolume;

    public void ApplyDefaults()
    {
        BackgroundVolume = 0.01f;
        MinVolume = 0.05f;
        MaxVolume = 0.2f;
        Speed = 1.0f;
    }
    
    public void Sanitize()
    {
        BackgroundVolume = Mathf.Max(0f, BackgroundVolume);
        MinVolume = Mathf.Max(BackgroundVolume * 1.5f, MinVolume);
        MaxVolume = Mathf.Max(MinVolume * 1.5f, MaxVolume);
        Speed = Mathf.Clamp(Speed, 0.5f, 5f);
    }
}