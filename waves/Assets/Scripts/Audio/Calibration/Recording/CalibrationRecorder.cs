using System;
using UnityEngine;

public class CalibrationRecorder
{
    private Func<float> _volumeProvider;
    private float _duration;
    private float _elapsedTime;
    private float _accumulatedVolume;
    private float _maxVolume;
    private int _sampleCount;
    private bool _isRecording;
    
    public bool IsRecording => _isRecording;
    public bool IsComplete => _isRecording && _elapsedTime >= _duration;
    public float ElapsedTime => _elapsedTime;
    public float Progress => _duration > 0 ? Mathf.Clamp01(_elapsedTime / _duration) : 0f;
    
    public void StartRecording(Func<float> volumeProvider, float duration)
    {
        if (_isRecording)
        {
            throw new InvalidOperationException("Already recording");
        }
        
        _volumeProvider = volumeProvider ?? throw new ArgumentNullException(nameof(volumeProvider));
        _duration = duration;
        _elapsedTime = 0f;
        _accumulatedVolume = 0f;
        _maxVolume = 0f;
        _sampleCount = 0;
        _isRecording = true;
    }

    public void UpdateRecording(float deltaTime)
    {
        if (!_isRecording) return;

        _elapsedTime += deltaTime;
        float currentVolume = _volumeProvider();
        _accumulatedVolume += currentVolume;
        _maxVolume = Mathf.Max(_maxVolume, currentVolume);
        _sampleCount++;
    }

    public void StopRecording()
    {
        _isRecording = false;
    }
    
    public RecordingResult GetResult()
    {
        return new RecordingResult
        {
            AverageVolume = _sampleCount > 0 ? _accumulatedVolume / _sampleCount : 0f,
            MaxVolume = _maxVolume,
            SampleCount = _sampleCount,
            Duration = _elapsedTime,
            WasCompleted = IsComplete
        };
    }
}

public struct RecordingResult
{
    public float AverageVolume;
    public float MaxVolume;
    public int SampleCount;
    public float Duration;
    public bool WasCompleted;
}