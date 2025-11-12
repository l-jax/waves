using System;

[Serializable]
public class CalibrationData
{
    public float MaxRecordedBackground;
    public float AvgRecordedQuiet;
    public float AvgRecordedLoud;

    private const float _defaultMaxRecordedBackground = 0.002f;
    private const float _defaultMidVolume = 0.05f;
    private const float _safetyMargin = 1.2f;

    public float GetBackgroundVolume()
    {
        if (MaxRecordedBackground <= 0)
        {
            return _defaultMaxRecordedBackground;
        }

        return MaxRecordedBackground * _safetyMargin;
    }

    public float GetMidVolume()
    {
        if (AvgRecordedQuiet <= MaxRecordedBackground || AvgRecordedLoud <= AvgRecordedQuiet)
        {
            return _defaultMidVolume;
        }

        return (AvgRecordedQuiet + AvgRecordedLoud) / 2f;
    }
}