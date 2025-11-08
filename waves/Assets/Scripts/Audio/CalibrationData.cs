using System;

[Serializable]
public class CalibrationData
{
    public float MinVolume;
    public float MidVolume;
    public float MaxVolume;
    public float Speed;

    public bool IsValid() => 
        MinVolume >= 0 && 
        MidVolume > MinVolume && 
        MaxVolume > MidVolume;
}