using UnityEngine;

public enum Direction
{
    Left,
    Right,
    Stationary
}

public class MicrophoneAdaptor
{
    public float CurrentVolume => _currentVolume;
    public float[] SampleBuffer => _sampleBuffer;

    private readonly MicrophoneInput _microphoneInput;
    private readonly float _backgroundVolume;
    private readonly float _midVolume;
    private float _currentVolume;
    private float[] _sampleBuffer;


    public MicrophoneAdaptor(AudioSource audioSource, CalibrationData calibrationData)
    {
        _backgroundVolume = calibrationData.GetBackgroundVolume();
        _midVolume = calibrationData.GetMidVolume();
        _microphoneInput = new MicrophoneInput(audioSource);
        _microphoneInput.Initialize();
        _sampleBuffer = _microphoneInput.SampleBuffer;
    }

    public void Update()
    {
        _microphoneInput.Update();
        _currentVolume = _microphoneInput.GetVolume();
        _sampleBuffer = _microphoneInput.SampleBuffer;
    }

    public Direction GetDirection()
    {
        if (_currentVolume < _backgroundVolume)
        {
            return Direction.Stationary;
        }

        if (_currentVolume < _midVolume)
        {
            return Direction.Left;
        }
        return Direction.Right;
    }
}