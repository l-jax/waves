using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PitchTracker : MonoBehaviour
{
    [Tooltip("The final, smoothed paddle position (0=Left, 0.5=Center, 1=Right)")]
    [Range(0f, 1f)]
    public float NormalizedPosition { get; private set; }

    [Header("Gameplay Pitch Control")]
    [Tooltip("The LOWEST note (Hz). This will be the far-left position (0.0).")]
    [SerializeField]
    private float _minFrequency = 100f;

    [Tooltip("The HIGHEST note (Hz). This will be the far-right position (1.0).")]
    [SerializeField]
    private float _maxFrequency = 400f;

    [Tooltip("The speed at which the paddle moves to the target pitch (0.1 is fast, 0.01 is slow).")]
    [SerializeField]
    private float _pitchSmoothingRate = 0.08f;

    [Header("Microphone Settings")]
    [Tooltip("The microphone device to use. Leave empty for default.")]
    [SerializeField]
    private string _microphoneDevice;

    [Tooltip("The minimum loudness threshold to trigger pitch detection.")]
    [SerializeField]
    private float _pitchDetectionThreshold = 0.005f;

    [Header("Pitch Algorithm Settings")]
    [Tooltip("The minimum confidence threshold for pitch detection.")]
    [SerializeField]
    private float _pitchConfidenceThreshold = 0.6f;
    
    [Tooltip("The sample window size for autocorrelation. Larger windows are better for low frequencies.")]
    [SerializeField]
    private int _autocorrelationSampleWindow = 2048;

    [Tooltip("The minimum frequency (in Hz) the algorithm should search for.")]
    [SerializeField]
    private float _minDetectionFrequency = 70f;

    [Tooltip("The maximum frequency (in Hz) the algorithm should search for.")]
    [SerializeField]
    private float _maxDetectionFrequency = 800f;
    
    private AudioSource _audioSource;
    private AudioClip _microphoneClip;
    private string _currentMicrophoneDevice;
    private float[] _micSampleBuffer;
    private int _sampleRate;
    private float _smoothedPitch = 0f;

    [Header("Debug Info (Read-Only)")]
    [SerializeField]
    private float _currentLoudness;
    [SerializeField]
    private float _currentRawPitch;
    [SerializeField]
    private float _currentNormalizedPosition;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _micSampleBuffer = new float[_autocorrelationSampleWindow];
        NormalizedPosition = 0.5f; // Start in the middle
        StartMicrophone();
    }

    void StartMicrophone()
    {
        _currentMicrophoneDevice = string.IsNullOrEmpty(_microphoneDevice) ? Microphone.devices[0] : _microphoneDevice;
        if (Microphone.IsRecording(_currentMicrophoneDevice)) Microphone.End(_currentMicrophoneDevice);
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
        while (!(Microphone.GetPosition(_currentMicrophoneDevice) > 0)) { } // Wait...
        _audioSource.Play();
    }

    void Update()
    {
        if (!Microphone.IsRecording(_currentMicrophoneDevice)) StartMicrophone();

        _currentLoudness = GetLoudness();
        if (_currentLoudness > _pitchDetectionThreshold) 
        {
            _currentRawPitch = GetPitch();
        } else {
            _currentRawPitch = 0f;
        }

        float targetPosition = NormalizedPosition;

        if (_currentRawPitch >= _minDetectionFrequency && _currentRawPitch <= _maxDetectionFrequency)
        {
            targetPosition = Mathf.InverseLerp(_minFrequency, _maxFrequency, _currentRawPitch);
        }
        NormalizedPosition = Mathf.Lerp(NormalizedPosition, targetPosition, _pitchSmoothingRate);
        _currentNormalizedPosition = NormalizedPosition;
    }

    private float GetLoudness()
    {
        if (_microphoneClip == null) return 0f;
        int micPosition = Microphone.GetPosition(_currentMicrophoneDevice);
        int sampleWindow = 128;
        if (micPosition < sampleWindow) return 0f;

        float[] loudnessBuffer = new float[sampleWindow];
        _microphoneClip.GetData(loudnessBuffer, micPosition - sampleWindow);
        float totalLoudness = 0f;
        for (int i = 0; i < sampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(loudnessBuffer[i]);
        }
        return totalLoudness / sampleWindow;
    }

    private float GetPitch()
    {
        if (_microphoneClip == null) return 0f;
        int micPosition = Microphone.GetPosition(_currentMicrophoneDevice);
        if (micPosition < _autocorrelationSampleWindow) return 0f;

        int startSample = micPosition - _autocorrelationSampleWindow;
        _microphoneClip.GetData(_micSampleBuffer, startSample);

        float maxCorrelation = 0f;
        int bestLag = 0;

        int minLag = (int)(_sampleRate / _maxDetectionFrequency);
        int maxLag = (int)(_sampleRate / _minDetectionFrequency);
        maxLag = Mathf.Min(maxLag, _autocorrelationSampleWindow - 1);

        for (int lag = minLag; lag < maxLag; lag++)
        {
            float correlation = 0f;
            float energy = 0f;
            for (int i = 0; i < _autocorrelationSampleWindow - lag; i++)
            {
                correlation += _micSampleBuffer[i] * _micSampleBuffer[i + lag];
                energy += _micSampleBuffer[i] * _micSampleBuffer[i] + _micSampleBuffer[i + lag] * _micSampleBuffer[i + lag];
            }
            if (energy > float.Epsilon) correlation /= energy * 0.5f;
            if (correlation > maxCorrelation)
            {
                maxCorrelation = correlation;
                bestLag = lag;
            }
        }

        if (maxCorrelation < _pitchConfidenceThreshold || bestLag == 0) return 0f;

        return _sampleRate / bestLag;
    }
}