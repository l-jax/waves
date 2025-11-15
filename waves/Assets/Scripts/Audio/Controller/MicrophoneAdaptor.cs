using UnityEngine;

public enum Direction
{
    Left,
    Right,
    Stationary
}

[RequireComponent(typeof(AudioSource))]
public class MicrophoneAdaptor : MonoBehaviour
{
    public float CurrentVolume => _currentVolume;
    public float[] SampleBuffer => _sampleBuffer;

    private AudioSource _audioSource;
    private MicrophoneInput _microphoneInput;
    private float _backgroundVolume;
    private float _midVolume;
    private float _currentVolume;
    private float[] _sampleBuffer;

    public void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _microphoneInput = new MicrophoneInput(_audioSource);   
        _microphoneInput.Initialize();
        _sampleBuffer = _microphoneInput.SampleBuffer;
    }
    
    public void Update()
    {
        _microphoneInput.Update();
        _currentVolume = _microphoneInput.GetVolume();
        _sampleBuffer = _microphoneInput.SampleBuffer;
    }

    public void SetVolumeThresholds(float backgroundVolume, float midVolume)
    {
        _backgroundVolume = backgroundVolume;
        _midVolume = midVolume;
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

    public float[] GetEightBandSpectrum()
    {
        float[] spectrum = _microphoneInput.GetSpectrumData();
        return ProcessSpectrumToEightBands(spectrum);
    }

    private float[] ProcessSpectrumToEightBands(float[] spectrum)
    {
        float[] eightBandData = new float[8];
        int[] bandLimits = new int[] {
            2, 4, 8, 16, 32, 64, 128, 256
        };

        for (int i = 0; i < 8; i++)
        {
            float averagePower = 0f;
            int startSample = (i == 0) ? 0 : bandLimits[i-1];
            int endSample = bandLimits[i];
            int bandWidth = endSample - startSample;

            // Sum the power (amplitude squared) within the band
            for (int j = startSample; j < endSample; j++)
            {
                averagePower += spectrum[j];
            }

            averagePower /= bandWidth;
            float scaledPower = averagePower * 50f; // Multiply for visibility
            float logPower = Mathf.Log10(scaledPower + 1f); // Add 1 to avoid Log(0)
            eightBandData[i] = Mathf.Clamp(logPower, 0f, 1f);
        }
        
        return eightBandData;
    }
}