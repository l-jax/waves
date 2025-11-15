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
            2,     // Sub Bass/Bass (0-43 Hz)
            4,     // Mid Bass (43-86 Hz)
            8,     // Low Midrange (86-172 Hz)
            16,    // Midrange (172-344 Hz)
            32,    // High Midrange (344-689 Hz)
            64,    // Low Treble (689-1378 Hz)
            128,   // Mid Treble (1378-2756 Hz)
            256,   // High Treble/Air (2756-5512 Hz)
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

            // Calculate the average power and scale it
            // We use logarithm to compress the wide dynamic range of audio power.
            averagePower /= bandWidth;
            eightBandData[i] = Mathf.Clamp(averagePower * 10f, 0f, 1f); // Simple scaling/clamping
        }
        return eightBandData;
    }
}