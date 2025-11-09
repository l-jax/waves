using UnityEngine;

public enum Direction
{
    Left,
    Right,
    Stationary
}

[RequireComponent(typeof(AudioSource))]
public class Tracker : MonoBehaviour
{
    public float GetCurrentVolume() => _currentVolume;
    
    [Tooltip("Current paddle direction (Left, Right, Stationary)")]
    public Direction CurrentDirection { get; private set; }

    [Header("Control Settings")]
    [SerializeField] private ControlSettings _controlSettings;

    [Header("Microphone Settings")]
    [SerializeField] private MicrophoneSettings _microphoneSettings;

    [Header("Debug Info (Read-Only)")]
    [SerializeField] private float _currentVolume;

    private Input _microphone;

    void Awake()
    {
        CurrentDirection = Direction.Stationary;
        _microphone = new Input(GetComponent<AudioSource>(), _microphoneSettings);
        _microphone.Initialize();
    }

    void Update()
    {
        _microphone.Update();
        _currentVolume = _microphone.GetVolume();
        SetDirection(_currentVolume);
    }

    private void SetDirection(float volume)
    {
        if (volume < _controlSettings.SilenceThreshold)
        {
            CurrentDirection = Direction.Stationary;
            return;
        }

        if (volume < _controlSettings.MidpointLoudness)
        {
            CurrentDirection = Direction.Left;
        }
        else
        {
            CurrentDirection = Direction.Right;
        }
    }

    public void ApplyCalibration(CalibrationData data)
    {
        _controlSettings.SilenceThreshold = data.BackgroundVolume;
        _controlSettings.MidpointLoudness = data.MinVolume;
        _controlSettings.MaxLoudness = data.MaxVolume;

        Debug.Log(
            $"Calibration applied: " +
            $"Silence ={_controlSettings.SilenceThreshold:F4}, " +
            $"Mid={_controlSettings.MidpointLoudness:F4}, " +
            $"Max={_controlSettings.MaxLoudness:F4}, "
        );
    }
}