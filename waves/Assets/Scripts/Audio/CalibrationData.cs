using System;

[Serializable]
public class CalibrationData
{
    public float SilenceThreshold;
    public float MidpointLoudness;
    public float MaxLoudness;
    public float Speed;

    public bool IsValid() => 
        SilenceThreshold >= 0 && 
        MidpointLoudness > SilenceThreshold && 
        MaxLoudness > MidpointLoudness;
}