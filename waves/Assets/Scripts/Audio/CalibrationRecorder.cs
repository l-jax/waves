using System;

public class CalibrationRecorder
{
    private readonly Func<float> _volumeProvider;
    private float _accumulatedVolume;
    private int _frameCount;

    public CalibrationRecorder(Func<float> volumeProvider)
    {
        _volumeProvider = volumeProvider;
    }

    public void StartRecording()
    {
        _accumulatedVolume = 0;
        _frameCount = 0;
    }

    public void UpdateRecording()
    {
        _frameCount++;
        _accumulatedVolume += _volumeProvider();
    }

    public float GetResult()
    {
        return _frameCount > 0 ? _accumulatedVolume / _frameCount : 0f;
    }
}