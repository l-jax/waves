using System;

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
}