using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PitchTracker : MonoBehaviour
{
    [Tooltip("The microphone device to use. Leave empty for default.")]
    public string _microphoneDevice;

    [Tooltip("The sample window size for the loudness calculation.")]
    public int _loudnessSampleWindow = 128;

    [Tooltip("The minimum loudness threshold to trigger pitch detection.")]
    public float _pitchDetectionThreshold = 0.001f;

    [Header("Autocorrelation Pitch Tracking")]
    [Tooltip("The sample window size for autocorrelation. Larger windows are better for low frequencies.")]
    public int _autocorrelationSampleWindow = 2048;

    [Tooltip("The minimum frequency (in Hz) to detect. Clamps the search range.")]
    public float _minFrequency = 70f;

    [Tooltip("The maximum frequency (in Hz) to detect. Clamps the search range.")]
    public float _maxFrequency = 800f;

    private AudioSource _audioSource;
    private AudioClip _microphoneClip;
    private string _currentMicrophoneDevice;
    private float[] _micSampleBuffer;
    private int _sampleRate;

    public float CurrentLoudness { get; private set; }
    public float CurrentPitch { get; private set; }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        _micSampleBuffer = new float[_autocorrelationSampleWindow];

        StartMicrophone();
    }

    void StartMicrophone()
    {
        _currentMicrophoneDevice = string.IsNullOrEmpty(_microphoneDevice) ? Microphone.devices[0] : _microphoneDevice;

        if (Microphone.IsRecording(_currentMicrophoneDevice))
        {
            Microphone.End(_currentMicrophoneDevice);
        }

        _microphoneClip = Microphone.Start(_currentMicrophoneDevice, true, 1, AudioSettings.outputSampleRate);
        _sampleRate = _microphoneClip.frequency;

        if (_microphoneClip == null)
        {
            Debug.LogError($"Could not start microphone device: {_currentMicrophoneDevice}");
            return;
        }

        _audioSource.clip = _microphoneClip;
        _audioSource.loop = true;
        _audioSource.mute = true;

        while (!(Microphone.GetPosition(_currentMicrophoneDevice) > 0))
        {
            // Wait...
        }

        Debug.Log("Microphone started successfully.");
        _audioSource.Play();
    }

    void Update()
    {
        if (!Microphone.IsRecording(_currentMicrophoneDevice))
        {
            StartMicrophone();
            return;
        }

        CurrentLoudness = GetLoudness();

        if (CurrentLoudness > _pitchDetectionThreshold)
        {
            Debug.Log("Loud enough for pitch detection.");
            CurrentPitch = GetPitch();
        }
        else
        {
            CurrentPitch = 0f;
        }

        Debug.Log($"Loudness: {CurrentLoudness:F4}, Pitch: {CurrentPitch:F2} Hz");
    }

    private float GetLoudness()
    {
        if (_microphoneClip == null) return 0f;

        int micPosition = Microphone.GetPosition(_currentMicrophoneDevice);
        if (micPosition < _loudnessSampleWindow) return 0f;

        int startSample = micPosition - _loudnessSampleWindow;

        if (_micSampleBuffer.Length < _loudnessSampleWindow)
        {
            _micSampleBuffer = new float[_loudnessSampleWindow];
        }

        _microphoneClip.GetData(_micSampleBuffer, startSample);

        float totalLoudness = 0f;
        for (int i = 0; i < _loudnessSampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(_micSampleBuffer[i]);
        }
        return totalLoudness / _loudnessSampleWindow;
    }


    private float GetPitch()
    {
        if (_microphoneClip == null) return 0f;

        int micPosition = Microphone.GetPosition(_currentMicrophoneDevice);
        if (micPosition < _autocorrelationSampleWindow) return 0f;

        int startSample = micPosition - _autocorrelationSampleWindow;

        if (_micSampleBuffer.Length < _autocorrelationSampleWindow)
        {
            _micSampleBuffer = new float[_autocorrelationSampleWindow];
        }
        _microphoneClip.GetData(_micSampleBuffer, startSample);

        float maxCorrelation = 0f;
        int bestLag = 0;

        int minLag = (int)(_sampleRate / _maxFrequency);
        int maxLag = (int)(_sampleRate / _minFrequency);

        maxLag = Mathf.Min(maxLag, _autocorrelationSampleWindow - 1);

        for (int lag = minLag; lag < maxLag; lag++)
        {
            float correlation = 0f;

            for (int i = 0; i < _autocorrelationSampleWindow - lag; i++)
            {
                correlation += _micSampleBuffer[i] * _micSampleBuffer[i + lag];
            }

            if (correlation > maxCorrelation)
            {
                maxCorrelation = correlation;
                bestLag = lag;
            }
        }

        if (bestLag == 0) return 0f;

        float fundamentalFrequency = (float)_sampleRate / (float)bestLag;

        return fundamentalFrequency;
    }
}
