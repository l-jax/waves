using UnityEngine;

public enum Direction
{
    Left,
    Right,
    Stationary
}

public class MicrophoneAdaptor
{
    public float GetCurrentVolume() => _currentVolume;
    private readonly MicrophoneInput _microphoneInput;
    private readonly float _backgroundVolume;
    private readonly float _midVolume;
    private float _currentVolume;

    public MicrophoneAdaptor(AudioSource audioSource, CalibrationData calibrationData)
    {
        _backgroundVolume = calibrationData.GetBackgroundVolume();
        _midVolume = calibrationData.GetMidVolume();
        _microphoneInput = new MicrophoneInput(audioSource);
        _microphoneInput.Initialize();
    }

    public void Update()
    {
        _microphoneInput.Update();
        _currentVolume = _microphoneInput.GetVolume();
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