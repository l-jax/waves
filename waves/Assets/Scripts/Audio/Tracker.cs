using UnityEngine;
using System;

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

    public int[] GetEqualiserBands() => _equaliserBands;
    
    [Tooltip("Current paddle direction (Left, Right, Stationary)")]
    public Direction CurrentDirection { get; private set; }

    [Header("Control Settings")]
    [SerializeField] private ControlSettings _controlSettings;

    [Header("Microphone Settings")]
    [SerializeField] private MicrophoneSettings _microphoneSettings;

    [Header("Debug Info (Read-Only)")]
    [SerializeField] private float _currentVolume;

    private Input _microphone;
    private int[] _equaliserBands;

    void Start()
    {
        _equaliserBands = new int[8];
        CurrentDirection = Direction.Stationary;
        _microphone = new Input(GetComponent<AudioSource>(), _microphoneSettings);
        _microphone.Initialize();
    }

    void Update()
    {
        _microphone.Update();
        _currentVolume = _microphone.GetVolume();
        SetDirection(_currentVolume);
        _equaliserBands = ConvertSpectrumDataToEqualizerBands(_microphone.GetSpectrumData());
    }

    private int[] ConvertSpectrumDataToEqualizerBands(float[] data)
    {
        // Sanity check to ensure we have the correct input size
        if (data == null || data.Length != 64)
        {
            throw new ArgumentException("Input array must contain exactly 64 samples.", nameof(data));
        }

        int[] equalizerBands = new int[8];

        // Loop through the 8 desired equalizer bands
        for (int i = 0; i < 8; i++)
        {
            float currentSum = 0f;
            int startIndex = i * 8; // Start index for the group (0, 8, 16, 24, ...)

            // Sum the 8 samples for the current band
            for (int j = 0; j < 8; j++)
            {
                currentSum += data[startIndex + j];
            }

            // Calculate the average (sum / 8) 
            // transform the data to an integer value between 0 and 11
            // and store it in the result array
            equalizerBands[i] = Mathf.FloorToInt((currentSum / 8) * 11f);
        }

        return equalizerBands;
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
        _controlSettings.SilenceThreshold = data.MinVolume;
        _controlSettings.MidpointLoudness = data.MidVolume;
        _controlSettings.MaxLoudness = data.MaxVolume;
        _controlSettings.MaxSpeed = data.Speed;
        
        Debug.Log($"Calibration applied: Silence={data.MinVolume:F4}, " +
                  $"Mid={data.MidVolume:F4}, Max={data.MaxVolume:F4}, Speed={data.Speed:F2}");
    }
}